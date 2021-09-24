using CleanArchitectureBase.Client.Infrastructure.Managers.Identity.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Toolbelt.Blazor;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Interceptors
{
    public class HttpInterceptorManager : IHttpInterceptorManager
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly IClientAuthenticationManager _clientAuthenticationManager;
        private readonly NavigationManager _navigationManager;
        private readonly ISnackbar _snackBar;
        private readonly IStringLocalizer<HttpInterceptorManager> _localizer;

        public HttpInterceptorManager(
            HttpClientInterceptor interceptor,
            IClientAuthenticationManager clientAuthenticationManager,
            NavigationManager navigationManager,
            ISnackbar snackBar,
            IStringLocalizer<HttpInterceptorManager> localizer)
        {
            _interceptor = interceptor;
            _clientAuthenticationManager = clientAuthenticationManager;
            _navigationManager = navigationManager;
            _snackBar = snackBar;
            _localizer = localizer;
        }

        public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request.RequestUri.AbsolutePath;
            if (!absPath.Contains("token") && !absPath.Contains("accounts"))
            {
                try
                {
                    var token = await _clientAuthenticationManager.TryRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
                        e.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
                    await _clientAuthenticationManager.Logout();
                    _navigationManager.NavigateTo("/");
                }
            }
        }

        public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
    }
}