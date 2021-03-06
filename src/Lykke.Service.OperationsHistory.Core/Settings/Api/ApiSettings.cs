using System.Collections.Generic;
using Lykke.Service.ClientAccount.Client;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.OperationsHistory.Core.Settings.Api
{
    public class ApiSettings
    {
        public OperationsHistorySettings OperationsHistoryService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public ClientAccountServiceClientSettings ClientAccountServiceClient { get; set; }
        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public MongoSettings OperationsHistoryMongo { get; set; }
    }

    public class OperationsHistorySettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings RabbitOperations { get; set; }
        public RabbitMqSettings RabbitRegistration { get; set; }
        public string[] ClientsToIgnore { get; set; }

        /// <summary>
        /// The maximum amout of values returned per 1 page
        /// </summary>
        public int ValuesPerPage { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string DataConnString { get; set; }
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

    public class AssetsServiceClientSettings
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
