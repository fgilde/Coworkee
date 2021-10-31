using CleanArchitectureBase.Application.Interfaces.Repositories;
using LazyCache;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Domain.Contracts;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Base.Queries
{
    public class GetAllQueryBase<TDto> : IRequest<IReadOnlyCollection<TDto>>
        where TDto : IDtoBase
    {}

    internal class GetAllQueryHandlerBase<TQuery, TEntityId, TDto, TEntity> : IRequestHandler<TQuery, IReadOnlyCollection<TDto>>
        where TQuery : GetAllQueryBase<TDto>
        where TEntity : AuditableEntity<TEntityId>
        where TDto : class, IDtoBase
    {
        private readonly IUnitOfWork<TEntityId> _unitOfWork;
        private readonly IAppCache _cache;

        protected virtual string CacheKey => null;

        public GetAllQueryHandlerBase(IUnitOfWork<TEntityId> unitOfWork, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<IReadOnlyCollection<TDto>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            Task<List<TEntity>> GetAll() => _unitOfWork.Repository<TEntity>().GetAllAsync(cancellationToken);
            var resultList = await (CacheKey.IsNullOrWhiteSpace() ? GetAll() : _cache.GetOrAddAsync(CacheKey, GetAll));
            return resultList.MapTo<List<TDto>>().AsReadOnly();
        }
    }
}