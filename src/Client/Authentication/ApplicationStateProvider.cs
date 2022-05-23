using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared;
using CleanArchitectureBase.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitectureBase.Client.Authentication
{
    public class ApplicationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ClientApplicationConfiguration _config;
        private readonly ILocalStorageService _localStorage;

        public ApplicationStateProvider(
            HttpClient httpClient,
            ClientApplicationConfiguration config,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _config = config;
            _localStorage = localStorage;
        }

        public void MarkUserAsAuthenticated()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
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
            return _config.AllowAnonymousPageAccess ? AuthenticationStates.Guest : AuthenticationStates.None;
        }
        
    }
}