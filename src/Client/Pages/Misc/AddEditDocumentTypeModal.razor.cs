using System;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Features.DocumentTypes.Commands.AddEdit;
using Coworkee.Application.Hubs;
using Coworkee.Client.Extensions;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace Coworkee.Client.Pages.Misc
{
    public partial class AddEditDocumentTypeModal: IAsyncDisposable
    {
        [Parameter] public DocumentTypeDto AddEditDocumentTypeModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            await _api.DocumentTypes_PostAsync(new AddEditDocumentTypesCommand(AddEditDocumentTypeModel));
            _snackBar.Add(_localizer["DocumentType Updated"], Severity.Success);
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
            await Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}