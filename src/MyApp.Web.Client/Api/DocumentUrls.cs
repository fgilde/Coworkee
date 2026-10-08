using Microsoft.AspNetCore.Components;

namespace MyApp.Web.Client.Api;

public static class DocumentUrls
{
    public static string Content(Guid id, bool download = false) => $"api/v1/documents/{id}/content" + (download ? "?download=true" : string.Empty);

    /// <summary>The file display loads with its own HttpClient, which needs an absolute address.</summary>
    public static string Absolute(this NavigationManager nav, Guid id) => nav.ToAbsoluteUri(Content(id)).ToString();
}
