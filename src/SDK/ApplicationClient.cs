using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Coworkee.SDK.Net.Extensions;
using SDK;
using System.Threading;

namespace Coworkee.SDK
{
    public class ApplicationClient: GeneratedClient, IApplicationClient
    {
        public ApplicationClient(string baseUrl, HttpClient httpClient) : base(baseUrl, httpClient)
        {}
        
        public async IAsyncEnumerable<string> AskAssistantAsync(string question, int chunkSize = 8192, [EnumeratorCancellation] CancellationToken token = default)
        {
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/Assistant/Ask");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(urlBuilder.ToString()),
                Content = JsonContent.Create(new { prompt = question }),
            };
            var response = await GetHttpClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            await foreach (string chunk in response.Content.ReadAsStreamAsync(token).AsChunkedUtf8Strings(chunkSize).WithCancellation(token))
            {
                yield return chunk;
            }
        }

        public async Task AskAssistantAsync(string question, Action<string> action, int chunkSize = 8192, CancellationToken token = default)
        {
            await foreach (var chunk in AskAssistantAsync(question, chunkSize, token))
            {
                action(chunk);
            }
        }

    }

    public interface IApplicationClient : IGeneratedClient
    {
        string BaseUrl { get; }

        string SwaggerUrl { get; }
        IAsyncEnumerable<string> AskAssistantAsync(string question, int chunkSize = 8192, CancellationToken token = default);
        Task AskAssistantAsync(string question, Action<string> action, int chunkSize = 8192, CancellationToken token = default);

        /// <summary>
        /// Delegate function to catch prepare request event
        /// </summary>
        GeneratedClient.PrepareRequestDelegate PrepareRequestDelegateFunction { get; set; }

        /// <summary>
        /// Delegate function to catch process response event
        /// </summary>
        GeneratedClient.ProcessResponseDelegate ProcessResponseDelegateFunction { get; set; }
    }
}