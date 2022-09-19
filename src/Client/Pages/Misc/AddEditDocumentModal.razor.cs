using System;
using System.Collections.Generic;
using CleanArchitectureBase.Application.Features.Documents.Commands.AddEdit;
using CleanArchitectureBase.Application.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Hubs;
using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using CleanArchitectureBase.Shared.Misc;
using Nextended.Blazor.Extensions;
using Nextended.Core;


namespace CleanArchitectureBase.Client.Pages.Misc
{
    public partial class AddEditDocumentModal : IAsyncDisposable
    {

        [Parameter] public DocumentDto AddEditDocumentModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private Stream _currentContentStream;

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private IList<DocumentTypeDto> _documentTypes = new List<DocumentTypeDto>();

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            await _api.Documents_PostAsync(new AddEditDocumentsCommand(AddEditDocumentModel));
            _snackBar.Add(_localizer["Document Updated"], Severity.Success);
            MudDialog.Close();
            await HubConnection.SendAsync(nameof(ClientEventHub.UpdateDashboardAsync));
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
        }

        private async Task LoadDataAsync()
        {
            await LoadDocumentTypesAsync();
        }

        private async Task LoadDocumentTypesAsync()
        {
            _documentTypes = await _api.DocumentTypes_GetAllAsync();
        }

        private IBrowserFile _file;

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                var buffer = new byte[_file.Size];
                var extension = Path.GetExtension(_file.Name);
                await _file.OpenReadStream(_file.Size).ReadAsync(buffer);

                if (MimeType.Matches(e.File.ContentType, "image*"))
                    AddEditDocumentModel.URL = await (await e.File.RequestImageFileAsync("image/png", 1000, 1000)).GetDataUrlAsync();
                else
                    AddEditDocumentModel.URL = await DataUrl.GetDataUrlAsync(buffer, e.File.ContentType);

                AddEditDocumentModel.UploadRequest = new UploadRequest { Data = buffer, ContentType = e.File.ContentType, FileName = _file.Name, UploadType = UploadType.Document, Extension = extension };
                AddEditDocumentModel.ContentType = e.File.ContentType;
                AddEditDocumentModel.Title ??= _file.Name;
                AddEditDocumentModel.Description ??= $"{extension?.Substring(1).ToUpper()} File '{_file.Name}' from {_file.LastModified.ToString("D")}";
                _currentContentStream = new MemoryStream(buffer);
            }
        }

        private async Task<IEnumerable<int>> SearchDocumentTypes(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _documentTypes.Select(x => x.Id);

            return _documentTypes.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}