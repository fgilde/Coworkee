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
using IModel = RabbitMQ.Client.IModel;

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

        private Task ExecuteWithChannel(Action<IModel> action, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration.RabbitMQ.HostName,
                    Port = _configuration.RabbitMQ.Port,
                    //Protocol = Protocols.AMQP_0_9_1,
                    UserName = _configuration.RabbitMQ.UserName,
                    Password = _configuration.RabbitMQ.Password
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                action(channel);
            }, cancellationToken);
        }

        public Task SendMessageAsync(string queue, object content, CancellationToken cancellationToken)
        {
            return Enabled ? ExecuteWithChannel(channel =>
            {
                channel.QueueDeclare(queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                string message = JsonConvert.SerializeObject(content);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: queue,
                    basicProperties: null,
                    body: body);
            }, cancellationToken) : Task.CompletedTask;
        }

        public Task ListenAsync<T>(string queue, Action<T> onReceived, CancellationToken cancellationToken)
        {
            return Enabled ? ExecuteWithChannel(channel =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var content = TryParse<T>(Encoding.UTF8.GetString(body));
                        if (content != null)
                        {
                            onReceived(content);
                        }

                    };
                    channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);
                }

            }, cancellationToken) : Task.CompletedTask;
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
