using Wta.Infrastructure.Scheduling;

namespace Wta.Application.Default.Data;

public class DefaultDbConfig : BaseDbConfig<DefaultDbContext>,
    IEntityTypeConfiguration<Tenant>,
    IEntityTypeConfiguration<Job>,
    IEntityTypeConfiguration<Dict>,
    IEntityTypeConfiguration<Department>,
    IEntityTypeConfiguration<User>,
    IEntityTypeConfiguration<Role>,
    IEntityTypeConfiguration<Permission>,
    IEntityTypeConfiguration<UserRole>,
    IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.Property(o => o.Name).IsRequired();
        builder.Property(o => o.Number).IsRequired();
        builder.HasIndex(o => o.Number).IsUnique();
    }
    public void Configure(EntityTypeBuilder<Job> builder)
    {

    }
    public void Configure(EntityTypeBuilder<Dict> builder)
    {
    }

    public void Configure(EntityTypeBuilder<Department> builder)
    {
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(o => o.Department).WithMany(o => o.Users).HasForeignKey(o => o.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(o => new { o.TenantNumber, o.NormalizedUserName }).IsUnique();
        builder.Navigation(o => o.UserRoles).AutoInclude();
    }

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(o => new { o.TenantNumber, o.Number }).IsUnique();
        builder.Navigation(o => o.RolePermissions).AutoInclude();
    }

    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(o => new { o.UserId, o.RoleId });
        builder.HasOne(o => o.User).WithMany(o => o.UserRoles).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(o => o.Role).WithMany(o => o.UserRoles).HasForeignKey(o => o.RoleId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<Permission> builder)
    {
    }

    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(o => new { o.RoleId, o.PermissionId });
        builder.HasOne(o => o.Role).WithMany(o => o.RolePermissions).HasForeignKey(o => o.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(o => o.Permission).WithMany(o => o.RolePermissions).HasForeignKey(o => o.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }

}
