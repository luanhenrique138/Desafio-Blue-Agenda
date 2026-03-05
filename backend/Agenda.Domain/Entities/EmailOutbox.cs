using Agenda.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Domain.Entities
{
    public class EmailOutbox
    {
        public Guid Id { get; set; }
        
        public string ToEmails { get; set; } = default;
        
        public string Subject { get; set; } = default;
        public string Body { get; set; } = default;
        
        public string FromEmail { get; set; } = default;
        public string? FromName { get; set; }
        
        public bool IsHtml { get; set; }
        
        public EmailStatus Status { get; set; } = EmailStatus.Pending;

        // Controle de fila
        public int Attempts { get; set; } = 0;
        public int MaxAttempts { get; set; } = 3;
        public string? LastError { get; set; }
        public DateTime? NextRetryAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
