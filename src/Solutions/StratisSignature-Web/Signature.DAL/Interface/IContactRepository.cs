using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Signature.Shared.Models;

namespace Signature.DAL.Interface
{
    public interface IContactRepository
    {
        Task<int> AddContact(Contact contact);

        Task<IEnumerable<Contact>> GetContacts(Guid userId);

        Task<int> DeleteContact(Guid contactId);
    }
}
