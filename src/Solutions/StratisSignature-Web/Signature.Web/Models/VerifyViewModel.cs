using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Signature.Web.Models
{
    public class VerifyViewModel
    {
        [Required(ErrorMessage = "Document is required")]        
        public IFormFile File { get; set; }      

        [Required(ErrorMessage = "WalletAddress required")]
        [DisplayName("Wallet Address")]
        public string SignerWalletAddress { get; set; }
        
        public byte[] FileBytes { get; set; }
    }
}
