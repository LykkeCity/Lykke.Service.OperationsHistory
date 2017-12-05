using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Client.Models;

namespace Lykke.Service.OperationsHistory.Client
{
    public interface IOperationsHistoryClient
    {
        Task<OperationsHistoryResponse> Get(string clientId, string operationType, string assetId, int take, int skip);
    }
}