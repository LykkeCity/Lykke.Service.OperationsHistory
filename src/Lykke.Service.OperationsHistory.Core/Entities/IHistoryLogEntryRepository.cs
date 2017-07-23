using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public interface IHistoryLogEntryRepository
    {
        Task AddAsync(DateTime dateTime, double amount, string currency, string clientId, string customData,
            string opType, string id);
        Task<IEnumerable<HistoryLogEntryEntity>> UpdateAsync(string id, string customData);
        Task<IEnumerable<HistoryLogEntryEntity>> GetAllAsync(string clientId);
        Task<IEnumerable<HistoryLogEntryEntity>> GetById(string id);
    }
}