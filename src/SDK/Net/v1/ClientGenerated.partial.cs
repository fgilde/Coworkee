using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Coworkee.SDK;
using Coworkee.Shared.Constants.Application;
using Nextended.Core.Extensions;

namespace SDK
{
    public partial class GeneratedClient : ClientBase
    {

        public string SwaggerUrl => new UriBuilder(BaseUrl) { Path = ApplicationConstants.Routes.ApiDocumentation.EnsureStartsWith("/") }.ToString();

        public delegate void PrepareRequestDelegate(HttpClient client, HttpRequestMessage request, string url);

        public delegate void ProcessResponseDelegate(HttpClient client, HttpResponseMessage response);

        public HttpClient GetHttpClient() => _httpClient;

        /// <summary>
        /// Delegate function to catch prepare request event
        /// </summary>
        public PrepareRequestDelegate PrepareRequestDelegateFunction { get; set; } = null;


        /// <summary>
        /// Delegate function to catch process response event
        /// </summary>
        public ProcessResponseDelegate ProcessResponseDelegateFunction { get; set; } = null;

        /// <summary>
        /// Dispatch delegate prepare request.
        /// Called by Swagger implementation.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="url"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request,
                string url)
        {
            PrepareRequestDelegateFunction?.Invoke(client, request, url);
        }

        /// <summary>
        /// Dispatch delegate process response.
        /// Called by Swagger implementation.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="url"></param>
        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
        {
            ProcessResponseDelegateFunction?.Invoke(client, response);
        }
    }
}