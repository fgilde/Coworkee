using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lib.Coworkee.Infrastructure.Configurations;

public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
    {
        builder.ToTable("UserTokens", "Identity");
    }
}