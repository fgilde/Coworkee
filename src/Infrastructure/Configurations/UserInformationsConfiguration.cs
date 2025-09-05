using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using lib.Coworkee.Domain.Entities.Identity;

namespace Coworkee.Infrastructure.Configurations;

public class UserInformationsConfiguration : IEntityTypeConfiguration<UserInformations>
{
    public void Configure(EntityTypeBuilder<UserInformations> builder)
    {
        builder.HasMany(u => u.Addresses)
            .WithOne(a => a.UserInformations)
            .HasForeignKey(d => d.UserInformationsId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}