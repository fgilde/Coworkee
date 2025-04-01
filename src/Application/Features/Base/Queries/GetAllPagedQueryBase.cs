using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Specifications.Base;
using Coworkee.Domain.Contracts;
using Coworkee.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;
using Coworkee.Shared;

namespace Coworkee.Application.Features.Base.Queries
{
    public class GetAllPagedQueryBase<TDto> : IRequest<PaginatedResult<TDto>>
        where TDto : IDtoBase
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; }
        public TransferableExpression<TDto> OdataFilterQuery { get; set; } = null;

        public GetAllPagedQueryBase<TDto> SortBy(Expression<Func<TDto, object>> expression, string direction = "")
        {
            return SortBy(expression.GetMemberName(), direction);
        }

        public GetAllPagedQueryBase<TDto> SortBy(string propertyName, string direction = "")
        {
            var sorts = (OrderBy ?? Array.Empty<string>()).ToList();
            sorts.Add($"{propertyName} {direction}");
            OrderBy = sorts.ToArray();
            return this;
        }

        public GetAllPagedQueryBase()
        { }

        public GetAllPagedQueryBase(int pageNumber, int pageSize, string searchString, string orderBy = "")
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllPagedQueryHandlerBase<TQuery, TEntityId, TDto, TEntity> : IRequestHandler<TQuery, PaginatedResult<TDto>>
        where TQuery : GetAllPagedQueryBase<TDto>
        where TEntity : class, IEntity<TEntityId>
        where TDto : class, IDtoBase
    {
        protected readonly IUnitOfWork<TEntityId> UnitOfWork;
        protected readonly IMediator Mediator;
        protected readonly IServiceProvider Provider;
        protected T Get<T>() => Provider.GetService<T>();

        public GetAllPagedQueryHandlerBase(
            IUnitOfWork<TEntityId> unitOfWork,
            IMediator mediator,
            IServiceProvider provider)
        {
            UnitOfWork = unitOfWork;
            Mediator = mediator;
            Provider = provider;
        }

        protected virtual ISpecification<TEntity> GetFilterSpecification(TQuery query)
        {
            return null;
        }

        protected virtual IQueryable<TEntity> Query(TQuery query, IQueryable<TEntity> entities)
        {
            var expression = ODataQueryOptionsExtensions.ParseExpression<TEntity>(query.OdataFilterQuery);
            return expression == null ? entities : entities.Where(expression);
        }

        protected virtual IQueryable<TEntity> Queryable => UnitOfWork.Repository<TEntity>().Entities;

        public virtual async Task<PaginatedResult<TDto>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<TEntity, TDto>> expression = e => e.MapTo<TDto>();
            var filterSpec = GetFilterSpecification(request);

            var orderBy = request.OrderBy?.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (orderBy?.Any() != true)
            {
                var data = await Query(request, Queryable
                   .Specify(filterSpec))
                   .Select(expression)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", orderBy); // of the form fieldname [ascending|descending], ...
                var data = await Query(request, Queryable
                   .Specify(filterSpec))
                   .OrderBy(ordering) // require system.linq.dynamic.core
                   .Select(expression)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;

            }
        }
    }
}