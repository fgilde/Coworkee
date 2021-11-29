using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Client.Extensions;

public static class BrowserFileExtensions
{

    public static async Task<byte[]> GetBytesAsync(this IBrowserFile file, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[file.Size];
        await file.OpenReadStream().ReadAsync(buffer, cancellationToken);
        return buffer;
    }

    public static byte[] GetBytes(this IBrowserFile file)
    {
        var buffer = new byte[file.Size];
        file.OpenReadStream().Read(buffer);
        return buffer;
    }

    public static string GetReadableFileSize(this IBrowserFile file, IStringLocalizer localizer, bool fullName = false)
    {
        var source = new Dictionary<string, string>
        {
            {"B", "Bytes"},
            {"KB", "Kilobytes"},
            {"MB", "Megabytes"},
            {"GB", "Gigabytes"},
            {"TB", "Terabytes"},
            {"PB", "Petabytes"},
            {"EB", "Exabytes"},
            {"ZB", "Zettabytes"},
            {"YB", "Yottabytes"},
            {"BB", "Brontobytes"}
        };
        double length = file.Size;
        int index;
        for (index = 0; length >= 1024.0 && index + 1 < source.Count; length /= 1024.0)
            ++index;
        KeyValuePair<string, string> keyValuePair = source.ElementAt(index);
        return $"{length:0.##} {localizer[fullName ? keyValuePair.Value : keyValuePair.Key]}";
    }
}