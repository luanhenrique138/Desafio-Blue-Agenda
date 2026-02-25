using Agenda.Application.DTOs.Requests;
using Agenda.Application.DTOs.Responses;
using Agenda.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Application.Services
{
    public interface IContactService
    {
        Task<PagedResult<ContactResponse>> GetAllAsync(string? search = null, int page = 1, int pageSize = 10);
        Task<ContactResponse?> GetByIdAsync(Guid id);
        Task<ContactResponse> CreateAsync(CreateContactRequest request);
        Task<ContactResponse?> UpdateAsync(Guid id,UpdateContactRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
