using System;
using System.Threading.Tasks;
using AzureStorage;
using System.Collections.Generic;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public class HistoryLogEntryRepository: IHistoryLogEntryRepository
    {
        private readonly INoSQLTableStorage<HistoryLogEntryEntity> _tableStorage;

        public HistoryLogEntryRepository(INoSQLTableStorage<HistoryLogEntryEntity> table)
        {
            _tableStorage = table;
        }

        public async Task AddAsync(DateTime dateTime, double amount, string currency, string clientId, string customData, string opType, string id)
        {
            var newEntry = new HistoryLogEntryEntity
            {
                DateTime = dateTime,
                OpType = opType,
                ClientId = clientId,
                Id = id,
                Amount = amount,
                CustomData = customData,
                Currency = currency
            };

            await Task.WhenAll(
                _tableStorage.InsertAsync(HistoryLogEntryEntity.ByClientId.Create(newEntry)),
                _tableStorage.InsertAsync(HistoryLogEntryEntity.ByDate.Create(newEntry)),
                _tableStorage.InsertAsync(HistoryLogEntryEntity.ByOperation.Create(newEntry)),
                _tableStorage.InsertAsync(HistoryLogEntryEntity.ByAssetId.Create(newEntry)));
        }

        public async Task<HistoryLogEntryEntity> GetAsync(string clientId, string id)
        {
            return await _tableStorage.GetDataAsync(
                HistoryLogEntryEntity.ByClientId.GeneratePartitionKey(clientId),
                HistoryLogEntryEntity.ByClientId.GenerateRowKey(id));
        }

        public async Task<HistoryLogEntryEntity> UpdateAsync(string clientId, string id, string customData)
        {
            var existingItem = await GetAsync(clientId, id);

            if (existingItem == null)
                return null;

            var result = await _tableStorage.MergeAsync(HistoryLogEntryEntity.ByClientId.GeneratePartitionKey(clientId),
                HistoryLogEntryEntity.ByClientId.GenerateRowKey(id),
                item =>
                {
                    item.CustomData = customData;
                    return item;
                });

            await _tableStorage.MergeAsync(HistoryLogEntryEntity.ByDate.GeneratePartitionKey(existingItem.DateTime),
                HistoryLogEntryEntity.ByDate.GenerateRowKey(existingItem.Id),
                item =>
                {
                    item.CustomData = customData;
                    return item;
                });

            await _tableStorage.MergeAsync(HistoryLogEntryEntity.ByOperation.GeneratePartitionKey(existingItem.OpType),
                HistoryLogEntryEntity.ByOperation.GenerateRowKey(existingItem.Id),
                item =>
                {
                    item.CustomData = customData;
                    return item;
                });

            await _tableStorage.MergeAsync(HistoryLogEntryEntity.ByAssetId.GeneratePartitionKey(existingItem.OpType),
                HistoryLogEntryEntity.ByAssetId.GenerateRowKey(existingItem.Id),
                item =>
                {
                    item.CustomData = customData;
                    return item;
                });

            return result;
        }

        public async Task<IList<HistoryLogEntryEntity>> GetByClientIdAsync(string clientId)
        {
            var data = new List<HistoryLogEntryEntity>();

            await _tableStorage.GetDataByChunksAsync(HistoryLogEntryEntity.ByClientId.GeneratePartitionKey(clientId),
                chunk => data.AddRange(chunk));

            return data;
        }
    }
}
