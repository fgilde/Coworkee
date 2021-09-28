using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Wrapper;
using LazyCache;
using MediatR;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetAll
{
    public class GetAllDocumentTypesQuery : IRequest<Result<List<GetAllDocumentTypesResponse>>>
    {
        public GetAllDocumentTypesQuery()
        {
        }
    }

    internal class GetAllDocumentTypesQueryHandler : IRequestHandler<GetAllDocumentTypesQuery, Result<List<GetAllDocumentTypesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IAppCache _cache;

        public GetAllDocumentTypesQueryHandler(IUnitOfWork<int> unitOfWork, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<List<GetAllDocumentTypesResponse>>> Handle(GetAllDocumentTypesQuery request, CancellationToken cancellationToken)
        {
            Task<List<DocumentType>> GetAllDocumentTypes() => _unitOfWork.Repository<DocumentType>().GetAllAsync();
            var documentTypeList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllDocumentTypesCacheKey, GetAllDocumentTypes);
            var mappedDocumentTypes = documentTypeList.MapTo<List<GetAllDocumentTypesResponse>>();
            return await Result<List<GetAllDocumentTypesResponse>>.SuccessAsync(mappedDocumentTypes);
        }
    }
}