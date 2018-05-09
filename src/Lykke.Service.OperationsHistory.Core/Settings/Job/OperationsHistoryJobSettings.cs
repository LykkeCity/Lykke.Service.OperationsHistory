namespace Lykke.Service.OperationsHistory.Core.Settings.Job
{
    public class OperationsHistoryJobSettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings Rabbit { get; set; }
    }
}
