using Coworkee.Domain.Entities.Catalog;
using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Export;
using lib.Coworkee.Application.Specifications.Base;
using Coworkee.Application.Specifications.Catalog;
using lib.Coworkee.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Features.Products.Queries.Export
{
    [CustomAuthorize(Policies = new[] { Permissions.Products.Export })]
    public class ExportProductsQuery: ExportQueryHashed
    { }

    internal class ExportProductsQueryHandler: ExportQueryHandlerBase<ExportProductsQuery, int, ProductDto, Product> {

        public ExportProductsQueryHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<ExportProductsQueryHandler> localizer, IServiceProvider serviceProvider) 
            : base(unitOfWork, localizer, serviceProvider)
        {}

        protected override ISpecification<Product> GetFilterSpecification(ExportProductsQuery query)
        {
            return new ProductFilterSpecification(query.SearchString);
        }
    }
}