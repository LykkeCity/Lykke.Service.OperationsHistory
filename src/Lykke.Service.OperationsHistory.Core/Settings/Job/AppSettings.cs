using Lykke.Service.ClientAccount.Client;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using SlackNotificationsSettings = Lykke.Service.OperationsHistory.Core.Settings.Job.SlackNotifications.SlackNotificationsSettings;

namespace Lykke.Service.OperationsHistory.Core.Settings.Job
{
    public class AppSettings
    {
        public OperationsHistoryJobSettings OperationsHistoryJob { get; set; }
        public RedisSettings RedisSettings { get; set; }
        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public ClientAccountServiceClientSettings ClientAccountServiceClient { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public MongoSettings OperationsHistoryMongo { get; set; }
    }
}