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

        public async Task<List<Contact>> GetAllAsync(string? search = null)
        {
            var query = _context.Contacts
                .AsNoTracking()
                .AsQueryable();
                //.OrderBy(c => c.Name)
                //.ToListAsync();

            if(!string.IsNullOrWhiteSpace(search))
            {
                var normalizeSearch = search.Trim().ToLower();

                query = query.Where(c =>
                    c.Name.ToLower().Contains(normalizeSearch) ||
                    c.Email.ToLower().Contains(normalizeSearch) ||
                    c.Phone.ToLower().Contains(normalizeSearch)
                );

            };

            return await query
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<(List<Contact> Items, int TotalItems)> GetPagedAsync(string? search, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;
            pageSize = Math.Min(pageSize, 100);

            var query = _context.Contacts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var like = $"%{search.Trim()}%";
                query = query.Where(c =>
                    EF.Functions.Like(c.Name, like) ||
                    EF.Functions.Like(c.Email, like) ||
                    EF.Functions.Like(c.Phone, like)
                );
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
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
