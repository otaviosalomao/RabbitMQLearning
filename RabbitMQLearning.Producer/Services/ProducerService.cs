using RabbitMQLearning.Producer.Models;
using RabbitMQLearning.Producer.Services.Interfaces;

namespace RabbitMQLearning.Producer.Services
{
    public class ProducerService : IProducerService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly RabbitMQConfiguration _rabbitMQConfiguration;

        public ProducerService(
            IRabbitMQService rabbitMQService,
            RabbitMQConfiguration rabbitMQConfiguration)
        {
            _rabbitMQService = rabbitMQService;
            _rabbitMQConfiguration = rabbitMQConfiguration;
        }

        public async Task Process()
        {
            foreach (var item in Enumerable.Range(1, 100000))
            {
                var learningMessages = Enumerable.Range(1, 200).Select(o => new LearningMessage()).ToList();
                await _rabbitMQService.PublishBatch<LearningMessage>(
                    learningMessages,
                    _rabbitMQConfiguration.LeaningQueue.Item1,
                    _rabbitMQConfiguration.LeaningQueue.Item2);
            }
        }       
    }
}
