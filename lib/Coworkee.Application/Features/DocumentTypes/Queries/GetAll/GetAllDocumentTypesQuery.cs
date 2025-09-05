using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Domain.Entities.Misc;
using lib.Coworkee.Shared.Constants.Permission;
using LazyCache;

namespace lib.Coworkee.Application.Features.DocumentTypes.Queries.GetAll
{
    [CustomAuthorize(Policies = new[] { CorePermissionProvider.Core.DocumentTypes.View })]
    public class GetAllDocumentTypesQuery : GetAllQueryBase<DocumentTypeDto>
    { }

    internal class GetAllDocumentTypesQueryHandler : GetAllQueryHandlerBase<GetAllDocumentTypesQuery, int, DocumentTypeDto, DocumentType>
    {
        public GetAllDocumentTypesQueryHandler(IUnitOfWork<int> unitOfWork, IAppCache cache) : base(unitOfWork, cache)
        { }
    }
}