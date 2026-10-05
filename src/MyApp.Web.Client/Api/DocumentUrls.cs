namespace MyApp.Web.Client.Api;

public static class DocumentUrls
{
    public static string Content(Guid id, bool download = false) => $"api/v1/documents/{id}/content" + (download ? "?download=true" : string.Empty);
}
