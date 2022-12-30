using Nextended.Core.Extensions;
using Coworkee.Application.Contracts.Attributes;
using Coworkee.Application.Contracts.Services;
using System.IO;
using System.Linq;
using Nextended.Core.Helper;
using Coworkee.Shared.Constants.Application;

namespace Coworkee.Infrastructure.Services;

[RegisterAs(typeof(IFileAccess))]
public class ServerFileAccess: IFileAccess
{
    private const string ServerFileDirectory = ApplicationConstants.FileAccess.StaticFileDirectoryName;
    // AppContext.BaseDirectory
    public string GetRelativeUrl(string fullPath)
    {
        return $"/{ServerFileDirectory}{fullPath.Substring(GetServerFileDirectory().Length).Replace("\\", "/").EnsureStartsWith("/")}";
    }

    public string EnsureFileNotExists(params string[] segments)
    {
        var res = GetPath(segments);
        return File.Exists(res) ? FileHelper.NextAvailableFilename(res) : res;
    }

    public string EnsureFileNotExists(string path) => EnsureFileNotExists(path.Split(Path.DirectorySeparatorChar));

    public string GetPath(params string[] segments)
    {
        var relative = segments
            .SelectMany(s => s.Split(Path.DirectorySeparatorChar))
            .SelectMany(s => s.Split('/'))
            .Prepend(ServerFileDirectory).Aggregate(Path.Combine);

        var res = GetServerFileDirectory(relative);
        
        var file = Path.GetFileName(res);
        bool hasExtension = !string.IsNullOrEmpty(Path.GetExtension(file));

        var dir = hasExtension ? Path.GetDirectoryName(res) : res;
        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return res;
    }

    public string GetPath(string path) => GetPath(path.Split(Path.DirectorySeparatorChar));
    

    internal static string GetServerFileDirectory(string relative = null)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), relative ?? ServerFileDirectory);
    }
}