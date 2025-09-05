using System;
using System.Security.Claims;
using Coworkee.Client.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Extensions;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Hubs.Events;
using lib.Coworkee.Shared.Constants.Storage;
using Microsoft.AspNetCore.SignalR.Client;

namespace Coworkee.Client.Shared.Components
{
    public partial class UserCard : IAsyncDisposable
    {
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        [Parameter] public string Class { get; set; }
        [Parameter] public string Style { get; set; }
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
        private bool _isLoading;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await Load();
        }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);

            HubConnection.On<UserProfileChanged>(a =>
            {
                if (a.User.Id == UserId || a.User.Id == UserData?.Id || a.User.Id == User?.GetUserId())
                {
                    UserData = a.User;
                    FirstName = a.User.FirstName;
                    SecondName = a.User.LastName;
                    Email = a.User.Email;
                    StateHasChanged();
                }
            });
            _localStorage.Changed += async (sender, args) =>
            {
                if (args.Key == StorageConstants.Local.AuthToken && User != null)
                {
                    User = (await _stateProvider.GetAuthenticationStateAsync()).User;
                    StateHasChanged();
                }
            };
        }

        private async Task Load()
        {
            _isLoading = true;
            if (string.IsNullOrWhiteSpace(UserId) && UserData == null)
                await LoadCurrentUserData();
            else
                await LoadOtherUserData();
            _isLoading = false;
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
        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}