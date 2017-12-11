namespace Lykke.Service.OperationsHistory.Core.Settings.Job.SlackNotifications
{
    public class AzureQueuePublicationSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}