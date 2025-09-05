using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace lib.Coworkee.Application.Contracts.Services;

public interface IFileAccess
{
    Task<bool> DeleteAsync(string path);
    IEnumerable<Task<bool>> DeleteAsync(string[] paths);
    bool Exists(params string[] segments);
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