using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Domain.Entities.Misc;
using Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.DocumentTypes.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { Permissions.DocumentTypes.View })]
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