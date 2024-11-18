using Newtonsoft.Json;
using RabbitMQLearning.Producer.Models;
using RabbitMQLearning.Producer.Services.Interfaces;
using System.Text.Json.Nodes;

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

        public async Task ProcessAtomic()
        {
            foreach (var item in Enumerable.Range(1, 100000))
            {
                var learningMessage = new LearningMessage();
                Console.WriteLine($"Publishing message {JsonConvert.SerializeObject(learningMessage)}");
                await _rabbitMQService.Publish<LearningMessage>(
                    learningMessage,
                    _rabbitMQConfiguration.LeaningQueue.Item1,
                    _rabbitMQConfiguration.LeaningQueue.Item2);
            }
        }
        public async Task ProcessBatch()
        {
            foreach (var item in Enumerable.Range(1, 100000))
            {
                var learningMessages = Enumerable.Range(1, 200).Select(o => new LearningMessage()).ToList();
                Console.WriteLine($"Publishing message {JsonConvert.SerializeObject(learningMessages)}");
                await _rabbitMQService.PublishBatch<LearningMessage>(
                    learningMessages,
                    _rabbitMQConfiguration.LeaningBatchQueue.Item1,
                    _rabbitMQConfiguration.LeaningBatchQueue.Item2);
            }
        }
        public async Task ProcessDelayed()
        {
            foreach (var item in Enumerable.Range(1, 100000))
            {
                var learningMessage = new LearningMessage();
                Console.WriteLine($"Publishing message {JsonConvert.SerializeObject(learningMessage)}");
                await _rabbitMQService.PublishDelayed<LearningMessage>(
                    learningMessage,
                    _rabbitMQConfiguration.LeaningDelayedQueue.Item1,
                    _rabbitMQConfiguration.LeaningDelayedQueue.Item2);
            }
        }
    }
}
