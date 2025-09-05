using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Attributes;
using lib.Coworkee.Application.Contracts.Services;
using Nextended.Core.Attributes;

namespace lib.Coworkee.Application.Common.Services;

[RegisterAs(typeof(IZipService))]
public class ZipService : IZipService
{
    public Task<byte[]> CreateArchiveAsync(IEnumerable<string> files, CancellationToken cancellationToken = default)
    {
        return CreateArchiveAsync(files.Select(f => new FileInfo(f)), cancellationToken);
    }

    public Task<byte[]> CreateArchiveAsync(IEnumerable<FileInfo> files, CancellationToken cancellationToken = default)
    {
        return Task.Run(async () =>
        {
            using var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var fileInfo in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var zipEntry = zipArchive.CreateEntry(fileInfo.Name);
                    await using var entryStream = zipEntry.Open();
                    await using var fileStream = fileInfo.OpenRead();
                    await fileStream.CopyToAsync(entryStream, 81920, cancellationToken);
                }
            }

            return memoryStream.ToArray();
        }, cancellationToken);
    }

}