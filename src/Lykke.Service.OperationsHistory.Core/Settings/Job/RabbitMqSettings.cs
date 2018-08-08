namespace Lykke.Service.OperationsHistory.Core.Settings.Job
{
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        public string ExchangeOperationsHistory { get; set; }
        public string QueueOperationsLogUpdater { get; set; }
    }
}