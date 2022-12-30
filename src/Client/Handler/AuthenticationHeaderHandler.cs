using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Coworkee.Client.Extensions;
using Coworkee.Shared.Constants.Storage;

namespace Coworkee.Client.Handler
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService localStorage;

        public AuthenticationHeaderHandler(ILocalStorageService localStorage)
            => this.localStorage = localStorage;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization?.Scheme != "Bearer")
            {
                var savedToken = await localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken, cancellationToken);

                if (!string.IsNullOrWhiteSpace(savedToken))
                {
                    request.SetAuthorization(savedToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}