using System;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Export;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Catalog;
using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Brands.Queries.Export
{
    [CustomAuthorize(Policies = new[] { Permissions.Brands.Export })]
    public class ExportBrandsQuery : ExportQueryBase<int>
    {}

    internal class ExportBrandsQueryHandler : ExportQueryHandlerBase<ExportBrandsQuery, int, Brand>
    {
        public ExportBrandsQueryHandler(IUnitOfWork<int> unitOfWork, 
            IStringLocalizer<ExportBrandsQueryHandler> localizer, IServiceProvider serviceProvider)
            : base(unitOfWork, localizer, serviceProvider)
        { }

        protected override ISpecification<Brand> GetFilterSpecification(ExportBrandsQuery query)
        {
            return new BrandFilterSpecification(query.SearchString);
        }
    }
}
