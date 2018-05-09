using System;
using Lykke.Service.OperationsHistory.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Lykke.Service.OperationsRepository.Contract;
using Lykke.Service.OperationsRepository.Contract.Abstractions;

namespace Lykke.Service.OperationsHistory.Mongo
{
    public class OperationsHistoryEntity
    {
        [BsonId]
        public string Id { set; get; }
        
        [BsonElement("assetId")]
        public string AssetId { set; get; }
        
        [BsonElement("assetPairId")]
        public string AssetPairId { set; get; }
        
        [BsonElement("price")]
        public double? Price { set; get; }
        
        [BsonElement("feeType"), BsonRepresentation(BsonType.String)]
        public FeeType FeeType { get; set; }
        
        [BsonElement("feeSize")]
        public double FeeSize { get; set; }
        
        [BsonElement("type"), BsonRepresentation(BsonType.String)]
        public HistoryOperationType Type { set; get; }
        
        [BsonElement("state"), BsonRepresentation(BsonType.String)]
        public HistoryOperationState State { set; get; }
        
        [BsonElement("dateTime")]
        public DateTime DateTime { set; get; }
        
        [BsonElement("amount")]
        public double Amount { set; get; }
        
        [BsonElement("walletId")]
        public string WalletId { set; get; }
        
        [BsonElement("clientId")]
        public string ClientId { set; get; }
        
        [BsonElement("customData")]
        public string CustomData { set; get; }

        public static OperationsHistoryEntity Create(string clientId, string walletId, HistoryOperation operation, string customData)
        {
            return new OperationsHistoryEntity
            {
                Id = operation.Id,
                AssetId = operation.Asset,
                AssetPairId = operation.AssetPair,
                Type = operation.Type,
                Price = operation.Price,
                State = operation.State,
                DateTime = operation.DateTime,
                Amount = operation.Amount,
                FeeSize = operation.FeeSize,
                FeeType = operation.FeeType,
                CustomData = customData,
                WalletId = walletId,
                ClientId = clientId
            };
        }
    }

    public static class OperationsHistoryEntityHelper
    {
        public static HistoryOperation ToHistoryOperation(this OperationsHistoryEntity entity)
        {
            if (entity == null)
                return null;
            
            return HistoryOperation.Create(
                entity.Id,
                entity.DateTime,
                entity.Type,
                entity.State,
                entity.Amount,
                entity.AssetId,
                entity.AssetPairId,
                entity.Price,
                entity.FeeSize,
                entity.FeeType);
        }
    }
}