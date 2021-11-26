using System;

namespace Signature.Shared.Models
{
    public class SignDocument
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
        public string SignTransactionHash { get; set; }
        public string SignedDocHash { get; set; }
        public long BlockNumber { get; set; }
        public byte[] SignedDocumentData { get; set; }
    }
}
