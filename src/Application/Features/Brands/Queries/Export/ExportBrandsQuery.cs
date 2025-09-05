using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Export;
using lib.Coworkee.Application.Specifications.Base;
using Coworkee.Application.Specifications.Catalog;
using Coworkee.Domain.Entities.Catalog;
using lib.Coworkee.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Features.Brands.Queries.Export
{
    [CustomAuthorize(Policies = new[] { Permissions.Brands.Export })]
    public class ExportBrandsQuery : ExportQueryHashed
    { }

    internal class ExportBrandsQueryHandler : ExportQueryHandlerBase<ExportBrandsQuery, int, BrandDto, Brand>
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
