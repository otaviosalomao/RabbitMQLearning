using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLearning.Consumer.services;
using RabbitMQLearning.Producer.Extensions;
using RabbitMQLearning.Producer.Models;
using System.Text;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json", false, false)
    .AddJsonFile("appSettings.Development.json", false, false)
    .AddEnvironmentVariables();

var config = builder.Build();
IHost _host = Host.CreateDefaultBuilder().Build();

var learningQueue = config["RabbitMQQueues:LearningDelayed"];
Console.WriteLine("Start listening");
Console.WriteLine($"Start listening queue: {learningQueue}");


var factory = new ConnectionFactory
{
    HostName = config["RabbitMqConfiguration:Host"],
    UserName = config["RabbitMqConfiguration:Username"],
    Password = config["RabbitMqConfiguration:Password"]
};

var connection = factory.CreateConnection();

using var channel = connection.CreateModel();
channel.QueueDeclare(learningQueue,
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
        channel.BasicAck(eventArgs.DeliveryTag, true);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed processing message: {message}");
        channel.BasicNack(eventArgs.DeliveryTag, false, true);
    }
};
channel.BasicConsume(queue: learningQueue, autoAck: false, consumer: consumer);

Console.ReadKey();