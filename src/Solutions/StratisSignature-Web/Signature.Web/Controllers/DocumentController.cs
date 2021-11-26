using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Signature.BAL.Interface;
using Signature.Shared.Models;
using Signature.Utility;
using Signature.Web.Extensions;
using Signature.Web.Models;

namespace Signature.Web.Controllers
{
    [Authorize]
    public class DocumentController : BaseController
    {
        private readonly IContactService _contactService;
        private readonly IDocumentService _documentService;
        private readonly IBlockchainApi _blockchainApi;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IPdfService _pdfService;

        public DocumentController(IContactService contactService,
            IDocumentService documentService,
            IBlockchainApi blockchainApi,
            IOptions<AppSettings> appSettings,
            IPdfService pdfService)
        {
            _contactService = contactService;
            _documentService = documentService;
            _blockchainApi = blockchainApi;
            _appSettings = appSettings;
            _pdfService = pdfService;
        }

        private async Task PopulateSigners()
        {
            var userContacts = await _contactService.GetContacts(User.GetUserId());
            ViewBag.Signers = new MultiSelectList(userContacts, "UserId", "FullName");
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _documentService.GetDocuments(User.GetUserId());
            return View(documents.ToListModel());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateSigners();
            return View("CreateDocument");
        }

        [HttpPost]
        public async Task<IActionResult> Create(DocumentViewModel model)
        {
            
            if (!ModelState.IsValid) return JsonError(Constants.InvalidInputs);

            using (var ms = new MemoryStream())
            {
                await model.File.CopyToAsync(ms);
                model.Data = ms.ToArray();
            }

            var document = new Document
            {
                CreatedBy = User.GetUserId(),
                DocumentName = model.DocumentName,
                Description = model.Description,
                FileName = model.File.FileName,
                Data = model.Data,
                Status = DocumentStatus.Unsigned,
                SignerIds = model.SignerIds,
                DocumentHash = model.Data.Sha256Hash()
            };

            var documentId = await _documentService.AddDocument(document);
            if (!documentId.IsValidGuid())
                return JsonError(Constants.DocumentErrorDuringDbProcessing);

            var docCreateTransaction = await _blockchainApi.CreateDocument(documentId, document.DocumentHash, User.GetWalletAddress());

            if (!docCreateTransaction.Success)
                return JsonError(docCreateTransaction.Message);
            for (int i = 0; i < 2; i++)
            {
                await Task.Delay(_appSettings.Value.AverageBlockTime);
                var receipt = await _blockchainApi.GetReceipt(docCreateTransaction.TransactionId);
                if (receipt.Success)
                {
                    await _documentService.UpdateDocumentCreationHash(Guid.Parse(documentId), docCreateTransaction.TransactionId);
                    return JsonSuccess(Constants.DocumentCreationSuccess, data: documentId);
                }
            }

            return JsonError(Constants.DocumentErrorDuringBlockchainProcessing);
        }

        [HttpPost]
        public async Task<IActionResult> AddSigners(Guid documentId)
        {            
            var signers = await _documentService.GetDocumentSigners(documentId);
            var documentSigners = signers as DocumentSigner[] ?? signers.ToArray();

            if (!documentSigners.Any()) return JsonError(Constants.DocumentInvalidSigner);

            foreach (var signer in documentSigners)
            {
                var addSignerTransaction = await _blockchainApi.AddSigner(documentId.ToString(), User.GetWalletAddress(), signer.WalletAddress);
                if (!addSignerTransaction.Success)
                    return JsonError(addSignerTransaction.Message);
            }

            ApiLocalCallResponse<Agreement> localCall;
            do
            {
                await Task.Delay(_appSettings.Value.AverageBlockTime);

                localCall = await _blockchainApi.GetDocument(documentId.ToString(), User.GetWalletAddress());

            } while (localCall.Return.Signers.Count < documentSigners.AsEnumerable().Count());
            return JsonSuccess(Constants.DocumentCreationSuccess);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid documentId)
        {
            var documentDetail = await _documentService.GetDocumentDetail(documentId);
            return View("DocumentDetail", documentDetail.ToSharedModel());
        }

        [HttpPost]
        public async Task<IActionResult> Sign(Guid documentId, string password)
        {            
            var documentDetail = await _documentService.GetDocumentDetail(documentId);
            if (documentDetail == null)
            {
                return JsonError(Constants.DocumentWasNotFound);
            }

            if (!documentDetail.Signers.Exists(x => x.UserId == User.GetUserId()))
            {
                return JsonError(Constants.DocumentInvalidSigner);
            }

            //generate digital sign
            var digitalSign = await _blockchainApi.SignMessage(documentDetail.Document.Id.ToString(), User.GetWalletAddress(), password);
            if (string.IsNullOrEmpty(digitalSign))
                return JsonError(Constants.DocumentErrorDuringSigning);

            var signTransaction = await _blockchainApi.SignDocument(documentId.ToString(), User.GetWalletAddress(), digitalSign);
            if (!signTransaction.Success)
                return Json(signTransaction.Message);

            var receipt = new TransactionReceipt();

            for (int i = 0; i < 2; i++)
            {
                await Task.Delay(_appSettings.Value.AverageBlockTime);
                receipt = await _blockchainApi.GetReceipt(signTransaction.TransactionId);
                if (receipt.Success)
                    break;

                if (string.IsNullOrEmpty(receipt.Error))
                    return JsonError(receipt.Error);
            }

            if (!receipt.Success)
                return JsonError(receipt.Error.ToString());

            var signedFilePath = await _pdfService.StampingFile(documentDetail, User.GetUserId(), User.GetFullName());
            if (!System.IO.File.Exists(signedFilePath))
                return JsonError(Constants.DocumentErrorDuringSigning);

            var signedBytes = System.IO.File.ReadAllBytes(signedFilePath);
            var signedFileHash = signedBytes.Sha256Hash();

            var localCall = await _blockchainApi.GetStamp(documentId.ToString(), User.GetWalletAddress());

            var signDocumentModel = new SignDocument
            {
                DocumentId = documentId,
                UserId = User.GetUserId(),
                SignTransactionHash = signTransaction.TransactionId,
                BlockNumber = localCall.Return.BlockNumber,
                SignedDocumentData = signedBytes,
                SignedDocHash = signedFileHash
            };

            await _documentService.SignDocument(signDocumentModel);

            System.IO.File.Delete(signedFilePath);

            return JsonSuccess(Constants.DocumentSignSuccess);
        }

        [HttpGet]
        public async Task<IActionResult> Download(Guid id)
        {
            var detail = await _documentService.GetDocumentDetail(id);
            if (detail == null)
                return null;

            return File(detail.Document.SignedData, "application/force-download", $"{Path.GetFileNameWithoutExtension(detail.Document.FileName)}.{FileExtension.Pdf}");
        }
    }
}