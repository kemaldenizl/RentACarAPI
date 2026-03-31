using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Users;

namespace Security.Infrastructure.Persistence.Configurations.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.NormalizedEmail)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.EmailVerified)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.LastLoginAtUtc);

        builder.HasIndex(x => x.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("ix_users_normalized_email");

        builder.Ignore(x => x.DomainEvents);

        builder.HasMany<UserRole>("_roles")
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Roles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}