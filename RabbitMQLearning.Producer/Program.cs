using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQLearning.Producer.Extensions;
using RabbitMQLearning.Producer.Services.Interfaces;

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
Console.WriteLine("Start publishing messages");
var producerService = _host.Services.GetRequiredService<IProducerService>();
await producerService.ProcessAtomic();
await producerService.ProcessBatch();
//await producerService.ProcessDelayed();
