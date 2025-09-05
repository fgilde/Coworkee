using lib.Coworkee.Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lib.Coworkee.Infrastructure.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable(name: "Users", "Identity");
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.HasOne(u => u.UserInfo);
    }
}