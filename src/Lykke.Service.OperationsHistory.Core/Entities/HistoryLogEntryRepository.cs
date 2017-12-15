using System;
using System.Threading.Tasks;
using AzureStorage;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public class HistoryLogEntryRepository : IHistoryLogEntryRepository
    {
        private readonly INoSQLTableStorage<HistoryLogEntryEntity> _tableStorage;

        private const int _batchPieceSize = 15;

        public HistoryLogEntryRepository(INoSQLTableStorage<HistoryLogEntryEntity> table)
        {
            _tableStorage = table;
        }

        public async Task AddAsync(DateTime dateTime, double amount, string currency, string walletId,
            string customData, string opType, string id)
        {
            var newEntry = new HistoryLogEntryEntity
            {
                DateTime = dateTime,
                OpType = opType,
                ClientId = walletId,
                Id = id,
                Amount = amount,
                CustomData = customData,
                Currency = currency
            };

            await Task.WhenAll(
                _tableStorage.InsertOrMergeAsync(HistoryLogEntryEntity.ByWalletId.Create(newEntry)),
                _tableStorage.InsertOrMergeAsync(HistoryLogEntryEntity.ByDate.Create(newEntry)),
                _tableStorage.InsertOrMergeAsync(HistoryLogEntryEntity.ByOperation.Create(newEntry)),
                _tableStorage.InsertOrMergeAsync(HistoryLogEntryEntity.ByAssetId.Create(newEntry)));
        }

        public async Task<HistoryLogEntryEntity> GetAsync(string walletId, string id)
        {
            return await _tableStorage.GetDataAsync(
                HistoryLogEntryEntity.ByWalletId.GeneratePartitionKey(walletId),
                HistoryLogEntryEntity.ByWalletId.GenerateRowKey(id));
        }

        public async Task<HistoryLogEntryEntity> UpdateAsync(string walletId, string id, string customData)
        {
            var existingItem = await GetAsync(walletId, id);

            if (existingItem == null)
                return null;

            var result = await _tableStorage.MergeAsync(HistoryLogEntryEntity.ByWalletId.GeneratePartitionKey(walletId),
                HistoryLogEntryEntity.ByWalletId.GenerateRowKey(id),
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

        public async Task<IList<HistoryLogEntryEntity>> GetByWalletIdAsync(string walletId)
        {
            var data = new List<HistoryLogEntryEntity>();

            await _tableStorage.GetDataByChunksAsync(HistoryLogEntryEntity.ByWalletId.GeneratePartitionKey(walletId),
                chunk => data.AddRange(chunk));

            return data;
        }

        public async Task<IList<HistoryLogEntryEntity>> GetByDatesAsync(DateTime dateFrom, DateTime dateTo)
        {
            var rangeQuery = AzureStorageUtils.QueryGenerator<HistoryLogEntryEntity>.PartitionKeyOnly.BetweenQuery(
                HistoryLogEntryEntity.ByDate.GeneratePartitionKey(dateFrom),
                HistoryLogEntryEntity.ByDate.GeneratePartitionKey(dateTo.AddDays(-1)),
                ToIntervalOption.IncludeTo);

            var data = new List<HistoryLogEntryEntity>();

            await _tableStorage.ExecuteAsync(rangeQuery, chunk => data.AddRange(chunk));

            return data;
        }

        public async Task<IList<HistoryLogEntryEntity>> GetByWalletsAsync(IEnumerable<string> walletIds)
        {
            var result = new List<HistoryLogEntryEntity>();

            await Task.WhenAll(
                walletIds.ToPieces(_batchPieceSize).Select(piece => _tableStorage.GetDataByChunksAsync(
                    AzureStorageUtils.QueryGenerator<HistoryLogEntryEntity>.MultiplePartitionKeys(piece
                        .Select(walletId => HistoryLogEntryEntity.ByWalletId.GeneratePartitionKey(walletId)).ToArray()),
                    items =>
                    {
                        lock (result)
                        {
                            result.AddRange(items);
                        }
                    })));

            return result;
        }
    }
}