using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Infrastructure.Extensions;
using CleanArchitectureBase.Client.Infrastructure.Theming;

namespace CleanArchitectureBase.Client.Shared
{
    public partial class MainLayout : IDisposable
    {
        private string CurrentUserId { get; set; }
        
        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();                
                
                var currentUserResult = await _api.User_GetByIdAsync(CurrentUserId);
                if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                {
                    _snackBar.Add(localizer["You are logged out because the user with your Token has been deleted."], Severity.Error);
                    await _clientAuthenticationManager.Logout();
                }

                await hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);
            }
        }

        private ClientTheme _currentTheme = ClientTheme.DefaultTheme;
        private bool _drawerOpen = true;
        private bool _rightToLeft = false;
        private async Task RightToLeftToggle()
        {
            var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
            _rightToLeft = isRtl;
            DrawerToggle();
            DrawerToggle(); // 2 calls to Ensure refresh and old state
        }


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            var currentUser = await _clientAuthenticationManager.CurrentUser();
            if (currentUser is {Identity: {IsAuthenticated: true}})
                _navigationManager.NavigateToReturnUrlIf();
        }

        protected override async Task OnInitializedAsync()
        {
            _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
            _rightToLeft = await _clientPreferenceManager.IsRTL();
            _interceptor.RegisterEvent();
            hubConnection = hubConnection.TryInitialize(_navigationManager);
            await hubConnection.StartAsync();
            hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
            {
                if (CurrentUserId == receiverUserId)
                {
                    _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                    var chatUrlToUser = $"chat/{senderUserId}";
                    if (!_navigationManager.Uri.EndsWith(chatUrlToUser))
                    {
                        _snackBar.Add(message, Severity.Info, config =>
                        {
                            config.VisibleStateDuration = 10000;
                            config.HideTransitionDuration = 500;
                            config.ShowTransitionDuration = 500;
                            config.Action = localizer["Chat?"];
                            config.ActionColor = Color.Primary;
                            config.Onclick = snackbar =>
                            {
                                _navigationManager.NavigateTo(chatUrlToUser);
                                return Task.CompletedTask;
                            };
                        });
                    }
                }
            });
            hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
            {
                try
                {
                    var token = await _clientAuthenticationManager.TryForceRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _snackBar.Add(localizer["Refreshed Token."], Severity.Success);
                        _httpClient.SetAuthorization(token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _snackBar.Add(localizer["You are Logged Out."], Severity.Error);
                    await _clientAuthenticationManager.Logout();
                    _navigationManager.NavigateToHomeWithReturnTo();
                }
            });
            hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
            {
                if (CurrentUserId != userId)
                {
                    var rolesResponse = await _api.Role_GetAllAsync();
                    if (rolesResponse.Succeeded)
                    {
                        var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
                        if (role != null)
                        {
                            var currentUserRolesResponse = await _api.User_GetRolesAsync(CurrentUserId);
                            if (currentUserRolesResponse.Succeeded && currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
                            {
                                _snackBar.Add(localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);
                                await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, CurrentUserId);
                                await _clientAuthenticationManager.Logout();
                                _navigationManager.NavigateToHomeWithReturnTo();
                            }
                        }
                    }
                }
            });
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.Logout.ContentText), $"{localizer["Logout Confirmation"]}"},
                {nameof(Dialogs.Logout.ButtonText), $"{localizer["Logout"]}"},
                {nameof(Dialogs.Logout.Color), Color.Error},
                {nameof(Dialogs.Logout.CurrentUserId), CurrentUserId},
                {nameof(Dialogs.Logout.HubConnection), hubConnection}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

             _dialogService.Show<Dialogs.Logout>(localizer["Logout"], parameters, options);
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private void ThemeChanged(ClientTheme theme)
        {
            var updateDrawer = theme.LayoutProperties.DrawerVariant == DrawerVariant.Temporary || _currentTheme.LayoutProperties.DrawerVariant == DrawerVariant.Temporary;
            _currentTheme = theme;
            if (updateDrawer)
                _drawerOpen = theme.LayoutProperties.DrawerVariant != DrawerVariant.Temporary;
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
            //_ = hubConnection.DisposeAsync();
        }

        private HubConnection hubConnection;
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
    }
}