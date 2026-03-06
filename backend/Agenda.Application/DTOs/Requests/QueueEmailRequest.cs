using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Application.DTOs.Requests
{
    public record QueueEmailRequest
    {
        public List<string> ToEmails {  get; set; } = new List<string>();

        public string Subject { get; set; } = default(string);
        public string Body { get; set; } = default(string);
        
        public bool IsHtml { get; set; } = true;

        public string FromEmail { get; set; } = default(string);
        public string? FromName { get; set; }
    }
}
