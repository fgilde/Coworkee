using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using lib.Coworkee.Domain.Entities.Identity;

namespace lib.Coworkee.Infrastructure.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasOne(d => d.UserInformations)
            .WithMany(p => p.Addresses)
            .HasForeignKey(d => d.UserInformationsId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}