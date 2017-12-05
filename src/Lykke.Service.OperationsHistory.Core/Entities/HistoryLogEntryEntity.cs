using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public class HistoryLogEntryEntity: TableEntity, IHistoryLogEntryEntity
    {
        public static class ByDate
        {
            public static string GeneratePartitionKey(DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd");
            }

            public static string GenerateRowKey(string id)
            {
                return id;
            }

            public static HistoryLogEntryEntity Create(IHistoryLogEntryEntity src)
            {
                var entity = CreateNew(src);
                entity.PartitionKey = GeneratePartitionKey(src.DateTime);
                entity.RowKey = GenerateRowKey(src.Id);

                return entity;
            }
        }

        public static class ByClientId
        {
            public static string GeneratePartitionKey(string clientId)
            {
                return clientId;
            }

            public static string GenerateRowKey(string id)
            {
                return id;
            }

            public static HistoryLogEntryEntity Create(IHistoryLogEntryEntity src)
            {
                var entity = CreateNew(src);
                entity.PartitionKey = GeneratePartitionKey(src.ClientId);
                entity.RowKey = GenerateRowKey(src.Id);

                return entity;
            }
        }

        public static class ByOperation
        {
            public static string GeneratePartitionKey(string opType)
            {
                return opType;
            }

            public static string GenerateRowKey(string id)
            {
                return id;
            }

            public static HistoryLogEntryEntity Create(IHistoryLogEntryEntity src)
            {
                var entity = CreateNew(src);
                entity.PartitionKey = GeneratePartitionKey(src.OpType);
                entity.RowKey = GenerateRowKey(src.Id);

                return entity;
            }
        }

        public static class ByAssetId
        {
            public static string GeneratePartitionKey(string assetId)
            {
                return assetId;
            }

            public static string GenerateRowKey(string id)
            {
                return id;
            }

            public static HistoryLogEntryEntity Create(IHistoryLogEntryEntity src)
            {
                var entity = CreateNew(src);
                entity.PartitionKey = GeneratePartitionKey(src.Currency);
                entity.RowKey = GenerateRowKey(src.Id);

                return entity;
            }
        }

        public string Id { get; set; }
        public DateTime DateTime { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string OpType { get; set; }
        public string ClientId { get; set; }
        public string CustomData { get; set; }

        private static HistoryLogEntryEntity CreateNew(DateTime dateTime, double amount, string currency,
            string clientId, string customData, string opType, string id)
        {
            return new HistoryLogEntryEntity
            {
                Id = id,
                DateTime = dateTime,
                Amount = amount,
                Currency = currency,
                ClientId = clientId,
                CustomData = customData,
                OpType = opType
            };
        }

        private static HistoryLogEntryEntity CreateNew(IHistoryLogEntryEntity src)
        {
            return CreateNew(src.DateTime, src.Amount, src.Currency, src.ClientId, src.CustomData, src.OpType, src.Id);
        }
    }
}
