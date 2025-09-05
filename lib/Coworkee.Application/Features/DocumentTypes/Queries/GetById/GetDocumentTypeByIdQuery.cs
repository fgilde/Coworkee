using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Domain.Entities.Misc;
using lib.Coworkee.Shared.Constants.Permission;

namespace lib.Coworkee.Application.Features.DocumentTypes.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { CorePermissionProvider.Core.DocumentTypes.View })]
    public class GetDocumentTypeByIdQuery : GetByIdQueryBase<int, DocumentTypeDto>
    {
        public GetDocumentTypeByIdQuery(int id) : base(id)
        { }
    }

    internal class GetDocumentTypeByIdQueryHandler : GetByIdQueryHandlerBase<GetDocumentTypeByIdQuery, int, DocumentTypeDto, Document>
    {
        public GetDocumentTypeByIdQueryHandler(IUnitOfWork<int> unitOfWork, IServiceProvider provider) : base(unitOfWork, provider)
        { }
    }
}