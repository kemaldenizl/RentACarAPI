using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Users;

namespace Security.Infrastructure.Persistence.Configurations.Users;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.Property(x => x.AssignedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.RoleId)
            .HasDatabaseName("ix_user_roles_role_id");
    }
}