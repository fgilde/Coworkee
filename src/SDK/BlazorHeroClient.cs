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

    }
}