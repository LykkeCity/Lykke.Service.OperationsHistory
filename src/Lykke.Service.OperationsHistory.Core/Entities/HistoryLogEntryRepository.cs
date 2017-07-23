using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task AddAsync(DateTime dateTime, double amount, string currency, string clientId, string customData, string opType,
            string id)
        {
            await _table.InsertAsync(HistoryLogEntryEntity.Create(
                dateTime, amount, currency, clientId, customData, opType, id));
        }

        public async Task<IEnumerable<HistoryLogEntryEntity>> UpdateAsync(string id, string customData)
        {
            var existing = await GetById(id);
            var tasks = new List<Task<HistoryLogEntryEntity>>();
            foreach (var historyLogEntryEntity in existing)
            {
                tasks.Add(_table.ReplaceAsync(historyLogEntryEntity.PartitionKey, historyLogEntryEntity.RowKey, itm =>
                {
                    itm.CustomData = customData;
                    return itm;
                }));
            }

            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<HistoryLogEntryEntity>> GetAllAsync(string clientId)
        {
            var query = new TableQuery<HistoryLogEntryEntity>().Where($"ClientId eq '{clientId}'");
            return await _table.WhereAsync(query);
        }

        public async Task<IEnumerable<HistoryLogEntryEntity>> GetById(string id)
        {
            var query = new TableQuery<HistoryLogEntryEntity>().Where($"Id eq '{id}'");
            return await _table.WhereAsync(query);
        }
    }
}
