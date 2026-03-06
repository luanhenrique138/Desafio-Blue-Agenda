using System.Text.Json;
using Agenda.Application.Services;
using Agenda.Domain.Enum;
using Agenda.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Api.Workers
{
    public class EmailOutboxWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailOutboxWorker> _logger;

        public EmailOutboxWorker(IServiceScopeFactory scopeFactory, ILogger<EmailOutboxWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailOutboxWorker iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();
                    var sender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

                    var now = DateTime.UtcNow;

                    // 1) Buscar pendentes que já podem rodar
                    var pending = await db.EmailOutboxes
                        .Where(x => x.Status == EmailStatus.Pending &&
                                    (x.NextRetryAt == null || x.NextRetryAt <= now))
                        .OrderBy(x => x.CreatedAt)
                        .Take(10)
                        .ToListAsync(stoppingToken);

                    if (pending.Count == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                        continue;
                    }

                    // 2) Marcar como Sending
                    foreach (var item in pending)
                    {
                        item.Status = EmailStatus.Sending;
                        item.UpdatedAt = DateTime.UtcNow;
                    }

                    await db.SaveChangesAsync(stoppingToken);

                    // 3) Enviar um por um
                    foreach (var item in pending)
                    {
                        try
                        {
                            var recipients = JsonSerializer.Deserialize<List<string>>(item.ToEmails) ?? new List<string>();
                            recipients = recipients.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

                            if (recipients.Count == 0)
                                throw new Exception("Destinatários vazios/ inválidos.");

                            await sender.SendAsync(
                                item.FromEmail,
                                item.FromName,
                                recipients,
                                item.Subject,
                                item.Body,
                                item.IsHtml,
                                stoppingToken
                            );

                            item.Status = EmailStatus.Sent;
                            item.SentAt = DateTime.UtcNow;
                            item.UpdatedAt = DateTime.UtcNow;
                            item.LastError = null;
                            item.NextRetryAt = null;
                        }
                        catch (Exception ex)
                        {
                            item.Attempts += 1;
                            item.LastError = ex.Message;
                            item.UpdatedAt = DateTime.UtcNow;

                            if (item.Attempts < item.MaxAttempts)
                            {
                                // backoff simples: 30s, 60s, 120s...
                                var delaySeconds = 30 * Math.Pow(2, item.Attempts - 1);
                                item.Status = EmailStatus.Pending;
                                item.NextRetryAt = DateTime.UtcNow.AddSeconds(delaySeconds);
                            }
                            else
                            {
                                item.Status = EmailStatus.Error;
                            }

                            _logger.LogError(ex, "Erro ao enviar EmailOutbox {Id}", item.Id);
                        }

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no loop do EmailOutboxWorker");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("EmailOutboxWorker finalizado.");
        }
    }
}