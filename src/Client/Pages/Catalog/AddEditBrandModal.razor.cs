using System;
using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.Brands.Commands.AddEdit;
using CleanArchitectureBase.Application.Hubs;

namespace CleanArchitectureBase.Client.Pages.Catalog
{
    public partial class AddEditBrandModal: IAsyncDisposable
    {
        [Parameter] public BrandDto AddEditBrandModel { get; set; } = new();
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
            await _api.Brands_PostAsync(new AddEditBrandsCommand(AddEditBrandModel));
            _snackBar.Add(_localizer["Brand Updated"], Severity.Success);
            await HubConnection.SendAsync(nameof(ClientEventHub.UpdateDashboardAsync));
            MudDialog.Close();
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