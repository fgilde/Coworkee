using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.Brands.Commands.AddEdit;
using CleanArchitectureBase.SDK;

namespace CleanArchitectureBase.Client.Pages.Catalog
{
    public partial class AddEditBrandModal
    {
        [Inject] private IApplicationClient Api { get; set; }

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
            await Api.Brands_PostAsync(new AddEditBrandsCommand(AddEditBrandModel));
            _snackBar.Add(_localizer["Brand Updated"], Severity.Success);
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
            MudDialog.Close();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = HubConnection.TryInitialize(_config.BackendOrigin);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}