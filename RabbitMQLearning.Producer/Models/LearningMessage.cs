namespace RabbitMQLearning.Producer.Models
{
    public class LearningMessage
    {
        public Guid Id => Guid.NewGuid();
        public int RandonNumber => new Random().Next(int.MinValue, int.MaxValue);
    }
}
