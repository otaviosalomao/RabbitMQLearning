namespace RabbitMQLearning.Producer.Services.Interfaces
{
    public interface IRabbitMQService
    {
        Task Publish<T>(T obj, string queue, string exchange);
        Task PublishBatch<T>(IEnumerable<T> obj, string queue, string exchange);
    }
}
