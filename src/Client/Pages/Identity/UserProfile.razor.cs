using CleanArchitectureBase.Application.Requests.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Identity;

namespace CleanArchitectureBase.Client.Pages.Identity
{
    public partial class UserProfile
    {
        [Parameter] public string Id { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Description { get; set; }

        private UserResponse user;

        private bool _loaded;

        private async Task ToggleUserStatus()
        {
            var request = new ToggleUserStatusRequest { ActivateUser = user.IsActive, EmailConfirmed = user.EmailConfirmed, UserId = Id };
            var result = await _api.User_ToggleUserStatusAsync(request);
            
            if (_errorService.IsSuccessFull(result))
            {
                _snackBar.Add(_localizer["Updated User Status."], Severity.Success);
                _navigationManager.NavigateTo("/identity/users");
            }
        }

        [Parameter] public string ImageDataUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var userId = Id;
            var result = await _api.User_GetByIdAsync(userId);
            if (result.Succeeded)
            {
                user = result.Data;
                if (user != null)
                {
                    Title = $"{user.FirstName} {user.LastName}'s {_localizer["Profile"]}";
                    Description = user.Email;
                    var data = await _api.Account_GetProfilePictureAsync(userId);
                    if (data.Succeeded)
                    {
                        ImageDataUrl = data.Data;
                    }
                }

            }

            _loaded = true;
        }
    }
}