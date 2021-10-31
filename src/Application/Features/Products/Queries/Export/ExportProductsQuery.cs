using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using CleanArchitectureBase.Application.Features.Base.Export;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Catalog;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Products.Queries.Export
{
    public class ExportProductsQuery: ExportQueryBase<int>
    {
        public ExportProductsQuery(int[] ids) : base(ids)
        {}

        public ExportProductsQuery(string searchString) : base(searchString)
        {}
    }

    internal class ExportProductsQueryHandler: ExportQueryHandlerBase<ExportProductsQuery, int, Product> {

        public ExportProductsQueryHandler(IExcelService excelService, IUnitOfWork<int> unitOfWork, IStringLocalizer<ExportProductsQueryHandler> localizer) 
            : base(excelService, unitOfWork, localizer)
        {}

        protected override Dictionary<string, Func<Product, object>> PropertyMappers()
        {
            return new Dictionary<string, Func<Product, object>>
            {
                {Localizer["Id"], item => item.Id},
                {Localizer["Name"], item => item.Name},
                {Localizer["Barcode"], item => item.Barcode},
                {Localizer["Description"], item => item.Description},
                {Localizer["Rate"], item => item.Rate}
            };
        }

        protected override ISpecification<Product> GetFilterSpecification(ExportProductsQuery query)
        {
            return new ProductFilterSpecification(query.SearchString);
        }
    }
}