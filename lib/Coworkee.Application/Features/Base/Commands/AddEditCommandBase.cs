using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Extensions;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Hubs.Events;
using lib.Coworkee.Domain.Contracts;
using lib.Coworkee.Shared.Constants.Application;
using lib.Coworkee.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace lib.Coworkee.Application.Features.Base.Commands;

public abstract class AddEditCommandBase<TDto> : IRequest<AddUpdateResult<TDto>>
    where TDto : IDtoBase
{
    protected AddEditCommandBase(params TDto[] items)
    {
        Items = items;
    }

    public bool CreateNewIfToUpdateNotExists { get; set; } = false;
    public TDto[] Items { get; set; }
}

internal class AddEditCommandHandlerBase<TCommand, TEntityId, TDto, TEntity> : IRequestHandler<TCommand, AddUpdateResult<TDto>>
    where TCommand : AddEditCommandBase<TDto>
    where TEntity : class, IEntity<TEntityId>
    where TDto : IDtoBase<TEntityId>
{

    protected readonly IUnitOfWork<TEntityId> UnitOfWork;
    protected readonly IPermissionService PermissionService;
    protected readonly IMediator Mediator;
    protected readonly IServiceProvider Provider;
    protected T Get<T>() => Provider.GetService<T>();
    protected virtual IEnumerable<string> CacheKeys(TCommand command) => new[] { ApplicationConstants.Cache.CacheKeyFor(typeof(TEntity)) };
    protected virtual string EditPermission => null;
    protected virtual string CreatePermission => null;
    protected virtual IQueryable<TEntity> Queryable => UnitOfWork.Repository<TEntity>().Entities;
    protected virtual Expression<Func<TEntity, object>>[] Includes => null;

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

    public virtual async Task<AddUpdateResult<TDto>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var cacheKeys = CacheKeys(command).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        var user = Get<ICurrentUserService>().CurrentUser();
        var all = command.Items.Select(dto => GetPreparedEntity(dto, command.CreateNewIfToUpdateNotExists)).ToLookup(t => t.IsNew);
        var toCreate = all[true].ToArray();
        var toUpdate = all[false].Where(t => t.Exists).ToArray();

        TDto[] created = Array.Empty<TDto>();
        TDto[] updated = Array.Empty<TDto>();

        var repository = UnitOfWork.Repository<TEntity>();

        if (toCreate.Any())
        {
            await PermissionService.EnsurePolicyAsync(CreatePermission);
            var entitiesToCreate = toCreate.Select(t => t.Entity).ToArray();
            await repository.AddManyAsync(entitiesToCreate, cancellationToken);
            await (cacheKeys.All(string.IsNullOrWhiteSpace) ? UnitOfWork.Commit(cancellationToken) : UnitOfWork.CommitAndRemoveCache(cancellationToken, cacheKeys));
            for (var index = 0; index < entitiesToCreate.Length; index++)
            {
                var entity = entitiesToCreate[index];
                toCreate[index].Dto.Id = entity.Id;
            }

            _ = Mediator.PublishClientEvents(cancellationToken,
                new EntitiesCreated<TDto>(user, created = toCreate.Select(t => t.Dto).ToArray()),
                new EntitiesCreated(user, entitiesToCreate.Select(e => e.Id?.ToString()))
            );
        }
        if (toUpdate.Any())
        {
            await PermissionService.EnsurePolicyAsync(EditPermission);
            await repository.UpdateManyAsync(toUpdate.Select(t => t.Entity), cancellationToken);
            await (cacheKeys.All(string.IsNullOrWhiteSpace) ? UnitOfWork.Commit(cancellationToken) : UnitOfWork.CommitAndRemoveCache(cancellationToken, cacheKeys));
            _ = Mediator.PublishClientEvents(cancellationToken,
                new EntitiesChanged<TDto>(user, updated = toUpdate.Select(t => t.Dto).ToArray()),
                new EntitiesChanged(user, updated.Select(d => d.Id?.ToString()))
            );
        }

        var createdAndUpdated = created.Concat(updated).ToArray();
        var skipped = command.Items.Where(dto => !created.Contains(dto) && !updated.Contains(dto)).ToArray();
        _ = Mediator.PublishClientEvents(cancellationToken,
            new EntitiesUpdated<TDto>(user, createdAndUpdated),
            new EntitiesUpdated(user, createdAndUpdated.Select(d => d.Id?.ToString()))
        );
        return new AddUpdateResult<TDto>(created, updated, skipped);
    }

    protected virtual TEntity ToEntity(TDto dto) => dto.MapTo<TEntity>();

    private (bool IsNew, bool Exists, TEntity Entity, TDto Dto) GetPreparedEntity(TDto dto, bool createNewIfToUpdateNotExists)
    {
        var mappedEntity = ToEntity(dto);
        var entity = dto.IsNew ? mappedEntity : GetById(dto.Id);
        var exists = !dto.IsNew && !Equals(entity, default(TEntity));
        var isNew = createNewIfToUpdateNotExists ? dto.IsNew || !exists : dto.IsNew;
        mappedEntity.SetProperties(e => e.Id = isNew ? default : e.Id);
        return (isNew, exists, isNew || !exists ? mappedEntity : mappedEntity.CopyChangedValuesTo(entity), dto);
    }

    protected virtual TEntity GetById(TEntityId id)
    {
        var query = Queryable;
        return Includes?.Any() == true
            ? Includes.Aggregate(query, (current, include) => current.Include(include)).FirstOrDefault(e => e.Id.Equals(id))
            : UnitOfWork.Repository<TEntity>().GetById(id);
    }
}