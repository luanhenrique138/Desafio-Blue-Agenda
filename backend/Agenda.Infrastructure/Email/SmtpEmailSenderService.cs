using Agenda.Application.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Agenda.Infrastructure.Email
{
    public class SmtpEmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailSenderService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(
            string fromEmail,
            string? fromName,
            IEnumerable<string> to,
            string subject,
            string body,
            bool isHtml,
            CancellationToken ct)
        {
            var message = new MimeMessage();

            // From
            if (string.IsNullOrWhiteSpace(fromName))
                message.From.Add(MailboxAddress.Parse(fromEmail));
            else
                message.From.Add(new MailboxAddress(fromName, fromEmail));

            // To
            foreach (var addr in to.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
                message.To.Add(MailboxAddress.Parse(addr));

            message.Subject = subject;

            // Body
            if (isHtml)
            {
                var builder = new BodyBuilder { HtmlBody = body };
                message.Body = builder.ToMessageBody();
            }
            else
            {
                message.Body = new TextPart("plain") { Text = body };
            }

            using var client = new SmtpClient();

            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl, ct);

            if (!string.IsNullOrWhiteSpace(_settings.User))
                await client.AuthenticateAsync(_settings.User, _settings.Pass, ct);

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}