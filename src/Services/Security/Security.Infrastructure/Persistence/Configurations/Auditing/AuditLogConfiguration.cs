using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Auditing;

namespace Security.Infrastructure.Persistence.Configurations.Auditing;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId);

        builder.Property(x => x.ActionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.UserAgent)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PayloadJson)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_audit_logs_user_id");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_audit_logs_created_at_utc");

        builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc })
            .HasDatabaseName("ix_audit_logs_user_id_created_at_utc");
    }
}