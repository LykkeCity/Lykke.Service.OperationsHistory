using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Services
{
    public interface IHistoryCache
    {
        Task<IEnumerable<HistoryEntryResponse>> GetAsync(string clientId, string operationType, string assetId,
            int take, int skip);

        void AddOrUpdate(IHistoryLogEntryEntity item);
        Task WarmUp(string clientId);
    }
}