using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Agenda.Application.DTOs.Requests;
using Agenda.Application.DTOs.Responses;
using Agenda.Domain.Entities;


namespace Agenda.Application.Mapping
{
    public class ContactProfile : Profile
    {
        public ContactProfile() 
        {
            CreateMap<Contact, ContactResponse>();

            CreateMap<CreateContactRequest, Contact>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateContactRequest, Contact>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore());

        }
    }
}
