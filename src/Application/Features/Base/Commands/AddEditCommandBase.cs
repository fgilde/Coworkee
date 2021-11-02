using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Shared.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Base.Commands
{
    public abstract class AddEditCommandBase<TDto> : IRequest
        where TDto : IDtoBase
    {
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
        protected virtual string CacheKey => null;

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
            var toCreate = command.Items.Where(t => t.IsNew).ToArray();
            var toUpdate = command.Items.Where(t => !t.IsNew).ToArray();

            var repository = UnitOfWork.Repository<TEntity>();
            if (toCreate.Any())
            {
                // await _permissionService.EnsurePolicyAsync(Permissions.Products.Create);
                await repository.AddManyAsync(toCreate.MapElementsTo<TEntity>(), cancellationToken);
                await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken): UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));
            }
            if (toUpdate.Any())
            {
                //await _permissionService.EnsurePolicyAsync(Permissions.Products.Edit);

                // TODO: Ugly currently but To ensure correct audit trail we need to load entities to change and update only changed properties. So toUpdate.MapElementsTo<TEntity>(); is badly not enough but working in general
                var entitiesToUpdate = (await repository.GetByIdsAsync(toUpdate.Select(dto => dto.Id), cancellationToken))
                    .Select(e => toUpdate.First(dto => Equals(dto.Id, e.Id)).MapTo<TEntity>().CopyChangedValuesTo(e)).ToArray();
                
                await repository.UpdateManyAsync(entitiesToUpdate, cancellationToken);
                await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken) : UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));

            }
            return Unit.Value;
        }
    }

}