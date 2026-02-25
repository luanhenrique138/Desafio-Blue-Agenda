using Agenda.Application.DTOs.Requests;
using Agenda.Application.DTOs.Responses;
using Agenda.Domain.Entities;
using Agenda.Domain.Repositories;
using AutoMapper;

namespace Agenda.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
        public ContactService(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }


        public class ConflictException : Exception
        {
            public ConflictException(string message) : base(message) { }
        }

        public async Task<ContactResponse> CreateAsync(CreateContactRequest request)
        {
            var isExistedEmail = await _contactRepository.ExistsByEmailAsync(request.Email);

            if (isExistedEmail)
                throw new ConflictException("Email já existe.");

            var contact = _mapper.Map<Contact>(request);
            contact.Id = Guid.NewGuid();
            contact.CreatedAt = DateTime.UtcNow;

            var created = await _contactRepository.AddAsync(contact);
            return _mapper.Map<ContactResponse>(created);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);

            if (contact is null)
                return false;

            await _contactRepository.DeleteAsync(id);

            return true;
            
        }

        public async Task<List<ContactResponse>> GetAllAsync(string? search = null)
        {
            var listContacts = await _contactRepository.GetAllAsync(search);
   
            return _mapper.Map<List<ContactResponse>>(listContacts);
        }

        public async Task<ContactResponse?> GetByIdAsync(Guid id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);

            if (contact is null)
                return null;

            return _mapper.Map<ContactResponse>(contact);
        }

        public async Task<ContactResponse?> UpdateAsync(Guid id, UpdateContactRequest request)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if(contact is null) 
                return null;

            if (!string.Equals(contact.Email, request.Email, StringComparison.OrdinalIgnoreCase)) 
            {
                var isExistedEmail = await _contactRepository.ExistsByEmailAsync(request.Email);
                if (isExistedEmail)
                    throw new ConflictException("Email já existe.");
            }

            _mapper.Map(request, contact);
            contact.UpdatedAt = DateTime.UtcNow;

            var updated = await _contactRepository.UpdateAsync(contact);

            return _mapper.Map<ContactResponse>(updated);
            
        }
    }
}
