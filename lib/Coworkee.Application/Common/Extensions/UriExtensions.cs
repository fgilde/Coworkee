using System;

namespace Coworkee.Application.Common.Extensions;

public static class UriExtensions
{
    public static string Origin(this Uri uri)
    {
        var port = uri.Port != default && uri.Port != 80 ? $":{uri.Port}" : string.Empty;
        return $"{uri.Scheme}://{uri.Host}{port}";
    }
}