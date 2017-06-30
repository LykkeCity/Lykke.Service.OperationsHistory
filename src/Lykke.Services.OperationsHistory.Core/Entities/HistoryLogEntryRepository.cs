using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public class HistoryLogEntryRepository: IHistoryLogEntryRepository
    {
        private readonly INoSQLTableStorage<HistoryLogEntryEntity> _table;
        public HistoryLogEntryRepository(INoSQLTableStorage<HistoryLogEntryEntity> table)
        {
            _table = table;
        }
        public Task AddAsync(DateTime dateTime, decimal amount, string currency, string clientId, string customData, string opType)
        {
            return _table.InsertAsync(HistoryLogEntryEntity.Create(
                dateTime, amount, currency, clientId, customData, opType));
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetAllAsync(string clientId)
        {
            return await _table.GetDataAsync();
        }
    }
}
