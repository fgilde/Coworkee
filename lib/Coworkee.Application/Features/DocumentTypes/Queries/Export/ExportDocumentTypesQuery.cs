using System;
using System.Collections.Generic;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Export;
using Coworkee.Application.Specifications.Base;
using Coworkee.Application.Specifications.Misc;
using Coworkee.Domain.Entities.Misc;
using Coworkee.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Features.DocumentTypes.Queries.Export
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