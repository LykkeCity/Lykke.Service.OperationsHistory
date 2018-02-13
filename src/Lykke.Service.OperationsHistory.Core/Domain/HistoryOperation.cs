using Lykke.Service.OperationsRepository.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Lykke.Service.OperationsHistory.Core
{
    public enum CashOutState
    {
        Regular = 0,
        Request,
        Done,
        Cancelled
    }

    public enum HistoryOperationType
    {
        CashIn,
        CashOut,
        Trade,
        LimitTrade,
        LimitOrderEvent
    }

    public enum HistoryOperationState
    {
        InProgress,
        Finished,
        Canceled,
        Failed
    }

    /// <summary>
    /// Represents any operation logged to history
    /// </summary>
    public class HistoryOperation
    {
        public string Id { get; set; }
        public DateTime DateTime { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public HistoryOperationType Type { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public HistoryOperationState State { get; set; }
        public double Amount { get; set; }
        public string Asset { get; set; }
        public string AssetPair { get; set; }
        public double? Price { get; set; }

        public static HistoryOperation Create(
            string id,
            DateTime dateTime,
            HistoryOperationType type,
            HistoryOperationState state,
            double amount,
            string asset,
            string assetPair,
            double? price = default(double?))
        {
            return new HistoryOperation
            {
                DateTime = dateTime,
                Id = id,
                Type = type,
                State = state,
                Amount = amount,
                Asset = asset,
                AssetPair = assetPair,
                Price = price
            };
        }
    }

    public class BaseCashOperation
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public string DateTime { get; set; }
        public string Asset { get; set; }
        public string BlockChainHash { get; set; }
        public bool IsRefund { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public bool IsSettled { get; set; }
        public string Type { get; set; }
        public string ContextOperationType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionStates State { get; set; }
    }

    public class CashInHistoryOperation : BaseCashOperation
    {

    }

    public class CashOutHistoryOperation : BaseCashOperation
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CashOutState CashOutState { get; set; }
    }

    public class TradeHistoryOperation
    {
        public string Id { get; set; }
        public string DateTime { get; set; }
        public string Asset { get; set; }
        public double Volume { get; set; }
        public bool IsSettled { get; set; }
        public string LimitOrderId { get; set; }
        public string MarketOrderId { get; set; }
        public string ContextOperationType { get; set; }
        public string State { get; set; }
    }
}