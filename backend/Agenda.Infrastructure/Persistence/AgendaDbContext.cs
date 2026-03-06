using Agenda.Domain.Entities;
using Agenda.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Persistence
{
    public class AgendaDbContext : DbContext
    {
        public AgendaDbContext(DbContextOptions<AgendaDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<EmailOutbox> EmailOutboxes => Set<EmailOutbox>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Phone)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });

            modelBuilder.Entity<EmailOutbox>(entity =>
            {
                entity.ToTable("EmailOutboxes");

                entity.HasKey(x => x.Id);

                // "To" é um nome meio ruim (pode confundir). Sugiro renomear a coluna:
                entity.Property(x => x.ToEmails)
                    .IsRequired()
                    .HasColumnName("ToEmails"); // você pode escolher outro nome

                entity.Property(x => x.Subject)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(x => x.Body)
                    .IsRequired();

                entity.Property(x => x.FromEmail)
                    .IsRequired()
                    .HasMaxLength(320);

                entity.Property(x => x.FromName)
                    .HasMaxLength(200);

                entity.Property(x => x.IsHtml)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasConversion<int>()
                    .IsRequired()
                    .HasDefaultValue(EmailStatus.Pending.ToString());

                entity.Property(x => x.Attempts)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(x => x.MaxAttempts)
                    .IsRequired()
                    .HasDefaultValue(3);

                entity.Property(x => x.LastError);

                entity.Property(x => x.NextRetryAt);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.UpdatedAt);
                entity.Property(x => x.SentAt);

                // índice útil pro worker buscar pendentes
                entity.HasIndex(x => new { x.Status, x.NextRetryAt });
            });
        }
    }
}