using System;
using System.Linq;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace CleanArchitectureBase.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        private static readonly string[] ForbiddenReturnUrls = {"login", "register"};

        private static bool IsForbidden(string url)
        {
            return !string.IsNullOrWhiteSpace(url) && ForbiddenReturnUrls.Contains(url.Replace("/", ""), StringComparer.InvariantCultureIgnoreCase);
        }

        private static string CleanReturnUrl(string url)
        {
            var returnUrlParamName = ApplicationConstants.ParameterNames.ReturnUrl;
            return !string.IsNullOrWhiteSpace(url) ? url.Replace($"?{returnUrlParamName}=", "").Replace($"{returnUrlParamName}=", "") : "/";
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
            returnUrl ??= navigationManager.ToBaseRelativePath(navigationManager.Uri);
            returnUrl = CleanReturnUrl(returnUrl);
            navigationManager.NavigateTo(string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/" || IsForbidden(returnUrl)
                ? "/"
                : $"/?{ApplicationConstants.ParameterNames.ReturnUrl}=" + returnUrl);
        }

        public static void NavigateToReturnUrlIf(this NavigationManager navigationManager, string fallback = null)
        {
            var uri = navigationManager.GetReturnUrlValue();
            if (!string.IsNullOrEmpty(uri))
                navigationManager.NavigateTo(uri, true);
            else if (!string.IsNullOrEmpty(fallback))
                navigationManager.NavigateTo(fallback, true);
        }
    }
}