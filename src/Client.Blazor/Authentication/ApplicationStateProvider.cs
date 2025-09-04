using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Coworkee.Client.Configuration;
using Coworkee.Client.Extensions;
using Coworkee.Shared;
using Coworkee.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Services;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Coworkee.Client.Authentication
{
    public class ApplicationStateProvider : AuthenticationStateProvider, ICurrentUserService
    {
        private static bool _generalAuthenticatedBeforeEvent; // Just a hack to have correct login state in app razor. before redirecting to login.

        private readonly HttpClient _httpClient;
        private readonly ClientApplicationConfiguration _config;
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorage;

        public bool IsAuthenticatedBeforeEvent => _generalAuthenticatedBeforeEvent; 

        public ApplicationStateProvider(
            HttpClient httpClient,
            ClientApplicationConfiguration config,
            IJSRuntime jsRuntime,
            NavigationManager navigationManager,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _config = config;
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
            _localStorage = localStorage;
        }
        
        public void MarkUserAsAuthenticated()
        {
            _generalAuthenticatedBeforeEvent = true;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            _generalAuthenticatedBeforeEvent = false;
            NotifyAuthenticationStateChanged(Task.FromResult(GetAnonymousState()));
        }

        public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
        {
            var state = await this.GetAuthenticationStateAsync();
            var authenticationStateProviderUser = state.User;
            return authenticationStateProviderUser;
        }

        public ClaimsPrincipal AuthenticationStateUser { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Read from cookie?    
            var token = _navigationManager.ReadQueryParam(ApplicationConstants.ParameterNames.AuthedUrlParameter);
            if(token != null)
            {
                await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
                // TODO: remove from url
            }

            var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            if (string.IsNullOrWhiteSpace(savedToken))
                return GetAnonymousState();

            _httpClient.SetAuthorization(savedToken);
            var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ClaimReader.ReadClaimsFromJwt(savedToken), "jwt")));
            AuthenticationStateUser = state.User;
            return state;
        }

        private AuthenticationState GetAnonymousState()
        {
            var state = _config.AllowAnonymousPageAccess ? AuthenticationStates.Guest : AuthenticationStates.None;
            AuthenticationStateUser = state.User;
            return state;
        }

        #region Implementation ICurrentUserService

        string ICurrentUserService.UserId => AuthenticationStateUser?.GetUserId();

        string[] ICurrentUserService.RoleIds => ServiceAccessor.Get<HttpClient>().DefaultRequestHeaders.GetValues(ApplicationConstants.HeaderNames.RoleIdHeader).SelectMany(s => s.Split(",")).ToArray();

        List<KeyValuePair<string, string>> ICurrentUserService.Claims => AuthenticationStateUser.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList();

        ClaimsPrincipal ICurrentUserService.Principal => AuthenticationStateUser;

        UserResponse ICurrentUserService.CurrentUser()
        {
            throw new NotImplementedException();
        }

        Task<IDisposable> ICurrentUserService.AsSystemUser()
        {
            throw new NotImplementedException();
        }

        Task<IDisposable> ICurrentUserService.AsUser(string userId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
