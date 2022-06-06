using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.JsInterop;
using CleanArchitectureBase.Client.Shared.Components;
using HeyRed.Mime;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Nextended.Core.Extensions;
using CleanArchitectureBase.Shared.Helper;
using CleanArchitectureBase.Shared.Misc;
using Microsoft.JSInterop;

namespace CleanArchitectureBase.Client.Extensions;

public static class BrowserFileExtensions
{
    public static async Task DownloadAsync(this IBrowserFile browserFile, IJSRuntime jsRuntime)
    {
        var url = await DataUrl.GetDataUrlAsync(await browserFile.GetBytesAsync(), browserFile.ContentType);
        await jsRuntime.InvokeVoidAsync(JsNamespace.Get("BrowserHelper", "download"), new
        {
            Url = url,
            FileName = $"{browserFile.Name}",
            MimeType = browserFile.ContentType
        });
    }

    public static async Task<string> GetDataUrlAsync(this IBrowserFile file)
    {
        return await DataUrl.GetDataUrlAsync(await file.GetBytesAsync(), file.ContentType);
    }

    public static async Task<byte[]> GetBytesAsync(this IBrowserFile file, CancellationToken cancellationToken = default)
    {
        if (file is ZipBrowserFile {FileBytes: { }} zipEntry)
            return zipEntry.FileBytes;
        var buffer = new byte[file.Size];
        await file.OpenReadStream(file.Size).ReadAsync(buffer, cancellationToken);
        return buffer;
    }

    public static byte[] GetBytes(this IBrowserFile file)
    {
        var buffer = new byte[file.Size];
        file.OpenReadStream(file.Size).Read(buffer);
        return buffer;
    }

    public static string GetReadableFileSize(this IBrowserFile file, IStringLocalizer localizer, bool fullName = false)
    {
        return GetReadableFileSize(file.Size, localizer, fullName);
    }

    public static string GetReadableFileSize(long size, IStringLocalizer localizer, bool fullName = false)
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
        double length = size;
        int index;
        for (index = 0; length >= 1024.0 && index + 1 < source.Count; length /= 1024.0)
            ++index;
        KeyValuePair<string, string> keyValuePair = source.ElementAt(index);
        return $"{length:0.##} {localizer[fullName ? keyValuePair.Value : keyValuePair.Key]}";
    }

    public static string GetIcon(this IBrowserFile file)
    {
        return IconForFile(file.ContentType);
    }

    public static bool IsZipFile(this IBrowserFile file)
    {
        return MimeTypeHelper.IsZip(file.ContentType);
    }

    public static string IconForFile(string contentType)
    {
        return IconForFile(new ContentType(contentType));
    }

    public static string IconForFile(ContentType contentType)
    {
        //Icons.Custom.Brands.MicrosoftVisualStudio
        //Icons.Filled.AlternateEmail
        //Icons.Custom.Brands.MicrosoftAzure
        //Icons.Custom.Brands.GitHub
        var mime = contentType.ToString().ToLower();
        if (MimeTypeHelper.IsZip(mime))
            return Icons.Filled.Archive;
        if (MimeTypeHelper.Matches(mime, "application/vnd.ms-excel", "text/csv", "application/vnd.openxmlformats-officedocument.spreadsheetml*", "application/vnd.ms-excel*"))
            return Icons.Custom.FileFormats.FileExcel;
        if (MimeTypeHelper.Matches(mime, "application/vnd.ms*", "application/msword", "application/vnd.openxmlformats-officedocument*"))
            return Icons.Custom.FileFormats.FileWord;
        if (MimeTypeHelper.Matches(mime, "application/pdf"))
            return Icons.Custom.FileFormats.FilePdf;
        if (MimeTypeHelper.Matches(mime, "application/java*", "application/json*", "text/html", "text/xml", "application/xml"))
            return Icons.Custom.FileFormats.FileCode;

        return contentType.MediaType.Split("/").First().ToLower() switch
        {
            "image" => Icons.Custom.FileFormats.FileImage,
            "audio" => Icons.Custom.FileFormats.FileMusic,
            "video" => Icons.Custom.FileFormats.FileVideo,
            "text" => Icons.Custom.FileFormats.FileDocument,
            _ => Icons.Custom.FileFormats.FileDocument
        };
    }


    public static string IconForExtension(string extension)
    {
        return IconForFile(MimeTypesMap.GetMimeType(extension = extension?.EnsureStartsWith(".")));
    }
}