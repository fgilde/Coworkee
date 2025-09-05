using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lib.Coworkee.Infrastructure.Configurations;

public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        builder.ToTable("UserClaims", "Identity");
    }
}