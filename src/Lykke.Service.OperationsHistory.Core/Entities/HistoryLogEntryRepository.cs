using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;

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

        public async Task<HistoryLogEntryEntity> UpdateAsync(string id, string customData)
        {
            var existing = await GetById(id);

            return await _table.ReplaceAsync(existing.PartitionKey, existing.RowKey, itm =>
            {
                itm.CustomData = customData;
                return itm;
            });
        }

        public async Task<HistoryLogEntryEntity> UpdateBlockchainHashAsync(string id, string hash)
        {
            var existing = await GetById(id);

            dynamic o = JObject.Parse(existing.CustomData);
            o.BlockChainHash = hash;

            return await UpdateAsync(id, o.ToString());
        }

        public async Task<HistoryLogEntryEntity> UpdateStateAsync(string id, int state)
        {
            var existing = await GetById(id);

            dynamic o = JObject.Parse(existing.CustomData);
            o.State = state;

            return await UpdateAsync(id, o.ToString());
        }

        public async Task<IList<HistoryLogEntryEntity>> GetAllAsync(string clientId)
        {
            var query = new TableQuery<HistoryLogEntryEntity>().Where($"ClientId eq '{clientId}'");
            return (await _table.WhereAsync(query)).ToList();
        }

        public async Task<HistoryLogEntryEntity> GetById(string id)
        {
            var query = new TableQuery<HistoryLogEntryEntity>().Where($"Id eq '{id}'");

            var records = (await _table.WhereAsync(query)).ToList();

            if (records.Count == 0)
            {
                throw new Exception($"No record with id={id} in history log");
            }
            if (records.Count > 1)
            {
                throw new Exception($"Multiple records for id={id} in history log (Count={records.Count})");
            }

            return records.First();
        }

    }
}
