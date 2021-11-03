using System.Net.Http;
using SDK;

namespace CleanArchitectureBase.SDK
{
    public class BlazorHeroClient: GeneratedClient, IBlazorHeroClient
    {
        public BlazorHeroClient(string baseUrl, HttpClient httpClient) : base(baseUrl, httpClient)
        {}
    }

    public interface IBlazorHeroClient : IGeneratedClient
    {
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