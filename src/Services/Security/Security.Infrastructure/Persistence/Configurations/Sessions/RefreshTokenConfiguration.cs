using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Sessions;

namespace Security.Infrastructure.Persistence.Configurations.Sessions;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.SessionId)
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.Consumed)
            .IsRequired();

        builder.Property(x => x.ConsumedAtUtc);

        builder.Property(x => x.Revoked)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("ix_refresh_tokens_token_hash");

        builder.HasIndex(x => x.SessionId)
            .HasDatabaseName("ix_refresh_tokens_session_id");

        builder.HasIndex(x => x.ExpiresAtUtc)
            .HasDatabaseName("ix_refresh_tokens_expires_at_utc");

        builder.HasIndex(x => new { x.SessionId, x.Revoked, x.Consumed })
            .HasDatabaseName("ix_refresh_tokens_session_state");
    }
}