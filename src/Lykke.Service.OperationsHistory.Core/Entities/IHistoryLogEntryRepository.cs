using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public interface IHistoryLogEntryRepository
    {
        Task AddAsync(DateTime dateTime, double amount, string currency, string clientId, string customData, string opType);
        Task<IEnumerable<IHistoryLogEntryEntity>> GetAllAsync(string clientId);
    }
}