
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.AutorestClient.Models;
using Lykke.Service.OperationsHistory.Client.Models;

namespace Lykke.Service.OperationsHistory.Client
{
    public interface IOperationsHistoryClient
    {
        Task<OperationsHistoryResponse> AllAsync(string clientId, int top, int skip);
        Task<OperationsHistoryResponse> AllAsync(string clientId, int page);
        Task<OperationsHistoryResponse> ByOperationAsync(string clientId, string operationType, int top, int skip);
        Task<OperationsHistoryResponse> ByOperationAsync(string clientId, string operationType, int page);
        Task<OperationsHistoryResponse> ByOperationAndAssetAsync(string clientId, string operationType,
            string assetId, int top, int skip);
        Task<OperationsHistoryResponse> ByOperationAndAssetAsync(string clientId, string operationType,
            string assetId, int page);
        Task<OperationsHistoryResponse> ByAssetAsync(string clientId, string assetId, int top, int skip);
        Task<OperationsHistoryResponse> ByAssetAsync(string clientId, string assetId, int page);
    }
}