using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Constants.Permission;
using LazyCache;

namespace CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetAll
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