using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Base.Commands
{
    public abstract class DeleteCommandBase<TId> : IRequest
    {
        public TId[] Ids { get; set; }
    }

    internal class DeleteCommandHandlerBase<TCommand, TEntityId, TEntity> : IRequestHandler<TCommand>
        where TEntity : AuditableEntity<TEntityId>
        where TCommand : DeleteCommandBase<TEntityId>
    {
        protected readonly IUnitOfWork<TEntityId> UnitOfWork;
        protected readonly IPermissionService PermissionService;
        protected readonly IMediator Mediator;
        protected readonly IServiceProvider Provider;
        protected T Get<T>() => Provider.GetService<T>();
        protected virtual string CacheKey => null;
        public DeleteCommandHandlerBase(IUnitOfWork<TEntityId> unitOfWork,
            IMediator mediator,
            IPermissionService permissionService, IServiceProvider provider)
        {
            UnitOfWork = unitOfWork;
            PermissionService = permissionService;
            Provider = provider;
            Mediator = mediator;
        }

        public async Task<Unit> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var entities = await UnitOfWork.Repository<TEntity>().GetByIdsAsync(command.Ids, cancellationToken);
            await UnitOfWork.Repository<TEntity>().DeleteManyAsync(entities, cancellationToken);
            await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken) : UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));

            return Unit.Value;
        }
    }
}