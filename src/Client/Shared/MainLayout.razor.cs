using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Models.Chat;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Chat;
using CleanArchitectureBase.Application.Contracts.Hubs;
using CleanArchitectureBase.Application.Hubs;
using CleanArchitectureBase.Application.Hubs.Events;
using CleanArchitectureBase.Client.Localization;
using CleanArchitectureBase.Client.Shared.Components;
using CleanArchitectureBase.Client.Theming;
using Microsoft.AspNetCore.Components;
using CleanArchitectureBase.Shared.Wrapper;
using MudBlazor.Extensions.Options;

namespace CleanArchitectureBase.Client.Shared
{
    public partial class MainLayout : IAsyncDisposable
    {
        private string CurrentUserId { get; set; }
        private NavMenu navMenu;
        private MudDrawer drawer;
        private AppBarHeader appBarHeader;
        private ClaimsPrincipal currentUser;

        [CascadingParameter]
        private HubConnection hubConnection { get; set; }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user.Identity?.IsAuthenticated == true && !user.IsGuest())
            {
                CurrentUserId = user.GetUserId();
                var currentUserResult = Result<UserResponse>.Fail();
                try
                {
                    currentUserResult = await _api.User_GetByIdAsync(CurrentUserId);
                }
                catch { /* ignored*/ }
                if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                {
                    _snackBar.Add(localizer["You are logged out because the user with your Token has been deleted or token is expired."], Severity.Error);
                    await _clientAuthenticationManager.Logout();
                    _navigationManager.NavigateToWithReturnTo("login");
                    return;
                }

                await hubConnection.SendAsync(nameof(ClientEventHub.OnConnectAsync), CurrentUserId);
            }
        }

        private ClientTheme _currentTheme = ClientTheme.DefaultTheme;
        private bool _drawerOpen = true;
        private bool _rightToLeft;
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
            currentUser = await _clientAuthenticationManager.CurrentUser();
        }

        protected override async Task OnInitializedAsync()
        {
            _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
            _rightToLeft = await _clientPreferenceManager.IsRTL();
            hubConnection = await hubConnection.EnsureStartedAsync(_config.BackendOrigin);
            
            hubConnection.On<EntitiesUpdated<TranslationDto>>(async (arg) =>
            {
                _snackBar.Add(localizer["Translations updated"], Severity.Normal, options => options.Icon = Icons.Filled.Translate);
                await ApiResources.UpdateEntries(_api, CultureInfo.DefaultThreadCurrentCulture, true);
                await _clientPreferenceManager.ChangeLanguageAsync((await _clientPreferenceManager.GetPreference()).LanguageCode);
                StateHasChanged();
            });

            hubConnection.On<ChatHistory<IChatUser>, string>(nameof(IClientEventHub.ReceiveMessage), async (chatHistory, userName) =>
            {
                await _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                var chatUrlToUser = $"chat/{chatHistory.FromUserId}";
                if (!_navigationManager.Uri.EndsWith(chatUrlToUser))
                {
                    _snackBar.Add(chatHistory.Message + " from "+ userName, Severity.Info, config =>
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
            });

            hubConnection.On(nameof(IClientEventHub.RegenerateTokens), async () =>
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
            hubConnection.On<string, string>(nameof(IClientEventHub.LogoutUsersByRole), async (userId, roleId) =>
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
                                await hubConnection.SendAsync(nameof(ClientEventHub.OnDisconnectAsync), CurrentUserId);
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

        private async void ShowAbout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.About.ClientInfo), new VersionInfoModel { ApplicationName = ApplicationConstants.ApplicationClientName }},
                {nameof(Dialogs.About.ServerInfo), await _api.System_VersionAsync()},
                {nameof(Dialogs.About.ApiVersions),  await _api.System_AvailableApiVersionsAsync()},
            };
            await _dialogService.ShowWithDefaultOptionsAsync<Dialogs.About>(localizer["about"], parameters, o => {
                o.MaxWidth = MaxWidth.Small;
                o.DisableBackdropClick = false;
                o.MaximizeButton = false;
                o.Animation = AnimationType.SlideIn;
            });
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

        public ValueTask DisposeAsync()
        {
            return hubConnection.TryDisposeAsync();
        }

        private string GetTitle()
        {
            var menuItem = navMenu?.FindEntriesForUrl()?.FirstOrDefault();
            var res = navMenu?.Locale(menuItem?.Parent?.Text ?? menuItem?.Text ?? appBarHeader?.Title);
            if (string.IsNullOrEmpty(res))
                return _navigationManager.ToBaseRelativePath(_navigationManager.Uri).Split("/").FirstOrDefault()?.ToUpper() ?? "404";
            return res;
        }
    }
}