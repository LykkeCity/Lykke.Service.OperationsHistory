using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.OperationsHistory.Core.Settings.Api
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }
    }
}