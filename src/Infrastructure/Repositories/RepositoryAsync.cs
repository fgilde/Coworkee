using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Exceptions;
using CleanArchitectureBase.Application.Contracts.Repositories;

namespace CleanArchitectureBase.Infrastructure.Repositories
{
    public class RepositoryAsync<T, TId> : IRepositoryAsync<T, TId> where T : AuditableEntity<TId>
    {
        private readonly ApplicationDbContext _dbContext;

        public RepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Entities => _dbContext.Set<T>(); // this.context.Set<T>().AsQueryable().AsNoTracking();

        public async Task<T> AddAsync(T entity, CancellationToken cancellation = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellation);
            return entity;
        }

        public Task AddManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
        {
            return _dbContext.Set<T>().AddRangeAsync(entities, cancellation);
        }

        public Task AddManyAsync(params T[] entities)
        {
            return _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public async Task UpdateAsync(TId id, object values, CancellationToken cancellation = default)
        {
            var existingEntity = await GetByIdAsync(id, cancellation);
            if (existingEntity == null)
                throw new NotFoundException(typeof(T).Name, id);

            _dbContext.Entry(existingEntity).CurrentValues.SetValues(values);
        }

        public Task UpdateAsync(T entity, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().Update(entity);
            return Task.CompletedTask;
            //return UpdateAsync(entity.Id, entity);
        }

        public Task UpdateManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            return Task.CompletedTask;
        }
        public Task UpdateManyAsync(params T[] entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task DeleteManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            return Task.CompletedTask;
        }

        public Task DeleteManyAsync(params T[] entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            return Task.CompletedTask;
        }

        public Task DeleteAllAsync()
        {
            _dbContext.RemoveRange(Entities);
            return Task.CompletedTask;
        }

        public T GetById(TId id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellation = default)
        {
            return Task.FromResult(_dbContext.Set<T>().Where(e => ids.Contains(e.Id)).AsEnumerable());
            //return await Task.WhenAll(ids.Select(id => GetByIdAsync(id, cancellation)));
        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbContext
                .Set<T>()
                .ToListAsync(cancellation);
        }

        public async Task<T> GetByIdAsync(TId id, CancellationToken cancellation = default)
        {
            return await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellation);
        }

        public IEnumerable<T> GetByIds(IEnumerable<TId> ids)
        {
            return _dbContext.Set<T>().Where(e => ids.Contains(e.Id));
        }

        public async Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize, CancellationToken cancellation = default)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellation);
        }
    }
}