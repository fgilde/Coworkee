using System;
using System.Collections.Generic;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Export;
using lib.Coworkee.Application.Specifications.Base;
using lib.Coworkee.Application.Specifications.Misc;
using lib.Coworkee.Domain.Entities.Misc;
using lib.Coworkee.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace lib.Coworkee.Application.Features.DocumentTypes.Queries.Export
{
    [CustomAuthorize(Policies = new[] { CorePermissionProvider.Core.DocumentTypes.Export })]
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