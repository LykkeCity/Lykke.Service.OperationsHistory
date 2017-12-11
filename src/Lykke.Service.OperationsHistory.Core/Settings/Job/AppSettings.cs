using Lykke.Service.OperationsHistory.Core.Settings.Job.SlackNotifications;

namespace Lykke.Service.OperationsHistory.Core.Settings.Job
{
    public class AppSettings
    {
        public OperationsHistoryJobSettings OperationsHistoryJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}