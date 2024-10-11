using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQLearning.Producer.Models;
using RabbitMQLearning.Producer.Services.Interfaces;
using System.Text;

namespace RabbitMQLearning.Producer.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMQService(RabbitMQConfiguration rabbitMQConfiguration)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = rabbitMQConfiguration.HostName,
                UserName = rabbitMQConfiguration.UserName,
                Password = rabbitMQConfiguration.Password
            };
        }

        public async Task Publish<T>(T obj, string queue, string exchange)
        {
            var json = JsonConvert.SerializeObject(obj);
            var body = Encoding.UTF8.GetBytes(json);
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueBind(queue: queue, exchange: exchange, routingKey: string.Empty);
            channel.BasicPublish(exchange: exchange,
                                 routingKey: string.Empty,
                                 basicProperties: null,
                                 body: body);
        }
        public async Task PublishBatch<T>(IEnumerable<T> obj, string queue, string exchange)
        {
            var json = JsonConvert.SerializeObject(obj);
            var body = Encoding.UTF8.GetBytes(json);
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueBind(queue: queue, exchange: exchange, routingKey: string.Empty);
            var basicPublishBatch = channel.CreateBasicPublishBatch();
            foreach (var item in obj)
            {
                basicPublishBatch.Add(exchange, string.Empty, false, null, body: body);
            }
            basicPublishBatch.Publish();
        }
        public async Task PublishDelayed<T>(T obj, string queueName, string exchange)
        {
            var delayBind = "delayBind";
            var json = JsonConvert.SerializeObject(obj);
            var body = Encoding.UTF8.GetBytes(json);
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            IDictionary<string, object> args = new Dictionary<string, object> { { "x-delayed-type", "direct" } };
            channel.ExchangeDeclare(exchange, "x-delayed-message", true, false, args);
            var queue = channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue, exchange, delayBind);
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object> { { "x-delay", 60000 } };
            channel.BasicPublish(exchange: exchange, routingKey: delayBind, basicProperties: basicProperties, body: body);
            Console.WriteLine($"Processing message: {json}");
        }
    }
}
