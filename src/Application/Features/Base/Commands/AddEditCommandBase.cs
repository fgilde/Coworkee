using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Extensions;
using CleanArchitectureBase.Application.Hubs.Events;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;
using Nextended.Core.Helper;

namespace CleanArchitectureBase.Application.Features.Base.Commands
{
    public abstract class AddEditCommandBase<TDto> : IRequest
        where TDto : IDtoBase
    {
        protected AddEditCommandBase(params TDto[] items)
        {
            Items = items;
        }

        public TDto[] Items { get; set; }
    }

    internal class AddEditCommandHandlerBase<TCommand, TEntityId, TDto, TEntity> : IRequestHandler<TCommand>
        where TCommand: AddEditCommandBase<TDto>
        where TEntity : AuditableEntity<TEntityId>
        where TDto : IDtoBase<TEntityId>
    {
        
        protected readonly IUnitOfWork<TEntityId> UnitOfWork;
        protected readonly IPermissionService PermissionService;
        protected readonly IMediator Mediator;
        protected readonly IServiceProvider Provider;
        protected T Get<T>() => Provider.GetService<T>();
        protected virtual string CacheKey => ApplicationConstants.Cache.CacheKeyFor(typeof(TEntity));
        protected virtual string EditPermission => null;
        protected virtual string CreatePermission => null;


        public AddEditCommandHandlerBase(IUnitOfWork<TEntityId> unitOfWork,
            IMediator mediator,
            IPermissionService permissionService,
            IServiceProvider provider)
        {
            Provider = provider;
            UnitOfWork = unitOfWork;
            PermissionService = permissionService;
            Mediator = mediator;
        }
        
        public virtual async Task<Unit> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var user = Get<ICurrentUserService>().CurrentUser();
            var toCreate = command.Items.Where(t => t.IsNew).ToArray();
            var toUpdate = command.Items.Where(t => !t.IsNew).ToArray();

            var repository = UnitOfWork.Repository<TEntity>();
            if (toCreate.Any())
            {
                if (CreatePermission != null)
                    await PermissionService.EnsurePolicyAsync(CreatePermission);
                await repository.AddManyAsync(toCreate.MapElementsTo<TEntity>(), cancellationToken);
                await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken): UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));
                await Mediator.PublishClientEvent(new EntitiesCreated<TDto>(user, command.Items), cancellationToken);
            }
            if (toUpdate.Any())
            {
                if (EditPermission != null)
                    await PermissionService.EnsurePolicyAsync(EditPermission);

                var entitiesToUpdate = (await repository.GetByIdsAsync(toUpdate.Select(dto => dto.Id), cancellationToken))
                    .Select(e => toUpdate.First(dto => Equals(dto.Id, e.Id)).MapTo<TEntity>().CopyChangedValuesTo(e)).ToArray();
                
                await repository.UpdateManyAsync(entitiesToUpdate, cancellationToken);
                await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken) : UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));
                await Mediator.PublishClientEvent(new EntitiesChanged<TDto>(user, command.Items), cancellationToken);

            }

            
            await Mediator.PublishClientEvent(new EntitiesUpdated<TDto>(user, command.Items), cancellationToken);
            return Unit.Value;
        }
    }

}