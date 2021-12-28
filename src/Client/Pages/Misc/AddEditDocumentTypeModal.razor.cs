using System;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace CleanArchitectureBase.Client.Pages.Misc
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
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
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