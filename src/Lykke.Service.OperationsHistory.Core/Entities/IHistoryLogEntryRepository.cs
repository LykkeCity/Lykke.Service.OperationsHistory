using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public interface IHistoryLogEntryRepository
    {
        Task AddAsync(DateTime dateTime, double amount, string currency, string walletId,
            string customData, string opType, string id);

        Task<HistoryLogEntryEntity> UpdateAsync(string walletId, string id, string customData);

        Task<HistoryLogEntryEntity> GetAsync(string walletId, string id);

        Task<IList<HistoryLogEntryEntity>> GetByWalletIdAsync(string walletId);

        Task<IList<HistoryLogEntryEntity>> GetByWalletsAsync(IEnumerable<string> walletIds);

        Task<IList<HistoryLogEntryEntity>> GetByDatesAsync(DateTime dateFrom, DateTime dateTo);

        Task DeleteIfExistsAsync(string clientId, string operationId);
    }
}