using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Client.Authentication;
using Coworkee.Client.Configuration;
using Coworkee.Client.JsInterop;
using Coworkee.SDK;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Nextended.Core.Extensions;

namespace Coworkee.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        private static readonly string[] ForbiddenReturnUrls = {
            ApplicationConstants.Routes.Login,
            ApplicationConstants.Routes.Register,
            ApplicationConstants.Routes.Forbidden
        };

        public static string ToAbsoluteServerUri(this NavigationManager navigationManager, string url)
        {
            if (!string.IsNullOrWhiteSpace(url) && !url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) && !url.StartsWith("blob:", StringComparison.InvariantCultureIgnoreCase) && !url.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    var config = ServiceAccessor.Get<ClientApplicationConfiguration>();
                    return new UriBuilder(config.BackendOrigin).SetProperties(b => b.Path = url).Uri.AbsoluteUri;
                }
                catch (Exception)
                {
                    return url;
                }
            }
            return url;
        }

        public static Task<string> EnsureUrlIsAccessible(this NavigationManager navigationManager, ClaimsPrincipal user, string url)
        {
            if (!navigationManager.ShouldBeAuthorized(url) || navigationManager.UriContainsAuth(url))
                return Task.FromResult(url);
            if (user?.Identity?.IsAuthenticated == true && !user.IsGuest())
                return ServiceAccessor.Get<IApplicationClient>().System_AuthorizeServerUrlAsync(url);

            return Task.FromResult(url);
        }

        public static bool UriContainsAuth(this NavigationManager navigationManager, string url)
        {
            return !string.IsNullOrEmpty(navigationManager.ReadQueryParam(ApplicationConstants.ParameterNames.AuthedUrlParameter, url));
        }

        /// <summary>
        /// Returns true if the given url is on our backend and our Backend isn't hosting the client
        /// </summary>
        public static bool ShouldBeAuthorized(this NavigationManager navigationManager, string url)
        {
            if (!url.ToLower().StartsWith("http") || !Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
            var config = ServiceAccessor.Get<ClientApplicationConfiguration>();
            var clientOrigin = navigationManager.ToAbsoluteUri(navigationManager.BaseUri);
            return Uri.TryCreate(config.BackendOrigin, UriKind.RelativeOrAbsolute, out var serverOrigin) 
                   && uri.Origin() == serverOrigin.Origin() 
                   && clientOrigin.Origin() != serverOrigin.Origin()
                   && ApplicationConstants.Routes.IsAuthRequired(uri.LocalPath);
        }

        /// <summary>
        /// Checks if url needs opened in a new tab
        /// </summary>
        public static NavigationManager NavigateToUnknown(this NavigationManager navigationManager, string url)
        {
            if (!navigationManager.IsExternalUrl(url))
                navigationManager.NavigateTo(url);
            else
                _= ServiceAccessor.Get<IJSRuntime>().InvokeVoidAsync(JsNamespace.Get("BrowserHelper", "navigateToExternalUrl"), url);
            return navigationManager;
        }

        public static string ReadQueryParam(this NavigationManager navigationManager, string paramName, string url = null)
        {
            var uri = string.IsNullOrEmpty(url) ? navigationManager.ToAbsoluteUri(navigationManager.Uri) : new Uri(url, UriKind.RelativeOrAbsolute);
            return QueryHelpers.ParseQuery(uri.Query).TryGetValue(paramName, out var param) ? param.FirstOrDefault() : null;
        }

        public static bool IsExternalUrl(this NavigationManager navigationManager, string url)
        {
            var baseUrl = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            var target = new Uri(url, UriKind.RelativeOrAbsolute);
            return target.IsAbsoluteUri && target.Host != baseUrl.Host;
        }

        public static void Reload(this NavigationManager navigationManager, bool forceLoad = false)
        {
            navigationManager.NavigateTo(navigationManager.Uri, forceLoad);
        }

        public static string GetReturnUrlValue(this NavigationManager navigationManager)
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(ApplicationConstants.ParameterNames.ReturnUrl, out var param))
            {
                var url = param.First();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    string result = CleanReturnUrl(url);
                    return !IsForbidden(result) ? result : null;
                }
            }

            return null;
        }

        public static void NavigateToHome(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo("/");
        }

        public static void NavigateToHomeWithReturnTo(this NavigationManager navigationManager, string returnUrl = null)
        {
            navigationManager.NavigateToWithReturnTo("/", returnUrl);
        }

        public static void NavigateToWithReturnTo(this NavigationManager navigationManager, string url, string returnUrl = null)
        {
            returnUrl ??= navigationManager.ToBaseRelativePath(navigationManager.Uri);
            returnUrl = CleanReturnUrl(returnUrl);
            navigationManager.NavigateTo(string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/" || IsForbidden(returnUrl)
                ? url
                : $"{url}?{ApplicationConstants.ParameterNames.ReturnUrl}=" + returnUrl);
        }

        public static void NavigateToReturnUrlIf(this NavigationManager navigationManager, string fallback = null)
        {
            var uri = navigationManager.GetReturnUrlValue();
            if (!string.IsNullOrEmpty(uri))
            {
                if (navigationManager.ShouldBeAuthorized(uri))
                {
                    ServiceAccessor.Get<ApplicationStateProvider>().GetAuthenticationStateProviderUserAsync().ContinueWith(
                        u => navigationManager.EnsureUrlIsAccessible(u.Result, uri).ContinueWith(
                            t => navigationManager.NavigateTo(t.Result, NeedReload(t.Result))));
                }
                else
                {
                    navigationManager.NavigateTo(uri, NeedReload(uri));
                }
            }
            else if (!string.IsNullOrEmpty(fallback))
                navigationManager.NavigateTo(fallback, NeedReload(fallback));
        }

        public static void GoBack(this NavigationManager navigationManager) => ServiceAccessor.Get<IJSRuntime>().InvokeVoidAsync("window.history.back");
        public static void GoForward(this NavigationManager navigationManager) => ServiceAccessor.Get<IJSRuntime>().InvokeVoidAsync("window.history.forward");

        public static Task<T> NavigateToNotFoundOnError<T>(this NavigationManager navigationManager, Func<Task<T>> action)
            => navigationManager.NavigateToOnError("/notfound", action);

        public static async Task<T> NavigateToOnError<T>(this NavigationManager navigationManager, string url, Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception)
            {
                navigationManager.NavigateTo(url);
                return default;
            }
        }

        private static bool IsForbidden(string url)
        {
            return !string.IsNullOrWhiteSpace(url) && ForbiddenReturnUrls.Contains(url.Replace("/", ""), StringComparer.InvariantCultureIgnoreCase);
        }

        private static string CleanReturnUrl(string url)
        {
            var returnUrlParamName = ApplicationConstants.ParameterNames.ReturnUrl;
            var r = !string.IsNullOrWhiteSpace(url) ? url.Replace($"?{returnUrlParamName}=", "").Replace($"{returnUrlParamName}=", "") : "/";
            ForbiddenReturnUrls.Where(u => r.StartsWith(u, StringComparison.InvariantCultureIgnoreCase)).Apply(u => r = r.Substring(u.Length));
            return r;
        }

        private static bool NeedReload(string url)
        {
            return url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase) || url.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}