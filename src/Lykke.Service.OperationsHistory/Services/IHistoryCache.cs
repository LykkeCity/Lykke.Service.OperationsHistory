using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Services
{
    public interface IHistoryCache
    {
        Task<IEnumerable<HistoryEntryResponse>> GetAllPagedAsync(string clientId, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, int top, int skip);
        Task<IEnumerable<HistoryEntryResponse>> GetAllPagedAsync(string clientId, string assetId, string operationType, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, string assetId, string operationType, int top, int skip);
        Task<IEnumerable<HistoryEntryResponse>> GetAllByOpTypePagedAsync(string clientId, string operationType, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllByOpTypeAsync(string clientId, string operationType, int top, int skip);
        Task<IEnumerable<HistoryEntryResponse>> GetAllByAssetPagedAsync(string clientId, string assetId, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllByAssetAsync(string clientId, string assetId, int top, int skip);

        void AddOrUpdate(IHistoryLogEntryEntity item);
        Task WarmUp(string clientId);
    }
}