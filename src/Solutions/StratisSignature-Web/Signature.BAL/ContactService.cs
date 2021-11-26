using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Signature.BAL.Interface;
using Signature.DAL.Interface;
using Signature.Shared.Models;

namespace Signature.BAL
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<int> AddContact(Contact contact)
        {
            return await _contactRepository.AddContact(contact);
        }

        public async Task<IEnumerable<Contact>> GetContacts(Guid userId)
        {
            return await _contactRepository.GetContacts(userId);
        }

        public async Task<int> DeleteContact(Guid contactId)
        {
            return await _contactRepository.DeleteContact(contactId);
        }
    }
}
