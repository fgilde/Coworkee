using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Shared.Wrapper;
using MudBlazor.Extensions;
using System.IO;
using MudBlazor;
using MudBlazor.Extensions.Options;
using Coworkee.Client.Extensions;
using Coworkee.Client.Shared.Components;

namespace Coworkee.Client.Pages.Administration
{
    public partial class Database
    {

        [Parameter] public string Action { get; set; }

        [Parameter] public string Id { get; set; }


        private async Task<PaginatedResult<DatabaseBackupDto>> Load(int pageNumber, int pageSize, string searchString,
            string[] orderings, CancellationToken cancellationToken)
        {
            var r = await _api.Database_GetDatabaseBackupsAsync(pageNumber, pageSize, searchString, orderings,
                cancellationToken: cancellationToken);
            return r;
        }

        private Task<DatabaseBackupDto> FindById(string id, IEnumerable<DatabaseBackupDto> loaded)
        {
            return Task.FromResult(loaded.FirstOrDefault(p => p.Name == id));
        }

        private string GetId(DatabaseBackupDto db)
        {
            return db.Name;
        }

        private async Task<Result> Delete(string[] ids)
        {
            await _api.Database_DeleteAsync(ids.ToList());
            return new Result { Succeeded = true };
        }

        private string GetName(DatabaseBackupDto arg)
        {
            return arg.Name;
        }


        private async Task<bool> CreateOrEdit(DatabaseBackupDto backupOrNull)
        {
            return backupOrNull == null ? await CreateNewBackupAsync() : await RenameBackupAsync(backupOrNull);
        }

        private async Task<bool> CreateNewBackupAsync()
        {
            var name = await _dialogService.PromptAsync(_localizer["Create new backup"], _localizer["Enter name of backup file"],
                icon: Icons.Material.Filled.Save, initialValue: $"DB_Backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.bak",
                canConfirm: (s) => !string.IsNullOrWhiteSpace(s) && !string.IsNullOrWhiteSpace(Path.GetExtension(s)));
            if (!string.IsNullOrEmpty(name))
            {
                await _api.Database_CreateDatabaseBackupAsync(name);
                return true;
            }

            return false;
        }

        private async Task<bool> RenameBackupAsync(DatabaseBackupDto backup)
        {
            var name = await _dialogService.PromptAsync(_localizer["Rename backup"], _localizer["Enter new name of backup file"],
                icon: Icons.Material.Filled.Save, initialValue: backup.Name,
                canConfirm: (s) => !string.IsNullOrWhiteSpace(s) && !string.IsNullOrWhiteSpace(Path.GetExtension(s)));
            if (!string.IsNullOrEmpty(name))
            {
                await _api.Database_RenameAsync(backup, name);
                return true;
            }

            return false;
        }

        private async Task DownloadAsync(DatabaseBackupDto[] backups)
        {
            var file = await _api.Database_DownloadAsync(backups.Select(b => b.Name).ToList());
            await file.ForceDownloadAsync(_jsRuntime);
        }

        private DialogOptionsEx DialogOptions(AnimationType animationType = AnimationType.FlipX)
        {
            return new DialogOptionsEx
            {
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                BackdropClick = false,
                CloseButton = true,
                Animations = new[] { animationType }
            };
        }

        private EditableDataTable.CustomAction<DatabaseBackupDto>[] GetCustomActions()
        {
            return new[]
            {
                new EditableDataTable.CustomAction<DatabaseBackupDto>
                {
                    Text = "Download",
                    Icon = Icons.Material.Filled.Download,
                    Action = async (backups) => await DownloadAsync(backups)
                },
                new EditableDataTable.CustomAction<DatabaseBackupDto>
                {
                    Text = "Restore",
                    Icon = Icons.Material.Filled.Restore,
                    Availability = EditableDataTable.CustomActionAvailability.SingleOnly,
                    Action = async (backups) =>
                    {
                        var message = _localizer["Do you really want to restore the database from backup '{0}'? Note that any changes made after {1} are gone. This also means any changes made by any user", backups[0].Name, backups[0].CreatedOn ];
                        var backup = backups.First();
                        var result = await _dialogService.ShowConfirmationDialogAsync("Restore database", message, "Restore", "Cancel", icon: Icons.Material.Filled.Warning, options: DialogOptions());
                        if (result)
                        {
                            var dlg = await _dialogService.ShowInformationAsync("Restore Database", "Restore in progress. Please wait until the restore is finished. This can take a while.", Icons.Material.Filled.Restore, false, true, DialogOptions(AnimationType.FadeIn));
                            await _api.Database_RestoreAsync(backup);
                            dlg.Close();
                            _snackBar.Add("Restore finished", Severity.Success);
                        }
                    }
                }
            };
        }


    }
}