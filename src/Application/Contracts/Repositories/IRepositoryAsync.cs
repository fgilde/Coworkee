using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Domain.Contracts;

namespace Coworkee.Application.Contracts.Repositories
{
    public interface IRepositoryAsync<T, in TId> where T : class, IEntity<TId>
    {
        IQueryable<T> Entities { get; }

        Task<T> GetByIdAsync(TId id, CancellationToken cancellation = default);
        T GetById(TId id);
        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellation = default);
        IEnumerable<T> GetByIds(IEnumerable<TId> ids);

        Task<List<T>> GetAllAsync(CancellationToken cancellation = default);
        Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize, CancellationToken cancellation = default);

        Task<T> AddAsync(T entity, CancellationToken cancellation = default);
        Task AddManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default);
        Task AddManyAsync(params T[] entities);

        Task UpdateAsync(TId id, object values, CancellationToken cancellation = default);
        Task UpdateAsync(T entity, CancellationToken cancellation = default);
        Task UpdateManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default);
        Task UpdateManyAsync(params T[] entities);

        Task DeleteAsync(T entity, CancellationToken cancellation = default);
        Task DeleteManyAsync(IEnumerable<T> entities, CancellationToken cancellation = default);
        Task DeleteManyAsync(params T[] entities);
        Task DeleteAllAsync();
    }
}