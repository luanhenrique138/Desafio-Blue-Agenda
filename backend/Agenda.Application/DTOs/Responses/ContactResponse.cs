using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Application.DTOs.Responses
{
    public record ContactResponse
    {
        public Guid Id { get; init; }

        public string Name {  get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Phone { get; init; } = string.Empty;

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
     };
}
