using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core;
using MongoDB.Driver;

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
                x =>
                    x.ClientId == clientId &&
                    x.Id == operation.Id,
                Builders<OperationsHistoryEntity>
                    .Update
                        .Set(s => s.CustomData, customData)
                        .Set(x => x.State, operation.State));
        }

        public async Task<OperationsHistoryEntity> GetByIdAsync(string clientId, string id)
        {
            return (await _collection.Find(x => x.ClientId == clientId && x.Id == id).ToListAsync()).FirstOrDefault();
        }

        public async Task<IEnumerable<OperationsHistoryEntity>> GetByClientIdAsync(string clientId,
            string walletId,
            HistoryOperationType[] operationTypes,
            string assetId,
            string assetPairId,
            int? take,
            int skip)
        {
            var result = new List<OperationsHistoryEntity>();
            
            var cursor =
                take.HasValue
                    ? await _collection
                        .Find(x =>
                            x.ClientId == clientId &&
                            (walletId == null || x.WalletId == walletId) &&
                            (!operationTypes.Any() || operationTypes.Contains(x.Type)) &&
                            (assetId == null || x.AssetId == assetId) &&
                            (assetPairId == null || x.AssetPairId == assetPairId),
                            new FindOptions
                            {
                                BatchSize = 1000
                            })
                        .Sort(Builders<OperationsHistoryEntity>.Sort.Descending(x => x.DateTime))
                        .Skip(skip)
                        .Limit(take)
                        .ToCursorAsync()
                    : await _collection
                        .Find(x =>
                                x.ClientId == clientId &&
                                (walletId == null || x.WalletId == walletId) &&
                                (!operationTypes.Any() || operationTypes.Contains(x.Type)) &&
                                (assetId == null || x.AssetId == assetId) &&
                                (assetPairId == null || x.AssetPairId == assetPairId),
                            new FindOptions
                            {
                                BatchSize = 1000
                            })
                        .Sort(Builders<OperationsHistoryEntity>.Sort.Descending(x => x.DateTime))
                        .Skip(skip)
                        .ToCursorAsync();
            
            while (await cursor.MoveNextAsync())
                result.AddRange(cursor.Current);
            
            return result.OrderByDescending(x => x.DateTime);

        }

        public async Task DeleteIfExistsAsync(string clientId, string id)
        {
            await _collection.DeleteOneAsync(x => x.ClientId == clientId && x.Id == id);
        }
    }
}