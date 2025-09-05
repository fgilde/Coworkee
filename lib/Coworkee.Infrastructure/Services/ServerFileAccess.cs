using System;
using Nextended.Core.Extensions;
using lib.Coworkee.Application.Contracts.Attributes;
using lib.Coworkee.Application.Contracts.Services;
using System.IO;
using System.Linq;
using Nextended.Core.Helper;
using lib.Coworkee.Shared.Constants.Application;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nextended.Core;
using Microsoft.AspNetCore.Hosting;
using Nextended.Core.Attributes;

namespace lib.Coworkee.Infrastructure.Services;

[RegisterAs(typeof(IFileAccess))]
public class ServerFileAccess: IFileAccess
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string ServerFileDirectory = ApplicationConstants.FileAccess.StaticFileDirectoryName;

    public ServerFileAccess(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    // AppContext.BaseDirectory
    public Task<bool> DeleteAsync(string path)
    {
        return Task.Run(() =>
        {
            var filePath = GetPath(GetRelativeUrl(path));
            if (File.Exists(filePath))
            {
                Check.TryCatch<Exception>(() => File.Delete(filePath));
                return !File.Exists(filePath);
            }
            return false;
        });
    }

    public IEnumerable<Task<bool>> DeleteAsync(string[] paths) => paths.Select(DeleteAsync);

    public bool Exists(params string[] segments) => File.Exists(GetPath(segments));

    public string GetRelativeUrl(string fullPath) => fullPath.StartsWith("/") ? fullPath : $"/{ServerFileDirectory}{fullPath[GetServerFileDirectory().Length..].Replace("\\", "/").EnsureStartsWith("/")}";


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
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();
        if (relative.FirstOrDefault() != ServerFileDirectory)
            relative = relative.Prepend(ServerFileDirectory).ToArray();

        string path = relative.Aggregate(Path.Combine);

        var res = GetServerFileDirectory(path);

        var file = Path.GetFileName(res);
        bool hasExtension = !string.IsNullOrEmpty(Path.GetExtension(file));

        var dir = hasExtension ? Path.GetDirectoryName(res) : res;
        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return res;
    }

    public string GetPath(string path) => GetPath(path.Split(Path.DirectorySeparatorChar));
    

    internal string GetServerFileDirectory(string relative = null)
    {
        return Path.Combine(_webHostEnvironment.WebRootPath, relative ?? ServerFileDirectory);
        //return Path.Combine(Directory.GetCurrentDirectory(), relative ?? ServerFileDirectory);
    }
}