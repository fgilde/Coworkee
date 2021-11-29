using System;
using System.Collections.Generic;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
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
    {}

    internal class ExportDocumentTypesQueryHandler : ExportQueryHandlerBase<ExportDocumentTypesQuery, int, DocumentTypeDto, DocumentType>
    {
        public ExportDocumentTypesQueryHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<ExportDocumentTypesQueryHandler> localizer, IServiceProvider serviceProvider)
            : base(unitOfWork, localizer, serviceProvider)
        { }

        protected override ISpecification<DocumentType> GetFilterSpecification(ExportDocumentTypesQuery query)
        {
            return new DocumentTypeFilterSpecification(query.SearchString);
        }
    }
}