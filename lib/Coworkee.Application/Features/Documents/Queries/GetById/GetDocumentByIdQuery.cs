using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Domain.Entities.Misc;
using Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Documents.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { Permissions.Documents.View })]
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