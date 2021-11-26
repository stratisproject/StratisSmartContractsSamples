using System;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Signature.BAL.Interface;
using Signature.Shared.Models;

namespace Signature.BAL
{
    public class PdfService : IPdfService
    {
        private readonly IHostingEnvironment _env;
        private readonly IDocumentService _documentService;

        public PdfService(IHostingEnvironment env, IDocumentService documentService)
        {
            _env = env;
            _documentService = documentService;
        }

        public async Task<string> StampingFile(DocumentDetail documentDetail, Guid userId, string userFullName)
        {
            //save previous data to the file
            var tempDirectoryPath = Path.Combine(_env.WebRootPath, "TempPath", userId.ToString(), documentDetail.Document.Id.ToString());
            if (!Directory.Exists(tempDirectoryPath))
            {
                Directory.CreateDirectory(tempDirectoryPath);
            }

            var filePath = Path.Combine(tempDirectoryPath, documentDetail.Document.FileName);
            using (Stream file = File.OpenWrite(filePath))
            {
                file.Write(documentDetail.Document.SignedData ?? documentDetail.Document.Data, 0, documentDetail.Document.Data.Length);
            }

            var shape = await _documentService.GetShapeForSign(documentDetail.Document.Id, userId);

            var reader = new PdfReader(filePath);
            var document = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1));
            var newFile = Path.Combine(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)}_signed.pdf");
            try
            {
                using (var fileStream = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                {
                    var writer = PdfWriter.GetInstance(document, fileStream);

                    document.Open();

                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        document.NewPage();

                        var importedPage = writer.GetImportedPage(reader, 1);

                        var contentByte = writer.DirectContent;
                        contentByte.BeginText();
                        contentByte.SetFontAndSize(BaseFont.CreateFont(), 10);
                        contentByte.SetColorFill(BaseColor.Red);

                        var sign = $"Signed by {userFullName}";

                        contentByte.ShowTextAligned(Element.ALIGN_CENTER, sign, shape.X, shape.Y, 0);

                        contentByte.EndText();
                        contentByte.AddTemplate(importedPage, 0, 0);
                    }
                    writer.Close();
                }
            }
            finally
            {
                document.Close();
                File.Delete(filePath);
            }
            return newFile;
        }
    }
}
