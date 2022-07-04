using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanArchitectureBase.Domain.Entities.Identity;

namespace CleanArchitectureBase.Infrastructure.Configurations;

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