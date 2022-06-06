using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using HeyRed.Mime;
using Microsoft.AspNetCore.Components.Forms;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Shared.Components;

public enum SelectItemsMode
{
    None,
    Single,
    MultiSelect,
    MultiSelectWithCtrlKey
}

public enum MimeTypeRestrictionType
{
    WhiteList,
    BlackList
}

public record ZipBrowserFile: IBrowserFile
{
    public ZipArchiveEntry Entry { get; }
    public byte[] FileBytes { get; private set; }

    public ZipBrowserFile(ZipArchiveEntry entry, bool load = true)
    {
        if (load)
            FileBytes = GetBytes(entry.Open());

        Entry = entry;
        Name = Entry.Name;
        Size = Entry.Length;
        LastModified = Entry.LastWriteTime;
        FullName = entry.FullName;
        ContentType = MimeTypesMap.GetMimeType(entry.FullName);

        if (string.IsNullOrWhiteSpace(FullName))
            FullName = Name;
        if (string.IsNullOrWhiteSpace(Name) && IsDirectory)
            Name = PathArray.Last(s => !string.IsNullOrEmpty(s));
    }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        return new MemoryStream(FileBytes ??= GetBytes(Entry.Open()));
    }

    public static byte[] GetBytes(Stream input)
    {
        using var ms = new MemoryStream();
        input.CopyToAsync(ms);
        return ms.ToArray();
    }

    public string Name { get; init; }
    public DateTimeOffset LastModified { get; }
    public long Size { get; }
    public string ContentType { get; }
    public string FullName { get; }
    public string Path => FullName.TrimEnd(Name.ToCharArray());
    public bool IsDirectory => Path == FullName;
    public string[] PathArray => Path.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
    public string ParentDirectoryName => PathArray?.Any() == true ? !IsDirectory ? PathArray.Last(s => !string.IsNullOrEmpty(s)) : PathArray[^2] : string.Empty;
}