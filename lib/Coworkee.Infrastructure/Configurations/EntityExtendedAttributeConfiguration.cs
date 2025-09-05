using Coworkee.Application.Serialization.Options;
using Coworkee.Application.Serialization.Serializers;
using Coworkee.Domain.Contracts;
using Coworkee.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Coworkee.Infrastructure.Configurations
{
    //public class EntityExtendedAttributeConfiguration : IEntityTypeConfiguration<IEntityExtendedAttribute>
    //{
    //    public void Configure(EntityTypeBuilder<IEntityExtendedAttribute> builder)
    //    {
    //        // This Converter will perform the conversion to and from Json to the desired type
    //        builder
    //            .Property(e => e.Json)
    //            .HasJsonConversion(
    //                new SystemTextJsonSerializer(
    //                    new OptionsWrapper<SystemTextJsonOptions>(new SystemTextJsonOptions())));
    //    }
    //}
}