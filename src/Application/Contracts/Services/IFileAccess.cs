using System.Collections.Generic;
using System.IO;

namespace Coworkee.Application.Contracts.Services;

public interface IFileAccess
{
    string GetRelativeUrl(string fullPath);
    string EnsureFileNotExists(params string[] segments);
    string EnsureFileNotExists(string path);
    string GetPath(params string[] segments);
    string GetPath(string path);
    
    IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
        => Directory.EnumerateFiles(GetPath(path), searchPattern, enumerationOptions);
    IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        => Directory.EnumerateFiles(GetPath(path), searchPattern, searchOption);
    IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        => Directory.EnumerateFiles(GetPath(path), searchPattern);
    IEnumerable<string> EnumerateFiles(string path)
        => Directory.EnumerateFiles(GetPath(path));
}