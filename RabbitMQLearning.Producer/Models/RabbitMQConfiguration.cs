namespace RabbitMQLearning.Producer.Models
{
    public class RabbitMQConfiguration
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Tuple<string, string> LeaningQueue { get; set; }
    }
}
