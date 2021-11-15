using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Hubs.Events;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Shared.Constants.Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Base.Commands
{
    public abstract class DeleteCommandBase<TId> : IRequest
    {
        public TId[] Ids { get; set; }
    }

    internal class DeleteCommandHandlerBase<TCommand, TEntityId, TDto, TEntity> : IRequestHandler<TCommand>
        where TEntity : AuditableEntity<TEntityId>
        where TDto : IDtoBase<TEntityId>
        where TCommand : DeleteCommandBase<TEntityId>
    {
        protected readonly IUnitOfWork<TEntityId> UnitOfWork;
        protected readonly IPermissionService PermissionService;
        protected readonly IMediator Mediator;
        protected readonly IServiceProvider Provider;
        protected T Get<T>() => Provider.GetService<T>();
        protected virtual string CacheKey => ApplicationConstants.Cache.CacheKeyFor(typeof(TEntity));
        public DeleteCommandHandlerBase(IUnitOfWork<TEntityId> unitOfWork,
            IMediator mediator,
            IPermissionService permissionService, IServiceProvider provider)
        {
            UnitOfWork = unitOfWork;
            PermissionService = permissionService;
            Provider = provider;
            Mediator = mediator;
        }

        public virtual async Task<Unit> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var user = Get<ICurrentUserService>().CurrentUser();
            var entities = (await UnitOfWork.Repository<TEntity>().GetByIdsAsync(command.Ids, cancellationToken)).ToArray();
            await UnitOfWork.Repository<TEntity>().DeleteManyAsync(entities, cancellationToken);
            await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken) : UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));
            var deletedItemsAsDto = entities.MapElementsTo<TDto>().ToArray();

            await Mediator.PublishClientEvents(cancellationToken, 
                new EntitiesDeleted<TDto>(user, deletedItemsAsDto),
                new EntitiesUpdated<TDto>(user, deletedItemsAsDto));

            return Unit.Value;
        }
    }
}