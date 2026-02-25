using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agenda.Domain.Entities;

namespace Agenda.Domain.Repositories
{
    public interface IContactRepository
    {
        Task<(List<Contact> Items, int TotalItems)> GetPagedAsync(string? search, int page, int pageSize);

        Task<Contact> AddAsync (Contact contact);
        Task<Contact?> GetByIdAsync(Guid id);
        Task<Contact> UpdateAsync(Contact contact);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
