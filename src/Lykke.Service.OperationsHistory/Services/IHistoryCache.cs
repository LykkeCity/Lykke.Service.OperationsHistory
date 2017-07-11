using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Services
{
    public interface IHistoryCache
    {
        Task<IEnumerable<HistoryEntryResponse>> GetAllPagedAsync(string clientId, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllPagedAsync(string clientId, string assetId, string operationType, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllByOpTypePagedAsync(string clientId, string operationType, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllByAssetPagedAsync(string clientId, string assetId, int page);
    }
}