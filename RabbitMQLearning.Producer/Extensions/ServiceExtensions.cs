using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQLearning.Producer.Models;
using RabbitMQLearning.Producer.Services;
using RabbitMQLearning.Producer.Services.Interfaces;

namespace RabbitMQLearning.Producer.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IProducerService, ProducerService>();
        }
        public static void ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConfiguration = new RabbitMQConfiguration()
            {
                HostName = configuration["RabbitMQConfiguration:Host"],
                UserName = configuration["RabbitMQConfiguration:Username"],
                Password = configuration["RabbitMQConfiguration:Password"],
                LeaningQueue = new Tuple<string, string>(
                    configuration["RabbitMQQueues:Learning:Name"],
                    configuration["RabbitMQQueues:Learning:Exchange"]
                )
            };
            services.AddScoped<IRabbitMQService, RabbitMQService>();
            services.AddSingleton(rabbitMqConfiguration);
        }
    }
}
