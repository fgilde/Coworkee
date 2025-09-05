using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Coworkee.Application.Contracts.Services;

public interface IZipService
{
    Task<byte[]> CreateArchiveAsync(IEnumerable<string> files, CancellationToken cancellationToken = default);
    Task<byte[]> CreateArchiveAsync(IEnumerable<FileInfo> files, CancellationToken cancellationToken = default);
}
