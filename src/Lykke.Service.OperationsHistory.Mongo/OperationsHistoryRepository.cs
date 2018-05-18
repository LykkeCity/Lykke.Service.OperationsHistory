using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core;
using MongoDB.Driver;
using Lykke.Service.OperationsRepository.Contract;

namespace Lykke.Service.OperationsHistory.Mongo
{
    public class OperationsHistoryRepository : IOperationsHistoryRepository
    {
        private readonly IMongoCollection<OperationsHistoryEntity> _collection;

        public OperationsHistoryRepository(
            IMongoCollection<OperationsHistoryEntity> collection)
        {
            _collection = collection;
        }
        
        public async Task AddOrUpdateAsync(string clientId, string walletId, HistoryOperation operation, string customData)
        {
            try
            {
                await _collection.InsertOneAsync(
                    OperationsHistoryEntity.Create(clientId, walletId, operation, customData));
                return;
            }
            catch (MongoWriteException writeException)
            {
                if (writeException.WriteError.Category != ServerErrorCategory.DuplicateKey)
                    throw;
            }

            await _collection.UpdateOneAsync(
                x => x.Id == operation.Id,
                Builders<OperationsHistoryEntity>
                    .Update
                        .Set(s => s.CustomData, customData)
                        .Set(x => x.State, operation.State));
        }

        public async Task<OperationsHistoryEntity> GetByIdAsync(string id)
        {
            return (await _collection.Find(x => x.Id == id).ToListAsync()).Single();
        }

        public async Task<IEnumerable<OperationsHistoryEntity>> GetByClientIdAsync(
            string clientId,
            string walletId,
            HistoryOperationType? operationType,
            string assetId,
            string assetPairId,
            int take, 
            int skip)
        {
            var result = new List<OperationsHistoryEntity>();

            var queryByOperationType = operationType.HasValue;
            var type = operationType ?? HistoryOperationType.Trade;
            
            var cursor = await _collection
                .Find(x =>
                    x.ClientId == clientId &&
                    (walletId == null || x.WalletId == walletId) &&
                    (!queryByOperationType || x.Type == type) &&
                    (assetId == null || x.AssetId == assetId) &&
                    (assetPairId == null || x.AssetPairId == assetPairId),
                    new FindOptions
                    {
                        BatchSize = 1000
                    })
                .Sort(Builders<OperationsHistoryEntity>.Sort.Descending(x => x.DateTime))
                .Skip(skip)
                .Limit(take)
                .ToCursorAsync();
            
            while (await cursor.MoveNextAsync())
                result.AddRange(cursor.Current);
            
            return result.OrderByDescending(x => x.DateTime);

        }

        public async Task DeleteIfExistsAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}