using Coworkee.Domain.Entities.Catalog;
using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Export;
using Coworkee.Application.Specifications.Base;
using Coworkee.Application.Specifications.Catalog;
using Coworkee.Shared.Constants.Permission;
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