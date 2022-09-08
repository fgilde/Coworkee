using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HeyRed.Mime;

namespace CleanArchitectureBase.Shared.Helper;

public static class MimeTypeHelper
{

    public static bool IsZip(string contentType)
    {
        return !string.IsNullOrWhiteSpace(contentType) && Matches(contentType, "application/zip*", "application/x-zip*");
    }

    /**
     * Returns true if the given mimeType matches any of given mimeTypes
     */
    public static bool Matches(string mimeType, params string[] mimeTypes)
    {
        if (mimeTypes == null || mimeTypes.Length == 0)
            return false;
        return mimeTypes.Any(type => mimeType != null && (type.Equals(mimeType, StringComparison.InvariantCultureIgnoreCase) || Regex.IsMatch(mimeType, type, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)));
    }


    private static bool IsValidUrl(string s) => Uri.TryCreate(s, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

    public static async Task<string> ReadContentTypeAsync(string url, CancellationToken cancellationToken = default)
    {
        string contentType = "application/octet-stream";
        try
        {
            if (!IsValidUrl(url))
            {
                return MimeTypesMap.GetMimeType(url);
            }
            var res = await new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Head, url), cancellationToken);
            contentType = res.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        }
        catch
        { }
        return contentType;
    }
}
