using System;

namespace Signature.Shared.Models
{
    public class Contact
    {
        public Guid ContactId { get; set; }

        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public Guid RequestedBy { get; set; }
    }
}
