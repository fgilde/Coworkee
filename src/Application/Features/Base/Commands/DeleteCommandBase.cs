using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Hubs.Events;
using Coworkee.Domain.Contracts;
using Coworkee.Shared.Constants.Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace Coworkee.Application.Features.Base.Commands
{
    public abstract class DeleteCommandBase<TId> : IRequest
    {
        public TId[] Ids { get; set; }
    }

    internal class DeleteCommandHandlerBase<TCommand, TEntityId, TDto, TEntity> : IRequestHandler<TCommand>
        where TEntity : AuditableEntity<TEntityId>
        where TDto : IDtoBase
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

        protected virtual async Task<IEnumerable<TEntity>> FindEntitiesAsync(TCommand command, CancellationToken cancellationToken)
        {
            return (await UnitOfWork.Repository<TEntity>().GetByIdsAsync(command.Ids, cancellationToken));
        }

        public virtual async Task Handle(TCommand command, CancellationToken cancellationToken)
        {
            var user = Get<ICurrentUserService>().CurrentUser();
            var entities = (await FindEntitiesAsync(command, cancellationToken)).ToArray();
            await UnitOfWork.Repository<TEntity>().DeleteManyAsync(entities, cancellationToken);
            await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken) : UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));
            var deletedItemsAsDto = entities.MapElementsTo<TDto>().ToArray();

            //var ids = command.Ids.Select(id => id.ToString()).ToArray();
            var ids = entities.Select(e => e.Id.ToString()).ToArray();
            await Mediator.PublishClientEvents(cancellationToken, 
                new EntitiesDeleted<TDto>(user, deletedItemsAsDto),
                new EntitiesUpdated<TDto>(user, deletedItemsAsDto),
                new EntitiesDeleted(user, ids),
                new EntitiesUpdated(user, ids));
        }
    }
}