using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public interface IHistoryLogEntryRepository
    {
        Task AddAsync(DateTime dateTime, double amount, string currency, string clientId, string customData,
            string opType, string id);
        Task<HistoryLogEntryEntity> UpdateAsync(string id, string customData);
        Task<HistoryLogEntryEntity> UpdateBlockchainHashAsync(string id, string hash);
        Task<HistoryLogEntryEntity> UpdateStateAsync(string id, int state);
        Task<IList<HistoryLogEntryEntity>> GetAllAsync(string clientId);
        Task<HistoryLogEntryEntity> GetById(string id);
    }
}