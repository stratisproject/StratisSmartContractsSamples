using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Signature.BAL.Interface;
using Signature.Utility;
using Signature.Web.Models;

namespace Signature.Web.Controllers
{
    public class VerifyController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IBlockchainApi _blockchainApi;
        private readonly IToastNotification _toastNotification;

        public VerifyController(IDocumentService documentService,
            IBlockchainApi blockchainApi,
            IToastNotification toastNotification)
        {
            _documentService = documentService;
            _blockchainApi = blockchainApi;
            _toastNotification = toastNotification;
        }


        [HttpGet]
        public IActionResult Verify()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify(VerifyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var ms = new MemoryStream())
            {
                await model.File.CopyToAsync(ms);
                model.FileBytes = ms.ToArray();
            }

            var document = await _documentService.SearchDocument(model.FileBytes.Sha256Hash());
            if (document == null)
            {
                _toastNotification.AddErrorToastMessage(Constants.DocumentWasNotFound);
                return View();
            }

            var stamp = await _blockchainApi.GetStamp(document.Id.ToString(), model.SignerWalletAddress);
            if (!string.IsNullOrEmpty(stamp.ErrorMessage))
            {
                _toastNotification.AddErrorToastMessage(stamp.ErrorMessage);
                return View();
            }

            if (stamp.Return != null && stamp.Return.Signer.Equals(model.SignerWalletAddress, StringComparison.OrdinalIgnoreCase))
            {
                _toastNotification.AddSuccessToastMessage(Constants.DocumentVerifySuccess);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(Constants.DocumentVerifyFailure);
            }

            return View();
        }
    }
}