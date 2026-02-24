using Agenda.Application.DTOs.Requests;
using Agenda.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Application.Services
{
    public interface IContactService
    {
        Task<List<ContactResponse>> GetAllAsync();
        Task<ContactResponse?> GetByIdAsync(Guid id);
        Task<ContactResponse> CreateAsync(CreateContactRequest request);
        Task<ContactResponse?> UpdateAsync(Guid id,UpdateContactRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
