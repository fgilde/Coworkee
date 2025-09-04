using System;
using System.Threading;
using System.Threading.Tasks;

namespace Coworkee.Application.Contracts
{
    public interface IServiceBus
    {
        Task SendMessageAsync(string queue, object content, CancellationToken cancellationToken);

        Task ListenAsync<T>(string queue, Action<T> onReceived, CancellationToken cancellationToken);
    }
}
