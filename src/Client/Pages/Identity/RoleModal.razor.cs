using System;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CleanArchitectureBase.Application.Hubs;

namespace CleanArchitectureBase.Client.Pages.Identity
{
    public partial class RoleModal: IAsyncDisposable
    {
        
        [Parameter] public RoleRequest RoleModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
        }

        private async Task SaveAsync()
        {
            var response = await _api.Role_PostAsync(RoleModel);
            if (_errorService.IsSuccessFull(response))
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                await HubConnection.SendAsync(nameof(ClientEventHub.UpdateDashboardAsync));
                MudDialog.Close();
            }
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}