using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts;
using CleanArchitectureBase.Application.Contracts.Attributes;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IModel = RabbitMQ.Client.IModel;

namespace CleanArchitectureBase.Infrastructure.Services
{
    [RegisterAs(typeof(IServiceBus))]
    public class RabbitMQServiceBus : IServiceBus
    {
        public bool Enabled { get; set; } = false;

        private Task ExecuteWithChannel(Action<IModel> action, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var factory = new ConnectionFactory
                {
                    //TODO: extract to a config
                    HostName = "your-rabbit-mq-server.de",
                    Port = 5672,
                    //Protocol = Protocols.AMQP_0_9_1,
                    UserName = "admin",
                    Password = "admin"
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
