namespace MyApp.Contracts.Documents;

public static class DocumentInlineTypes
{
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        "application/pdf", "image/jpeg", "image/png", "image/gif", "image/webp", "video/mp4", "video/webm", "audio/mpeg", "audio/wav", "audio/ogg", "text/plain", "text/csv",
    };
}
