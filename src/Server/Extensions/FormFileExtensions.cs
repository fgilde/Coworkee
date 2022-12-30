using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Coworkee.Server.Extensions;

public static class FormFileExtensions
{
    public static async Task<byte[]> GetBytesAsync(this IFormFile formFile, CancellationToken cancellationToken = default)
    {
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    public static byte[] GetBytes(this IFormFile formFile)
    {
        using var memoryStream = new MemoryStream();
        formFile.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}