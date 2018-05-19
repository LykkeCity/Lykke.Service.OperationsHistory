using AzureStorage.Tables;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.OperationsHistory.Core.Entities.MigrationFlag
{
    public class MigrationFlagEntity : TableEntity, IMigrationFlag
    {
        public string ClientId => RowKey;

        public static string GetPartitionKey()
        {
            return "OperationsHistoryMigrationFlag";
        }

        public static MigrationFlagEntity Create(string clientId)
        {
            return new MigrationFlagEntity
            {
                PartitionKey = GetPartitionKey(),
                RowKey = clientId
            };
        }
    }
}