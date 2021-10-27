using System.Security.Claims;
using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Infrastructure.Authentication;
namespace CleanArchitectureBase.Client.Shared.Components
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; }
        [Parameter] public bool ShowEmail { get; set; } = true;
        [Parameter] public bool ShowLogout { get; set; }
        [Parameter] public ClaimsPrincipal User { get; set; }
        [Parameter] public EventCallback Logout { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User ??= (await _stateProvider.GetAuthenticationStateAsync()).User;
            LoadData();
        }

        private void LoadData()
        {
            Email = User.GetEmail();
            FirstName = User.GetFirstName();
            SecondName = User.GetLastName();
        }
    }
}