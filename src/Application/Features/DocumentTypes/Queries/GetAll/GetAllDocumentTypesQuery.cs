using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Domain.Entities.Misc;
using Coworkee.Shared.Constants.Permission;
using LazyCache;

namespace Coworkee.Application.Features.DocumentTypes.Queries.GetAll
{
    [CustomAuthorize(Policies = new[] { Permissions.DocumentTypes.View })]
    public class GetAllDocumentTypesQuery : GetAllQueryBase<DocumentTypeDto>
    { }

    internal class GetAllDocumentTypesQueryHandler : GetAllQueryHandlerBase<GetAllDocumentTypesQuery, int, DocumentTypeDto, DocumentType>
    {
        public GetAllDocumentTypesQueryHandler(IUnitOfWork<int> unitOfWork, IAppCache cache) : base(unitOfWork, cache)
        { }
    }
}