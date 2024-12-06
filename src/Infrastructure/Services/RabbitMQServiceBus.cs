using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Configurations;
using Coworkee.Application.Contracts;
using Coworkee.Application.Contracts.Attributes;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Coworkee.Infrastructure.Services
{
    [RegisterAs(typeof(IServiceBus))]
    public class RabbitMQServiceBus : IServiceBus
    {
        private readonly ServerConfiguration _configuration;
        public bool Enabled => _configuration.RabbitMQ.Enabled;

        public RabbitMQServiceBus(ServerConfiguration configuration)
        {
            _configuration = configuration;
        }


        public Task SendMessageAsync(string queue, object content, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task ListenAsync<T>(string queue, Action<T> onReceived, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        static T TryParse<T>(string json)
        {
            //TODO: Generate and use schema to check type T and remove try catch
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
        }

    }
}
