using System;
using Signature.Utility;

namespace Signature.Shared.Models
{
    public class Document
    {
        public Guid Id { get; set; }

        public string DocumentName { get; set; }

        public string FileName { get; set; }

        public string Description { get; set; }

        public string DocumentHash { get; set; }

        public byte[] Data { get; set; }

        public byte[] SignedData { get; set; }

        public DocumentStatus Status { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid CreatedBy { get; set; }

        public string UploadedBy { get; set; }

        public string[] SignerIds { get; set; }

        public string SignTxHash { get; set; }

        public long BlockNumber { get; set; }
    }
}
