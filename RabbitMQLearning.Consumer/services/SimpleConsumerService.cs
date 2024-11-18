using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLearning.Producer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLearning.Consumer.services
{
    public static class SimpleConsumerService
    {
        public static void Consume(string queue, ConnectionFactory factory)
        {
            var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue,
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
                    var result = JsonConvert.DeserializeObject<LearningMessage>(message);
                    Console.WriteLine($"Processing message: {message}");
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed processing message: {message}");
                    channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };
            channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
        }
    }
}
