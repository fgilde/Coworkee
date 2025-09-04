using Coworkee.Domain.Contracts;
using Coworkee.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Exceptions;
using Coworkee.Application.Contracts.Repositories;

namespace Coworkee.Infrastructure.Repositories;

public class RepositoryAsync<T, TId> : IRepositoryAsync<T, TId> where T : class, IEntity<TId>
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryAsync(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private DbContext Context => _dbContext;

    public IQueryable<T> Entities => Context.Set<T>();

    // this.context.Set<T>().AsQueryable().AsNoTracking();
    public async Task<T> AddAsync(T entity, CancellationToken cancellation = default)
    {
        await Context.Set<T>().AddAsync(entity, cancellation);
        return entity;
    }

    public Task AddManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
    {
        return Context.Set<T>().AddRangeAsync(entities, cancellation);
    }

    public Task AddManyAsync(params T[] entities)
    {
        return Context.Set<T>().AddRangeAsync(entities);
    }

    public async Task UpdateAsync(TId id, object values, CancellationToken cancellation = default)
    {
        var existingEntity = await GetByIdAsync(id, cancellation);
        if (existingEntity == null)
            throw new NotFoundException(typeof(T).Name, id);

        Context.Entry(existingEntity).CurrentValues.SetValues(values);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellation = default)
    {
        Context.Set<T>().Update(entity);
        return Task.CompletedTask;
        //return UpdateAsync(entity.Id, entity);
    }

    public Task UpdateManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
    {
        Context.Set<T>().UpdateRange(entities);
        return Task.CompletedTask;
    }
    public Task UpdateManyAsync(params T[] entities)
    {
        Context.Set<T>().UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellation = default)
    {
        Context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
    {
        Context.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteManyAsync(params T[] entities)
    {
        Context.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteAllAsync()
    {
        Context.RemoveRange(Entities);
        return Task.CompletedTask;
    }

    public T GetById(TId id)
    {
        return Context.Set<T>().Find(id);
    }

    public Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellation = default)
    {
        return Task.FromResult(Context.Set<T>().Where(e => ids.Contains(e.Id)).AsEnumerable());
        //return await Task.WhenAll(ids.Select(id => GetByIdAsync(id, cancellation)));
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellation = default)
    {
        return await Context
            .Set<T>()
            .ToListAsync(cancellation);
    }

    public async Task<T> GetByIdAsync(TId id, CancellationToken cancellation = default)
    {
        return await Context.Set<T>().FindAsync(new object[] { id }, cancellation);
    }

    public IEnumerable<T> GetByIds(IEnumerable<TId> ids)
    {
        return Context.Set<T>().Where(e => ids.Contains(e.Id));
    }

    public async Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize, CancellationToken cancellation = default)
    {
        return await Context
            .Set<T>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellation);
    }
}