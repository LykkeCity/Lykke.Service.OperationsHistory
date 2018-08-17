using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.OperationsHistory.Core.Settings
{
    public class MongoSettings
    {
        [MongoCheck]
        public string ConnectionString { set; get; }
    }
}