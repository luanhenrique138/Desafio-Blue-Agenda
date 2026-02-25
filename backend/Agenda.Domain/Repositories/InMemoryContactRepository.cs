using Agenda.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Domain.Repositories
{
    public
        class InMemoryContactRepository : IContactRepository
    {
        private readonly List<Contact> _contacts = new();
        public Task<Contact> AddAsync(Contact contact)
        {
            _contacts.Add(contact);
            return Task.FromResult(contact);
        }

        public Task DeleteAsync(Guid id)
        {
            _contacts.RemoveAll(contact => contact.Id == id);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            var exited = _contacts.Exists(x => x.Email.Equals(email));
            return Task.FromResult(exited);
        }

        public Task<List<Contact>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<Contact>> GetAllAsync(string? search = null)
        {
            throw new NotImplementedException();
        }

        public Task<Contact?> GetByIdAsync(Guid id)
        {
            var contact = _contacts.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(contact);
        }

        public Task<Contact> UpdateAsync(Contact contact)
        {
            var index = _contacts.FindIndex(c => c.Id == contact.Id);
            if(index >= 0) 
            { 
                _contacts[index] = contact;
            }

            return Task.FromResult(contact);

        }
    }
}
