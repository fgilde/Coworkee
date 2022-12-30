using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Shared.Constants.Role;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Server.Controllers;

public class DatabaseController : BaseApiController<DatabaseController>
{
    [Authorize(Roles = RoleConstants.AdministratorRole)]
    [HttpPost]
    public async Task<IActionResult> CreateDatabaseBackup(string name, CancellationToken cancellationToken = default)
    {
        var file = await Get<IDbBackupService>().BackupDatabaseAsync(name, cancellationToken);
        var bytes = await System.IO.File.ReadAllBytesAsync(file, cancellationToken);
        return File(bytes, "application/octet-stream", System.IO.Path.GetFileName(file));
    }

    [Authorize(Roles = RoleConstants.AdministratorRole)]
    [HttpPut]
    public IActionResult Rename(DatabaseBackupDto backup, string newName)
    {
        var file = Get<IFileAccess>().EnumerateFiles(Get<IDbBackupService>().BackupDirectory, backup.Name).FirstOrDefault();
        if (!System.IO.File.Exists(file))
            return NotFound();
        var newFile = Path.Combine(Path.GetDirectoryName(file), newName);
        System.IO.File.Move(file, newFile);
        return Ok();
    }

    [Authorize(Roles = RoleConstants.AdministratorRole)]
    [HttpPost(nameof(Download))]
    public async Task<IActionResult> Download(string[] files, CancellationToken cancellationToken = default)
    {
        var bytes = await Get<IZipService>().CreateArchiveAsync(files.Select(FullPath), cancellationToken);
        return File(bytes, "application/zip", files.Length == 1 ? $"{files[0].Trim().Replace(" ", "_")}.zip" : "backups.zip");
    }

    [Authorize(Roles = RoleConstants.AdministratorRole)]
    [HttpDelete]
    public async Task<IActionResult> Delete(string[] names, CancellationToken cancellationToken = default)
    {
        var tasks = Get<IFileAccess>().EnumerateFiles(Get<IDbBackupService>().BackupDirectory).Where(f => names.Contains(Path.GetFileName(f))).Select(f => Task.Run(() => System.IO.File.Delete(f), cancellationToken));
        await Task.WhenAll(tasks);

        return Ok();
    }
    
    [Authorize(Roles = RoleConstants.AdministratorRole)]
    [HttpGet]
    [Produces(typeof(PaginatedResult<DatabaseBackupDto>))]
    public IActionResult GetDatabaseBackups([FromQuery] GetAllPagedQueryBase<DatabaseBackupDto> query)
    {
        var search = query?.SearchString ?? "*";
        if (!search.Contains("*"))
            search = $"*{search}*";
        var res = Get<IFileAccess>().EnumerateFiles(Get<IDbBackupService>().BackupDirectory, search).Select(f => new DatabaseBackupDto
        {
            Name = Path.GetFileName(f),
            Size = new FileInfo(f).Length,
            CreatedOn = System.IO.File.GetCreationTimeUtc(f)
        });

        var queryable = query?.OrderBy?.Any() == true ? res.AsQueryable().OrderBy(string.Join(",", query.OrderBy)) : res.AsQueryable();
        return Ok(queryable.ToPaginatedList(query?.PageNumber ?? 1, query?.PageSize ?? 25));
    }

    [Authorize(Roles = RoleConstants.AdministratorRole)]
    [HttpPost(nameof(Restore))]
    public async Task<IActionResult> Restore(DatabaseBackupDto backup, CancellationToken cancellationToken = default)
    {
        await Get<IDbBackupService>().RestoreDatabaseAsync(backup.Name, cancellationToken);
        return Ok();
    }

    private string FullPath(string name) => Get<IFileAccess>().GetPath(Get<IDbBackupService>().BackupDirectory, name);
}