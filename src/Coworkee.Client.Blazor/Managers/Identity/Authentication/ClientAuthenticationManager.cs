using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Requests.Identity;
using Coworkee.Client.Authentication;
using Coworkee.Client.Extensions;
using Coworkee.Client.Managers.Preferences;
using Coworkee.SDK;
using Coworkee.Shared.Constants.Storage;
using Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Coworkee.Client.Managers.Identity.Authentication
{
    public class ClientAuthenticationManager : IClientAuthenticationManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ApplicationStateProvider _authenticationStateProvider;
        private readonly IStringLocalizer<ClientAuthenticationManager> _localizer;
        private readonly IApplicationClient _api;

        public ClientAuthenticationManager(
            IServiceProvider serviceProvider,
            HttpClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider,
            IStringLocalizer<ClientAuthenticationManager> localizer,
            IApplicationClient api)
        {
            _serviceProvider = serviceProvider;
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider as ApplicationStateProvider;
            _localizer = localizer;
            _api = api;

        }

        public async Task<ClaimsPrincipal> CurrentUser()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User;
        }

        public async Task<IResult> Login(TokenRequest model)
        {
            var result = await _api.Token_GetAsync(model);
            if (result.Succeeded)
            {
                await UpdateToken(result.Data);
                return await Result.SuccessAsync();
            }

            return await Result.FailAsync(result.Messages);
        }

        public async Task<IResult> RegenerateAndUpdateTokenAsync()
        {
            var res = await _api.Token_RegenerateNewAsync();
            if (res.Succeeded)
                await UpdateToken(res.Data);
            return res;
        }

        public async Task UpdateToken(TokenResponse response)
        {
            var token = response.Token;
            var refreshToken = response.RefreshToken;
            var userImageUrl = response.UserImageURL;
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
            if (!string.IsNullOrEmpty(userImageUrl))
                await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, userImageUrl);

            _httpClient.SetAuthorization(token);
            _authenticationStateProvider.MarkUserAsAuthenticated();
        }

        public async Task<IResult> Logout()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, string.Empty);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, string.Empty);
            await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, string.Empty);
            await _serviceProvider.GetService<IClientPreferenceManager>()?.SetActiveSelectedRolesAsync(Array.Empty<UserRoleModel>())!;
            try
            {
                if (state?.User.Identity?.IsAuthenticated == true && !state.IsGuest())
                    await _api.Account_LogoutAsync();
            }
            catch { /* ignored*/ }

            _httpClient.SetAuthorization(null);
            _httpClient.SetActiveRoleIds(null);
            _authenticationStateProvider.MarkUserAsLoggedOut();

            return await Result.SuccessAsync();
        }

        public async Task<string> RefreshToken()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
                var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

                var result = await _api.Token_RefreshAsync(new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });

                if (!result.Succeeded)
                {
                    throw new ApplicationException(_localizer["Something went wrong during the refresh token action"]);
                }

                token = result.Data.Token;
                refreshToken = result.Data.RefreshToken;
                await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
                await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
                _httpClient.SetAuthorization(token);
                return token;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> TryRefreshToken()
        {
            //check if token exists
            var availableToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
            if (string.IsNullOrEmpty(availableToken)) return string.Empty;
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 1)
                return await RefreshToken();
            return string.Empty;
        }

        public async Task<string> TryForceRefreshToken()
        {
            return await RefreshToken();
        }
    }
}