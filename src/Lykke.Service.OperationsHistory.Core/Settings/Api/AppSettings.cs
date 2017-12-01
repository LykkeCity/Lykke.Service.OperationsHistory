namespace Lykke.Service.OperationsHistory.Core.Settings.Api
{
    public class ApiSettings
    {
        public OperationsHistorySettings OperationsHistoryService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }

    public class OperationsHistorySettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings Rabbit { get; set; }

        /// <summary>
        /// The maximum amout of values returned per 1 page
        /// </summary>
        public int ValuesPerPage { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }

        public int ThrottlingLimitSeconds { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
