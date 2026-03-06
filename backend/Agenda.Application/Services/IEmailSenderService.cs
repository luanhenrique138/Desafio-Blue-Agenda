using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Application.Services
{
    public interface IEmailSenderService
    {
        Task SendAsync(
            string fromEmail,
            string? fromName,
            IEnumerable<string> to,
            string subject,
            string body,
            bool isHtml,
            CancellationToken ct
        );
    }
}
