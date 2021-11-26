using System;
using System.ComponentModel.DataAnnotations;

namespace Signature.Web.Models
{
    public class ContactViewModel
    {
        public Guid ContactId { get; set; }
        public Guid UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
