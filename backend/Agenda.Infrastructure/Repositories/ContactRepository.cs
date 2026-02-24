using Agenda.Domain.Entities;
using Agenda.Domain.Repositories;
using Agenda.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly AgendaDbContext _context;
        public ContactRepository(AgendaDbContext context)
        {
            _context = context;
        }

        public async Task<Contact> AddAsync(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return contact;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return;

            _context.Contacts.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Contacts.AnyAsync(x => x.Email == email);
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            
            return entity;
        }

        public async Task<Contact> UpdateAsync(Contact contact)
        {
            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
            return contact;

        }
    }
}
