using Lykke.Service.OperationsRepository.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Lykke.Service.OperationsRepository.Contract.Abstractions;

namespace Lykke.Service.OperationsHistory.Core
{
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
        public double FeeSize { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FeeType FeeType { get; set; }

        public static HistoryOperation Create(
            string id,
            DateTime dateTime,
            HistoryOperationType type,
            HistoryOperationState state,
            double amount,
            string asset,
            string assetPair,
            double? price = default(double?),
            double feeSize = 0,
            FeeType feeType = FeeType.Unknown)
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
                Price = price,
                FeeSize = feeSize,
                FeeType = feeType
            };
        }
    }
}
