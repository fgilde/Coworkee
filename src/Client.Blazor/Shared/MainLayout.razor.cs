using Coworkee.Client.Extensions;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Models.Chat;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Chat;
using Coworkee.Application.Contracts.Hubs;
using Coworkee.Application.Hubs;
using Coworkee.Application.Hubs.Events;
using Coworkee.Client.Authentication;
using Coworkee.Client.JsInterop;
using Coworkee.Client.Localization;
using Coworkee.Client.Shared.Components;
using Coworkee.Client.Theming;
using Microsoft.AspNetCore.Components;
using Coworkee.Shared.Wrapper;
using MudBlazor.Extensions.Helper;

namespace Coworkee.Client.Shared
{
    public partial class MainLayout : IAsyncDisposable
    {
        internal static MainLayout Instance { get; private set; }
        internal string CurrentUserId { get; set; }
        internal MainNavMenuDrawer NavMenuDrawer;
        internal ClaimsPrincipal CurrentUser;
        internal ClientTheme CurrentTheme = ClientThemes.DefaultTheme;
        
        internal bool IsDrawerOpen;
        internal bool IsDarkMode;
        internal bool RightToLeft;

        [CascadingParameter] private HubConnection hubConnection { get; set; }



        protected override async Task OnInitializedAsync()
        {
            if(CurrentTheme?.PreferDarkMode.HasValue == true)
                IsDarkMode = CurrentTheme.PreferDarkMode.Value;
            //@inject IOptionsSnapshot<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>> Options
            //var remoteAuthenticationOptions = Options.Get(Microsoft.Extensions.Options.Options.DefaultName);
            //_navigationManager.NavigateToLogin(remoteAuthenticationOptions.AuthenticationPaths.LogInPath);
            Instance = this;
            await base.OnInitializedAsync();
            if (await LoadDataAsync())
            {
                await Task.WhenAll(
                    ApplyFromPreferenceAsync(),
                    InitializeMudBlazorExtensions(),
                    StartHubConnection());
            }
        }

        private Task InitializeMudBlazorExtensions() => _jsRuntime.InitializeMudBlazorExtensionsAsync();

        private async Task ApplyFromPreferenceAsync()
        {
            var preference = await _clientPreferenceManager.GetPreference();
            CurrentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
            IsDarkMode = preference.DarkMode ?? await _themeManager.BrowserPrefersDarkMode();
            RightToLeft = preference.IsRTL;
            IsDrawerOpen = preference.IsDrawerOpen;
        }

        private async Task StartHubConnection()
        {
            hubConnection = await hubConnection.EnsureStartedAsync(_config.BackendOrigin);
            SetHubConnectionHandlers();
            await hubConnection.SendAsync(nameof(ClientEventHub.OnConnectAsync), CurrentUserId);
        }

        private void SetHubConnectionHandlers()
        {
            hubConnection.On<EntitiesUpdated<TranslationDto>>(async (arg) =>
            {
                _snackBar.Add(localizer["Translations updated"], Severity.Normal,
                    options => options.Icon = Icons.Material.Filled.Translate);
                await ApiResources.UpdateEntries(_api, CultureInfo.DefaultThreadCurrentCulture, true);
                await _clientPreferenceManager.ChangeLanguageAsync((await _clientPreferenceManager.GetPreference())
                    .LanguageCode);
                StateHasChanged();
            });

            hubConnection.On<ChatHistory<IChatUser>, string>(nameof(IClientEventHub.ReceiveMessage),
                async (chatHistory, userName) =>
                {
                    await _jsRuntime.InvokeAsync<string>(JsNamespace.Get("BrowserHelper", "playAudio"), "notification");
                    var chatUrlToUser = $"chat/{chatHistory.FromUserId}";
                    if (!_navigationManager.Uri.EndsWith(chatUrlToUser))
                    {
                        _snackBar.Add(chatHistory.Message + " from " + userName, Severity.Info, config =>
                        {
                            config.VisibleStateDuration = 10000;
                            config.HideTransitionDuration = 500;
                            config.ShowTransitionDuration = 500;
                            config.Action = localizer["Chat?"];
                            config.ActionColor = Color.Primary;
                            config.OnClick = snackbar =>
                            {
                                _navigationManager.NavigateTo(chatUrlToUser);
                                return Task.CompletedTask;
                            };
                        });
                    }
                });

            hubConnection.On(nameof(IClientEventHub.RegenerateTokens),
                async () => { await _clientAuthenticationManager.RegenerateAndUpdateTokenAsync(); });
            hubConnection.On<string>(nameof(IClientEventHub.UserRolesChanged), async (userId) =>
            {
                if (CurrentUserId == userId)
                    await _clientAuthenticationManager.RegenerateAndUpdateTokenAsync();
            });
            hubConnection.On<string>(nameof(IClientEventHub.LogoutUserById), async (userId) =>
            {
                if (CurrentUserId == userId)
                {
                    await hubConnection.TrySendAsync(_config.BackendOrigin, nameof(ClientEventHub.OnDisconnectAsync), CurrentUserId);
                    await _clientAuthenticationManager.Logout();
                    if ((await _stateProvider.GetAuthenticationStateAsync()).IsGuest())
                        _navigationManager.NavigateToHomeWithReturnTo();
                    else
                        _navigationManager.NavigateToWithReturnTo(ApplicationConstants.Routes.Login);
                    _snackBar.Add(localizer["You are logged out by an Administrator or profile change."],
                        Severity.Error);
                }
            });
        }



        private async Task<bool> LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user.Identity?.IsAuthenticated == true && !user.IsGuest())
            {
                CurrentUserId = user.GetUserId();
                var currentUserResult = Result<UserResponse>.Fail();
                try { currentUserResult = await _api.User_GetByIdAsync(CurrentUserId); }
                catch { }

                if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                {
                    _snackBar.Add(localizer["You are logged out because the user with your Token has been deleted or token is expired."], Severity.Error);
                    await _clientAuthenticationManager.Logout();
                    _navigationManager.NavigateToWithReturnTo(ApplicationConstants.Routes.Login);
                    return false;
                }
            }

            return true;
        }
        
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            CurrentUser = await _clientAuthenticationManager.CurrentUser();
        }
        

        internal void DrawerToggle()
        {
            IsDrawerOpen = !IsDrawerOpen;
            _= _clientPreferenceManager.SetPreference(p => p.IsDrawerOpen = IsDrawerOpen);
        }


        public ValueTask DisposeAsync()
        {
            return hubConnection.TryDisposeAsync();
        }

        private async Task OnPreferenceChanged(ReactOnPreferenceChanged.PreferenceChangedArgs arg)
        {
            if (arg.NewValue.ThemeName != arg?.OldValue?.ThemeName)
            {
                CurrentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
            }
            IsDarkMode = arg.NewValue.DarkMode ?? IsDarkMode;
            if (RightToLeft != arg.NewValue.IsRTL)
            {
                RightToLeft = arg.NewValue.IsRTL;
                DrawerToggle(); DrawerToggle(); // 2 calls to Ensure refresh and old state
            }
        }
    }
}