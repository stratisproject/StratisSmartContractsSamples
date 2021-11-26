using System.Collections.Generic;

namespace Signature.Shared.Models
{
    public class DocumentDetail
    {
        public DocumentDetail()
        {
            Document = new Document();
            Signers = new List<DocumentSigner>();
        }
        public Document Document { get; set; }
        public List<DocumentSigner> Signers { get; set; }
    }
}
