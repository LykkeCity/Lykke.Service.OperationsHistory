using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Services.OperationsHistory.Core.Entities
{
    public interface IHistoryLogEntryRepository
    {
        Task AddAsync(DateTime dateTime, decimal amount, string currency, string clientId, string customData, string opType);
        Task<IEnumerable<IHistoryLogEntryEntity>> GetAllAsync(string clientId);
    }
}