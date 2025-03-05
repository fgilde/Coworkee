using System;
using LazyCache;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Domain.Contracts;
using Coworkee.Shared.Constants.Application;
using Microsoft.EntityFrameworkCore;
using Nextended.Core.Extensions;
using Coworkee.Application.Common.Extensions;
using Coworkee.Shared;

namespace Coworkee.Application.Features.Base.Queries
{
    public class GetAllQueryBase<TDto> : IRequest<IReadOnlyCollection<TDto>>
        where TDto : IDtoBase
    {
        public TransferableExpression<TDto> OdataFilterQuery { get; set; } = null;

        /// <summary>
        /// If true cache is cleared first
        /// </summary>
        public bool Force { get; set; } = false;
    }

    internal class GetAllQueryHandlerBase<TQuery, TEntityId, TDto, TEntity> : IRequestHandler<TQuery, IReadOnlyCollection<TDto>>
        where TQuery : GetAllQueryBase<TDto>
        where TEntity : class, IEntity<TEntityId>
        where TDto : class, IDtoBase
    {
        protected readonly IUnitOfWork<TEntityId> UnitOfWork;
        protected readonly IAppCache Cache;

        protected virtual string CacheKey(TQuery query) => ApplicationConstants.Cache.CacheKeyFor(typeof(TEntity), query.OdataFilterQuery);

        public GetAllQueryHandlerBase(IUnitOfWork<TEntityId> unitOfWork, IAppCache cache)
        {
            UnitOfWork = unitOfWork;
            Cache = cache;
        }

        protected virtual IQueryable<TEntity> Query(TQuery query, IQueryable<TEntity> entities)
        {
            var expression = ODataQueryOptionsExtensions.ParseExpression<TEntity>(query.OdataFilterQuery);
            return expression == null ? entities : entities.Where(expression);
        }

        protected virtual IQueryable<TEntity> Queryable => UnitOfWork.Repository<TEntity>().Entities;
        protected virtual TDto ToDto(TEntity res) => res?.MapTo<TDto>();

        public virtual async Task<IReadOnlyCollection<TDto>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = CacheKey(request);
            if (request.Force && !string.IsNullOrWhiteSpace(cacheKey))
                Cache.Remove(cacheKey);
            Task<List<TEntity>> GetAll() => Query(request, Queryable).ToListAsync(cancellationToken);
            var resultList = await (cacheKey.IsNullOrWhiteSpace() ? GetAll() : Cache.GetOrAddAsync(cacheKey, GetAll));
            return resultList.Select(ToDto).ToList().AsReadOnly();
        }
    }
}