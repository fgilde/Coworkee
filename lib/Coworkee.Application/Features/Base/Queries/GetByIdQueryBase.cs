using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace Coworkee.Application.Features.Base.Queries;

public class GetByIdQueryBase<TId, TDto> : IRequest<TDto>
{
    public TId Id { get; set; }

    public GetByIdQueryBase(TId id)
    {
        Id = id;
    }
}

internal class GetByIdQueryHandlerBase<TQuery, TEntityId, TDto, TEntity> : IRequestHandler<TQuery, TDto>
    where TQuery : GetByIdQueryBase<TEntityId, TDto>
    where TEntity : class, IEntity<TEntityId>
    where TDto : class, IDtoBase
{
    protected readonly IUnitOfWork<TEntityId> UnitOfWork;
    protected readonly IServiceProvider Provider;
    protected T Get<T>() => Provider.GetService<T>();

    public GetByIdQueryHandlerBase(IUnitOfWork<TEntityId> unitOfWork, IServiceProvider provider)
    {
        UnitOfWork = unitOfWork;
        Provider = provider;
    }

    protected virtual TDto ToDto(TEntity res) => res?.MapTo<TDto>();
    protected virtual IQueryable<TEntity> Queryable => UnitOfWork.Repository<TEntity>().Entities;
    protected virtual Expression<Func<TEntity, object>>[] Includes => null;

    public virtual async Task<TDto> Handle(TQuery request, CancellationToken cancellationToken)
    {
        var query = Queryable;

        TEntity res = Includes?.Any() == true
            ? await Includes.Aggregate(query, (current, include) => current.Include(include)).FirstOrDefaultAsync(e => e.Id.Equals(request.Id), cancellationToken: cancellationToken)
            : await UnitOfWork.Repository<TEntity>().GetByIdAsync(request.Id, cancellationToken);

        return ToDto(res);
    }
}