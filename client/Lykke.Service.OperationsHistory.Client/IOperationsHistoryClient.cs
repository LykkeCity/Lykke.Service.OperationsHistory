
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.AutorestClient.Models;

namespace Lykke.Service.OperationsHistory.Client
{
    public interface IOperationsHistoryClient
    {
        Task<IList<HistoryEntryResponse>> AllAsync(string clientId, int top, int skip);
        Task<IList<HistoryEntryResponse>> AllAsync(string clientId, int page);
        Task<IList<HistoryEntryResponse>> ByOperationAsync(string clientId, string operationType, int top, int skip);
        Task<IList<HistoryEntryResponse>> ByOperationAsync(string clientId, string operationType, int page);
        Task<IList<HistoryEntryResponse>> ByOperationAndAssetAsync(string clientId, string operationType,
            string assetId, int top, int skip);
        Task<IList<HistoryEntryResponse>> ByOperationAndAssetAsync(string clientId, string operationType,
            string assetId, int page);
        Task<IList<HistoryEntryResponse>> ByAssetAsync(string clientId, string assetId, int top, int skip);
        Task<IList<HistoryEntryResponse>> ByAssetAsync(string clientId, string assetId, int page);
    }
}