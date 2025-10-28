using System;
using Coworkee.Domain.Contracts;
using Coworkee.Domain.Entities.Catalog;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Coworkee.Server.OData;

internal class EdmBuilder
{
    public static IEdmModel GetEdmModel()
    {        
        ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
        builder.EntitySet<AuditableEntity<Guid>>("AuditableEntities");
        builder.EntitySet<Product>("Products");
        builder.EntitySet<Brand>("Brands");

        
        return builder.GetEdmModel();
    }
}