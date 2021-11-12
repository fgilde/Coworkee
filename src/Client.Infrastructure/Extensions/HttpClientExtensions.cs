using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Extensions;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {

        public static HttpRequestHeaders UpdateAcceptLanguage(this HttpRequestHeaders headers, CultureInfo cultureInfo = null)
        {
            if (cultureInfo != null)
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            headers.AcceptLanguage.Clear();
            headers.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture.AcceptHeaderCode());
            return headers;
        }

        public static HttpRequestHeaders SetAuthorization(this HttpRequestHeaders headers, string token)
        {
            headers.Authorization = GetAuth(token);
            return headers;
        }

        public static HttpRequestHeaders SetActiveRoleId(this HttpRequestHeaders headers, params string[] roleIds)
        {
            if (headers.Contains(ApplicationConstants.HeaderNames.RoleIdHeader))
                headers.Remove(ApplicationConstants.HeaderNames.RoleIdHeader);
            foreach (var roleId in roleIds.EmptyIfNull())
                headers.Add(ApplicationConstants.HeaderNames.RoleIdHeader, RoleIdHeaderValue(roleId));
            return headers;
        }

        public static void UpdateAcceptLanguage(this HttpClient client, CultureInfo cultureInfo = null) => client.DefaultRequestHeaders.UpdateAcceptLanguage(cultureInfo);
        public static void SetActiveRoleId(this HttpClient client, params string[] roleIds) => client.DefaultRequestHeaders.SetActiveRoleId(roleIds);
        public static void SetActiveRoleId(this HttpRequestMessage request, string roleId) => request.Headers.SetActiveRoleId(roleId);
        public static void SetAuthorization(this HttpClient httpClient, string token) => httpClient.DefaultRequestHeaders.SetAuthorization(token);
        public static void SetAuthorization(this HttpRequestMessage request, string token) => request.Headers.SetAuthorization(token);

        private static string RoleIdHeaderValue(string roleId)
        {
            return roleId;
        }

        private static AuthenticationHeaderValue GetAuth(string token)
        {
            return token.IsNullOrWhiteSpace() ? null : new AuthenticationHeaderValue("Bearer", token);
        }
    }
}