using Coworkee.Application.Requests.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Hubs;
using Coworkee.Client.Extensions;
using Coworkee.Client.Shared.Dialogs;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;
using MudBlazor.Extensions.Components;

namespace Coworkee.Client.Pages.Identity
{
    public partial class UserProfile
    {
        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Description { get; set; }

        private UserResponse user;
        
        private async Task ToggleUserStatus()
        {
            var request = new ToggleUserStatusRequest { ActivateUser = user.IsActive, EmailConfirmed = user.EmailConfirmed, UserId = Id };
            var result = await _api.User_ToggleUserStatusAsync(request);

            if (_errorService.IsSuccessFull(result))
            {
                _snackBar.Add(_localizer["Updated User Status."], Severity.Success);
                if (!user.IsActive || !user.EmailConfirmed)
                    await ExecuteUserLogout();
                _navigationManager.NavigateTo("/identity/users");
            }
        }

        [Parameter] public string ImageDataUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await _api.User_GetByIdAsync(Id);
            if (_errorService.IsSuccessFull(result))
            {
                user = result.Data;
                Title = $"{user.FirstName} {user.LastName}'s {_localizer["Profile"]}";
                Description = user.Email;
                HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
            }
        }

        private async Task LogoutUser()
        {
            var actions = new[]
            {
                new MudExDialogResultAction
                {
                    Label = "Cancel",
                    Variant = Variant.Text,
                    Result = DialogResult.Cancel()
                },
                new MudExDialogResultAction
                {
                    Label = "Confirm",
                    Color = Color.Error,
                    Variant = Variant.Filled,
                    Result = DialogResult.Ok(true)
                },
            };
            var parameters = new DialogParameters
            {
                {nameof(MudExMessageDialog.Message), $"{_localizer["Are you sure you want to force logout for user {0}", user.FullName]}"},
                {nameof(MudExMessageDialog.Icon), Icons.Material.Filled.Logout},
                {nameof(MudExMessageDialog.Class), "mud-ex-dialog-initial"},
                {nameof(MudExMessageDialog.Buttons), actions}
            };
            var options = new DialogOptionsEx { CloseButton = true, BackdropClick = true, Animations = DialogServiceExtensions.DefaultAnimationNoFullHeight };
            var dialog = await _dialogService.ShowEx<MudExMessageDialog>(_localizer["Logout user"], parameters, options);

            if (!(await dialog.Result).Canceled)
            {
                await ExecuteUserLogout();
                _snackBar.Add(_localizer["The user {0} has been logged off", user.FullName], Severity.Success);
            }
        }

        private async Task ExecuteUserLogout()
        {
            await HubConnection.TrySendAsync(_config.BackendOrigin, nameof(ClientEventHub.LogoutUserById), Id);
        }
    }
}