namespace MyApp.Documents.Features.Documents.Commands.Upload;

internal static class UploadFileName
{
    public static string Clean(string? fileName) => Path.GetFileName((fileName ?? string.Empty).Replace('\\', '/')).Trim();
}
