using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Coworkee.Application.Contracts.Services;

public interface IDbBackupService
{
    string BackupDirectory { get; }

    Task<string> BackupDatabaseAsync(string fileName = null, CancellationToken cancellationToken = default);
    Task RestoreDatabaseAsync(string fullFileName, CancellationToken cancellationToken = default);
}