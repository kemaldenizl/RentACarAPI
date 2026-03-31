using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Sessions;

namespace Security.Infrastructure.Persistence.Configurations.Sessions;

public sealed class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        builder.ToTable("refresh_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.DeviceName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Revoked)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.RevokedAtUtc);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_refresh_sessions_user_id");

        builder.HasIndex(x => new { x.UserId, x.Revoked })
            .HasDatabaseName("ix_refresh_sessions_user_id_revoked");

        builder.Ignore(x => x.DomainEvents);

        builder.HasMany<RefreshToken>("_tokens")
            .WithOne()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Tokens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}