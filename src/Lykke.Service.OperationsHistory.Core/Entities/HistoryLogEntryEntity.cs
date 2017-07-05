using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public class HistoryLogEntryEntity: TableEntity, IHistoryLogEntryEntity
    {
        public static string GeneratePartitionKey(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
        public static string GenerateRowKey()
        {
            return Guid.NewGuid().ToString();
        }
        public DateTime DateTime { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string OpType { get; set; }
        public string ClientId { get; set; }
        public string CustomData { get; set; }

        public static HistoryLogEntryEntity Create(DateTime dateTime, double amount, string currency, string clientId, string customData, string opType)
        {
            return new HistoryLogEntryEntity
            {
                PartitionKey = GeneratePartitionKey(dateTime),
                RowKey = GenerateRowKey(),
                DateTime = dateTime,
                Amount = amount,
                Currency = currency,
                ClientId = clientId,
                CustomData = customData,
                OpType = opType
            };
        }
    }
}
