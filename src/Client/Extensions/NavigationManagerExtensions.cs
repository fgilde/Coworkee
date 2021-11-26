using System;
using System.Linq;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        private static readonly string[] ForbiddenReturnUrls = {
            ApplicationConstants.Routes.Login,
            ApplicationConstants.Routes.Register,
            ApplicationConstants.Routes.Forbidden
        };

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
                navigationManager.NavigateTo(uri, NeedReload(uri));
            else if (!string.IsNullOrEmpty(fallback))
                navigationManager.NavigateTo(fallback, NeedReload(fallback));
        }

        private static bool NeedReload(string url)
        {
            return url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase) || url.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}