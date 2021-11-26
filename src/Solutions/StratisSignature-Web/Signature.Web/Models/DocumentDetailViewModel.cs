using System.Collections.Generic;
using Signature.Shared.Models;

namespace Signature.Web.Models
{
    public class DocumentDetailViewModel
    {
        public DocumentDetailViewModel()
        {
            Document = new Document();
            Signers = new List<DocumentSigner>();
        }
        public Document Document { get; set; }
        public List<DocumentSigner> Signers { get; set; }
    }
}
