using Coworkee.Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coworkee.Infrastructure.Configurations;

public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        builder.ToTable(name: "RoleClaims", "Identity");

        builder.HasOne(d => d.Role)
            .WithMany(p => p.RoleClaims)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}