using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Tokens;

namespace Security.Infrastructure.Persistence.Configurations.Tokens;

public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.Used)
            .IsRequired();

        builder.Property(x => x.UsedAtUtc);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("ix_password_reset_tokens_token_hash");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_password_reset_tokens_user_id");

        builder.HasIndex(x => x.ExpiresAtUtc)
            .HasDatabaseName("ix_password_reset_tokens_expires_at_utc");
    }
}