using CleanArchitectureBase.Application.Interfaces.Repositories;
using LazyCache;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.EntityFrameworkCore;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Base.Queries
{
    public class GetAllQueryBase<TDto> : IRequest<IReadOnlyCollection<TDto>>
        where TDto : IDtoBase
    {
        /// <summary>
        /// If true cache is cleared first
        /// </summary>
        public bool Force { get; set; } = false;
    }

    internal class GetAllQueryHandlerBase<TQuery, TEntityId, TDto, TEntity> : IRequestHandler<TQuery, IReadOnlyCollection<TDto>>
        where TQuery : GetAllQueryBase<TDto>
        where TEntity : AuditableEntity<TEntityId>
        where TDto : class, IDtoBase
    {
        protected readonly IUnitOfWork<TEntityId> UnitOfWork;
        protected readonly IAppCache Cache;

        protected virtual string CacheKey => ApplicationConstants.Cache.CacheKeyFor(typeof(TEntity));

        public GetAllQueryHandlerBase(IUnitOfWork<TEntityId> unitOfWork, IAppCache cache)
        {
            UnitOfWork = unitOfWork;
            Cache = cache;
        }

        protected virtual IQueryable<TEntity> Queryable => UnitOfWork.Repository<TEntity>().Entities;

        public virtual async Task<IReadOnlyCollection<TDto>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            if(request.Force && !string.IsNullOrWhiteSpace(CacheKey))
                Cache.Remove(CacheKey);
            Task<List<TEntity>> GetAll() => Queryable.ToListAsync(cancellationToken);
            var resultList = await (CacheKey.IsNullOrWhiteSpace() ? GetAll() : Cache.GetOrAddAsync(CacheKey, GetAll));
            return resultList.MapTo<List<TDto>>().AsReadOnly();
        }
    }
}