using CleanArchitectureBase.Domain.Entities.Catalog;
using System;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Export;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Catalog;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Products.Queries.Export
{
    [CustomAuthorize(Policies = new[] { Permissions.Products.Export })]
    public class ExportProductsQuery: ExportQueryBase<int>
    {}

    internal class ExportProductsQueryHandler: ExportQueryHandlerBase<ExportProductsQuery, int, Product> {

        public ExportProductsQueryHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<ExportProductsQueryHandler> localizer, IServiceProvider serviceProvider) 
            : base(unitOfWork, localizer, serviceProvider)
        {}

        protected override ISpecification<Product> GetFilterSpecification(ExportProductsQuery query)
        {
            return new ProductFilterSpecification(query.SearchString);
        }
    }
}