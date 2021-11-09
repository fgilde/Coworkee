using System.Security.Claims;
using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Infrastructure.Authentication;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Client.Shared.Components
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; }
        [Parameter] public bool ShowEmail { get; set; } = true;
        [Parameter] public bool ShowLogout { get; set; }
        [Parameter] public ClaimsPrincipal User { get; set; }
        [Parameter] public UserResponse UserData { get; set; }
        [Parameter] public string UserId { get; set; }
        [Parameter] public EventCallback Logout { get; set; }
        [Parameter] public bool? IsUserOnline { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }

        protected override async Task OnInitializedAsync()
        {            
            if (string.IsNullOrWhiteSpace(UserId) && UserData == null)
                await LoadCurrentUserData();
            else
                await LoadOtherUserData();
        }

        private async Task LoadCurrentUserData()
        {
            User ??= (await _stateProvider.GetAuthenticationStateAsync()).User;
            Email = User.GetEmail();
            FirstName = User.GetFirstName();
            SecondName = User.GetLastName();
        }

        protected async Task LoadOtherUserData()
        {
            if (UserData == null && !string.IsNullOrWhiteSpace(UserId))
            {
                var result = await _api.User_GetByIdAsync(UserId);
                if (result.Succeeded)
                    UserData = result.Data;
            }
            if (UserData != null)
            {
                Email = UserData.Email;
                FirstName = UserData.FirstName;
                SecondName = UserData.LastName;
            }
        }
    }
}