using System.Threading;
using System.Threading.Tasks;

namespace lib.Coworkee.Application.Contracts.Services;

public interface IDbBackupService
{
    string BackupDirectory { get; }

    Task<string> BackupDatabaseAsync(string fileName = null, CancellationToken cancellationToken = default);
    Task RestoreDatabaseAsync(string fullFileName, CancellationToken cancellationToken = default);
}