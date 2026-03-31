using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Domain.Authorization;

namespace Security.Infrastructure.Persistence.Configurations.Authorization;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(x => new { x.RoleId, x.PermissionId });

        builder.Property(x => x.AssignedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.PermissionId)
            .HasDatabaseName("ix_role_permissions_permission_id");
    }
}