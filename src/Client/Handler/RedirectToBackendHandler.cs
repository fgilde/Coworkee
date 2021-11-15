using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Shared.Constants.Storage;

namespace CleanArchitectureBase.Client.Handler
{
    public class RedirectToBackendHandler : DelegatingHandler
    {
        private readonly ClientApplicationConfiguration config;

        public RedirectToBackendHandler(ClientApplicationConfiguration config)
            => this.config = config;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var uri = request?.RequestUri?.AbsoluteUri;
            if (uri != null)
            {

            }
           

            return await base.SendAsync(request, cancellationToken);
        }
    }
}