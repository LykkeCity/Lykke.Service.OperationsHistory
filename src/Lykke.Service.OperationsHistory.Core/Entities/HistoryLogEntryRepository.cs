using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public class HistoryLogEntryRepository: IHistoryLogEntryRepository
    {
        private readonly INoSQLTableStorage<HistoryLogEntryEntity> _table;
        public HistoryLogEntryRepository(INoSQLTableStorage<HistoryLogEntryEntity> table)
        {
            _table = table;
        }

        public Task AddAsync(DateTime dateTime, double amount, string currency, string clientId, string customData, string opType,
            string id)
        {
            return _table.InsertAsync(HistoryLogEntryEntity.Create(
                dateTime, amount, currency, clientId, customData, opType, id));
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetAllAsync(string clientId)
        {
            var query = new TableQuery<HistoryLogEntryEntity>().Where($"ClientId eq '{clientId}'");
            return await _table.WhereAsync(query);
        }
    }
}
