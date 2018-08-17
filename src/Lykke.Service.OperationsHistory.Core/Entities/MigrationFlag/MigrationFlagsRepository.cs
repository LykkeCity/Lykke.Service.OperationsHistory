using System.Threading.Tasks;
using AzureStorage;

namespace Lykke.Service.OperationsHistory.Core.Entities.MigrationFlag
{
    public class MigrationFlagsRepository : IMigrationFlagsRepository
    {
        public const string TableName = "OperationsHistoryMigrationFlags";
        
        private readonly INoSQLTableStorage<MigrationFlagEntity> _tableStorage;

        public MigrationFlagsRepository(INoSQLTableStorage<MigrationFlagEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }
        
        public async Task<bool> ClientWasMigrated(string clinetId)
        {
            return await _tableStorage.GetDataAsync(MigrationFlagEntity.GetPartitionKey(), clinetId) != null;
        }
    }
}