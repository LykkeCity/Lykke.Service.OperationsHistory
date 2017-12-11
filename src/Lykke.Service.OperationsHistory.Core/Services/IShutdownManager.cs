using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}