using System;
using Coworkee.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Hubs;
using Coworkee.Shared.Constants.Role;

namespace Coworkee.Client.Pages.Identity
{
    public partial class RoleModal: IAsyncDisposable
    {
        private bool isSystemRequiredRole => RoleModel?.Name == RoleConstants.AdministratorRole || RoleModel?.Name == RoleConstants.BasicRole;

        [Parameter] public RoleDto RoleModel { get; set; } = new();
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
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
                await HubConnection.TrySendAsync(_config.BackendOrigin, nameof(ClientEventHub.UpdateDashboardAsync));
                MudDialog.Close();
            }
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}