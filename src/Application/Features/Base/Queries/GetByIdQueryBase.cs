using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Base.Queries
{
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
        where TEntity : AuditableEntity<TEntityId>
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

        public virtual async Task<TDto> Handle(TQuery request, CancellationToken cancellationToken)
        {
            var result = await UnitOfWork.Repository<TEntity>().GetByIdAsync(request.Id, cancellationToken);
            return result?.MapTo<TDto>();
        }
    }
}