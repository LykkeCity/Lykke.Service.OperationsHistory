using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Services.OperationsHistory.Core.Entities
{
    public class HistoryLogEntryEntity: TableEntity, IHistoryLogEntryEntity
    {
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ClientId { get; set; }
        public string CustomData { get; set; }

        public static HistoryLogEntryEntity Create(DateTime dateTime ,decimal amount, string currency, string clientId, string customData)
        {
            return new HistoryLogEntryEntity
            {
                DateTime = dateTime,
                Amount = amount,
                Currency = currency,
                ClientId = clientId,
                CustomData = customData
            };
        }
    }
}
