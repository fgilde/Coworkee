using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Extensions;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Extensions
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

        public static string[] GetActiveRoleIds(this HttpHeaders headers) => headers.AllHeaderValuesFor(ApplicationConstants.HeaderNames.RoleIdHeader);
        public static string[] GetActiveRoleIds(this HttpClient client) => client.DefaultRequestHeaders.GetActiveRoleIds();
        public static HttpHeaders SetActiveRoleIds(this HttpHeaders headers, params string[] roleIds) => headers.SetHeaderValues(ApplicationConstants.HeaderNames.RoleIdHeader, roleIds);
        public static void UpdateAcceptLanguage(this HttpClient client, CultureInfo cultureInfo = null) => client.DefaultRequestHeaders.UpdateAcceptLanguage(cultureInfo);
        public static void SetActiveRoleIds(this HttpClient client, params string[] roleIds) => client.DefaultRequestHeaders.SetActiveRoleIds(roleIds);
        public static void SetActiveRoleIds(this HttpRequestMessage request, string roleIds) => request.Headers.SetActiveRoleIds(roleIds);
        public static void SetAuthorization(this HttpClient httpClient, string token) => httpClient.DefaultRequestHeaders.SetAuthorization(token);
        public static string GetAuthorization(this HttpClient httpClient) => httpClient.DefaultRequestHeaders.Authorization?.Parameter;
        public static void SetAuthorization(this HttpRequestMessage request, string token) => request.Headers.SetAuthorization(token);

        internal static string[] AllHeaderValuesFor(this HttpHeaders headers, string headerName)
            => headers.Contains(headerName) ? headers.GetValues(headerName).ToArray() : Array.Empty<string>();

        internal static HttpHeaders SetHeaderValues(this HttpHeaders headers, string headerName, string[] values)
        {
            if (headers.Contains(headerName))
                headers.Remove(headerName);
            foreach (var value in values.EmptyIfNull())
                headers.Add(headerName, value);
            return headers;
        }
        
        private static AuthenticationHeaderValue GetAuth(string token)
        {
            return token.IsNullOrWhiteSpace() ? null : new AuthenticationHeaderValue("Bearer", token);
        }
    }
}