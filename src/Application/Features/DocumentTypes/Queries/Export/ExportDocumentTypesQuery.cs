using System;
using System.Collections.Generic;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Export;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Misc;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.DocumentTypes.Queries.Export
{
    [CustomAuthorize(Policies = new[] { Permissions.DocumentTypes.Export })]
    public class ExportDocumentTypesQuery : ExportQueryBase<int>
    {
        public ExportDocumentTypesQuery(int[] ids) : base(ids)
        { }

        public ExportDocumentTypesQuery(string searchString) : base(searchString)
        { }
    }

    internal class ExportDocumentTypesQueryHandler : ExportQueryHandlerBase<ExportDocumentTypesQuery, int, DocumentType>
    {
        public ExportDocumentTypesQueryHandler(IExcelService excelService, IUnitOfWork<int> unitOfWork, IStringLocalizer<ExportDocumentTypesQueryHandler> localizer)
            : base(excelService, unitOfWork, localizer)
        { }

        protected override Dictionary<string, Func<DocumentType, object>> PropertyMappers()
        {
            return new Dictionary<string, Func<DocumentType, object>>
            {
                {Localizer["Id"], item => item.Id},
                {Localizer["Name"], item => item.Name},
                {Localizer["Description"], item => item.Description},
            };
        }

        protected override ISpecification<DocumentType> GetFilterSpecification(ExportDocumentTypesQuery query)
        {
            return new DocumentTypeFilterSpecification(query.SearchString);
        }
    }

}