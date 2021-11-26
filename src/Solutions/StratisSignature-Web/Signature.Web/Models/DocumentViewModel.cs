using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Signature.Shared.Models;

namespace Signature.Web.Models
{
    public class DocumentViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Document Name")]
        public string DocumentName { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public byte[] Data { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        public List<User> Signers { get; set; }

        [DisplayName("Created On")]
        public DateTime CreationTime { get; set; }

        [DisplayName("Uploaded By")]        
        public string UploadedBy { get; set; }

        [DisplayName("Signers")]
        [Required(ErrorMessage = "At least one signer is required")]
        public string[] SignerIds { get; set; }
    }
}
