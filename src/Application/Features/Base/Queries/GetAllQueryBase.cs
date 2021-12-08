using System;
using LazyCache;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.EntityFrameworkCore;
using Nextended.Core.Extensions;
using CleanArchitectureBase.Application.Common.Extensions;

namespace CleanArchitectureBase.Application.Features.Base.Queries
{
    public class GetAllQueryBase<TDto> : IRequest<IReadOnlyCollection<TDto>>
        where TDto : IDtoBase
    {
        public string OdataFilterQuery { get; set; } = null;

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

        protected virtual string CacheKey(TQuery query) => ApplicationConstants.Cache.CacheKeyFor(typeof(TEntity), query.OdataFilterQuery);

        public GetAllQueryHandlerBase(IUnitOfWork<TEntityId> unitOfWork, IAppCache cache)
        {
            UnitOfWork = unitOfWork;
            Cache = cache;
        }

        protected virtual IQueryable<TEntity> Query(TQuery query)
        {
            var expression = ODataQueryOptionsExtensions.ParseExpression<TEntity>(query.OdataFilterQuery);
            return expression == null ? Queryable : Queryable.Where(expression);
        }

        protected virtual IQueryable<TEntity> Queryable => UnitOfWork.Repository<TEntity>().Entities;

        public virtual async Task<IReadOnlyCollection<TDto>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = CacheKey(request);
            if(request.Force && !string.IsNullOrWhiteSpace(cacheKey))
                Cache.Remove(cacheKey);
            Task<List<TEntity>> GetAll() => Query(request).ToListAsync(cancellationToken);
            var resultList = await (cacheKey.IsNullOrWhiteSpace() ? GetAll() : Cache.GetOrAddAsync(cacheKey, GetAll));
            return resultList.MapTo<List<TDto>>().AsReadOnly();
        }
    }
}