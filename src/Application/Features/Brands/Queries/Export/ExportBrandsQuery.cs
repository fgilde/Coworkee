using System;
using System.Collections.Generic;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Export;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Catalog;
using CleanArchitectureBase.Domain.Entities.Catalog;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Brands.Queries.Export
{
    public class ExportBrandsQuery : ExportQueryBase<int>
    {
        public ExportBrandsQuery(int[] ids) : base(ids)
        { }

        public ExportBrandsQuery(string searchString) : base(searchString)
        { }
    }

    internal class ExportBrandsQueryHandler : ExportQueryHandlerBase<ExportBrandsQuery, int, Brand>
    {
        public ExportBrandsQueryHandler(IExcelService excelService, IUnitOfWork<int> unitOfWork, IStringLocalizer<ExportBrandsQueryHandler> localizer)
            : base(excelService, unitOfWork, localizer)
        { }

        protected override Dictionary<string, Func<Brand, object>> PropertyMappers()
        {
            return new Dictionary<string, Func<Brand, object>>
            {
                {Localizer["Id"], item => item.Id},
                {Localizer["Name"], item => item.Name},
                {Localizer["Description"], item => item.Description},
                {Localizer["Tax"], item => item.Tax}
            };
        }

        protected override ISpecification<Brand> GetFilterSpecification(ExportBrandsQuery query)
        {
            return new BrandFilterSpecification(query.SearchString);
        }
    }
}
