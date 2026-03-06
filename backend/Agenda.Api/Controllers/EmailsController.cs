using Agenda.Application.DTOs.Requests;
using Agenda.Domain.Entities;
using Agenda.Domain.Enum;
using Agenda.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Api.Controllers
{
    [ApiController]
    [Route("api/emails")]
    public class EmailsController : ControllerBase
    {
        private readonly AgendaDbContext _db;

        public EmailsController(AgendaDbContext db) => _db = db;

        [HttpPost("queue")]
        public async Task<IActionResult> Queue([FromBody] QueueEmailRequest req, CancellationToken ct)
        {
            if (req.ToEmails == null || req.ToEmails.Count == 0) return BadRequest("Informe ao menos 1 destinatário.");
            if (string.IsNullOrWhiteSpace(req.Subject)) return BadRequest("Assunto é obrigatório.");
            if (string.IsNullOrWhiteSpace(req.Body)) return BadRequest("Conteúdo é obrigatório.");

            var emailOutbox = new EmailOutbox
            {
                Id = Guid.NewGuid(),
                ToEmails = System.Text.Json.JsonSerializer.Serialize(req.ToEmails.Distinct().ToList()),
                Subject = req.Subject,
                Body = req.Body,
                FromEmail = req.FromEmail ?? "no-reply@agenda.local",
                FromName = req.FromName,
                IsHtml = req.IsHtml,
                Status = EmailStatus.Pending,
                Attempts = 0,
                MaxAttempts = 3,
                CreatedAt = DateTime.UtcNow
            };

            _db.EmailOutboxes.Add(emailOutbox);
            await _db.SaveChangesAsync(ct);

            return Ok(new { id = emailOutbox.Id, status = emailOutbox.Status.ToString() });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetStatus(Guid id, CancellationToken ct)
        {
            var item = await _db.EmailOutboxes.FindAsync(new object[] { id }, ct);
            if (item == null) return NotFound();

            return Ok(new
            {
                item.Id,
                item.Status,
                item.Attempts,
                item.MaxAttempts,
                item.LastError,
                item.CreatedAt,
                item.SentAt
            });
        }
    }
}
