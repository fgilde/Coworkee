using System;
using System.Threading.Tasks;
using lib.Coworkee.Client.ErrorHandling;
using lib.Coworkee.Client.Extensions;
using lib.Coworkee.Client.Managers.Identity.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Toolbelt.Blazor;

namespace lib.Coworkee.Client.Managers.Interceptors
{
    public class HttpInterceptorManager : IHttpInterceptorManager
    {
        private readonly IErrorHandler _errorHandler;
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
            IStringLocalizer<HttpInterceptorManager> localizer, IErrorHandler errorHandler)
        {
            _interceptor = interceptor;
            _clientAuthenticationManager = clientAuthenticationManager;
            _navigationManager = navigationManager;
            _snackBar = snackBar;
            _localizer = localizer;
            _errorHandler = errorHandler;
        }

        public void RegisterEvent()
        {
            _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
            _interceptor.AfterSendAsync += InterceptAfterHttpAsync;
        }

        private async Task InterceptAfterHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            if (e?.Response?.IsSuccessStatusCode == false)
            {
                e.Cancel = true;
                await _errorHandler.HandleAsync(e.Response);
            }
        }

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
                        e.Request.SetAuthorization(token);
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

        public void DisposeEvent()
        {
            _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
            _interceptor.AfterSendAsync -= InterceptAfterHttpAsync;
        }
    }
}