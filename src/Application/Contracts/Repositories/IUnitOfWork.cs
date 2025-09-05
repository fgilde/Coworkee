using System;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Domain.Contracts;

namespace Coworkee.Application.Contracts.Repositories
{
    public interface IUnitOfWork<TId> : IDisposable
    {
        IRepositoryAsync<T, TId> Repository<T>() where T : class, IEntity<TId>;

        Task<int> Commit(CancellationToken cancellationToken);

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}