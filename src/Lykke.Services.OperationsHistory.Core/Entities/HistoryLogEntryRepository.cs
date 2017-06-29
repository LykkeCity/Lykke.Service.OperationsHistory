using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;

namespace Lykke.Services.OperationsHistory.Core.Entities
{
    public class HistoryLogEntryRepository: IHistoryLogEntryRepository
    {
        private readonly INoSQLTableStorage<HistoryLogEntryEntity> _table;
        public HistoryLogEntryRepository(INoSQLTableStorage<HistoryLogEntryEntity> table)
        {
            _table = table;
        }
        public Task AddAsync(DateTime dateTime, decimal amount, string currency, string clientId, string customData)
        {
            return _table.InsertAsync(HistoryLogEntryEntity.Create(
                dateTime, amount, currency, clientId, customData));
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetAllAsync()
        {
            return await _table.GetDataAsync();
        }
    }
}
