using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLearning.Consumer.services.Interfaces;
using RabbitMQLearning.Producer.Models;
using System.Text;

namespace RabbitMQLearning.Consumer.services
{
    public class ConsumerService : IConsumerService
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queue;

        public ConsumerService(RabbitMQConfiguration rabbitMQConfiguration)
        {
            
            _queue = rabbitMQConfiguration.LeaningQueue.Item1;
            _factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMQConfiguration.HostName),                
                UserName = rabbitMQConfiguration.UserName,
                Password = rabbitMQConfiguration.Password
            };
        }

        public async Task Process()
        {
            Console.WriteLine("Start listening");
            Console.WriteLine($"Start listening queue: {_queue}");
            var connection = _factory.CreateConnection();

            var channel = connection.CreateModel();
            channel.QueueDeclare(_queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    var result = JsonConvert.DeserializeObject<List<LearningMessage>>(message);
                    Console.WriteLine($"Processing message: {message}");
                    channel.BasicAck(eventArgs.DeliveryTag, false);                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed processing message: {message}");
                    channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };
            channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);
        }
    }
}
