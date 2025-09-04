using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Client.Configuration;
using Coworkee.Shared.Constants.Storage;

namespace Coworkee.Client.Handler
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