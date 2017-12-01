namespace Lykke.Service.OperationsHistory.Core.Settings.Api
{
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        public string ExchangeOperationsHistory { get; set; }
        public string QueueOperationsCacheUpdater { get; set; }
    }
}