using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Domain.Entities.Misc;
using lib.Coworkee.Shared.Constants.Permission;

namespace lib.Coworkee.Application.Features.Documents.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { CorePermissionProvider.Core.Documents.View })]
    public class GetDocumentByIdQuery : GetByIdQueryBase<int, DocumentDto>
    {
        public GetDocumentByIdQuery(int id) : base(id)
        { }
    }

    internal class GetDocumentByIdQueryHandler : GetByIdQueryHandlerBase<GetDocumentByIdQuery, int, DocumentDto, Document>
    {
        public GetDocumentByIdQueryHandler(IUnitOfWork<int> unitOfWork, IServiceProvider provider) : base(unitOfWork, provider)
        { }
    }
}