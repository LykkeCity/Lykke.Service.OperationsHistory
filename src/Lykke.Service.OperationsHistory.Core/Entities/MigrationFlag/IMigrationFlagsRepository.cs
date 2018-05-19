using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Core.Entities.MigrationFlag
{
    public interface IMigrationFlagsRepository
    {
        Task<bool> ClientWasMigrated(string clinetId);
    }
}