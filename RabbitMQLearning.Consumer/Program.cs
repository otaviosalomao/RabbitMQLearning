using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQLearning.Consumer.Extensions;
using RabbitMQLearning.Consumer.services.Interfaces;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json", false, false)
    .AddJsonFile("appSettings.Development.json", false, false)
    .AddEnvironmentVariables();

var config = builder.Build();
IHost _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
{
    services.ConfigureServices();
    services.ConfigureRabbitMQ(config);
}).Build();
var consumerService = _host.Services.GetRequiredService<IConsumerService>();
await consumerService.Process();

Console.ReadKey();