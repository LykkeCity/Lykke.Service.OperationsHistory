using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public interface IHistoryLogEntryRepository
    {
        Task AddAsync(DateTime dateTime, double amount, string assetId, string clientId,
            string customData, string opType, string id);

        Task<HistoryLogEntryEntity> UpdateAsync(string clientId, string id, string customData);

        Task<HistoryLogEntryEntity> GetAsync(string clientId, string id);

        Task<IList<HistoryLogEntryEntity>> GetByClientIdAsync(string clientId);
    }
}