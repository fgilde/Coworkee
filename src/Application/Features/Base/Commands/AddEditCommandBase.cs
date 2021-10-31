using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Domain.Contracts;
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
        where TDto : IDtoBase
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

            if (toCreate.Any())
            {
                // await _permissionService.EnsurePolicyAsync(Permissions.Products.Create);
                await UnitOfWork.Repository<TEntity>().AddManyAsync(toCreate.MapElementsTo<TEntity>(), cancellationToken);
                await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken): UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));
            }
            if (toUpdate.Any())
            {
                //await _permissionService.EnsurePolicyAsync(Permissions.Products.Edit);
                await UnitOfWork.Repository<TEntity>().UpdateManyAsync(toUpdate.MapElementsTo<TEntity>(), cancellationToken);
                await (CacheKey.IsNullOrWhiteSpace() ? UnitOfWork.Commit(cancellationToken) : UnitOfWork.CommitAndRemoveCache(cancellationToken, CacheKey));

            }
            return Unit.Value;
        }
    }

}