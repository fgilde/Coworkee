using System;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Domain.Contracts;

namespace lib.Coworkee.Application.Contracts.Repositories
{
    public interface IExtendedAttributeUnitOfWork<TId, TEntityId, TEntity> : IDisposable where TEntity : class, IEntity<TEntityId>
    {
        IRepositoryAsync<T, TId> Repository<T>() where T : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>;

        Task<int> Commit(CancellationToken cancellationToken);

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}