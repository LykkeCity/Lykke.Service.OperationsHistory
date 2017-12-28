using Lykke.Service.OperationsRepository.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Lykke.Service.OperationsHistory.Models
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
        None = 0,
        CashIn,
        CashOut,
        Trade
    }

    /// <summary>
    /// Represents any operation logged to history
    /// </summary>
    public class HistoryOperationModel
    {
        public string Id { get; set; }
        public DateTime DateTime { get; set; }
        public CashInHistoryOperationModel CashIn { get; set; }
        public CashOutHistoryOperationModel CashOut { get; set; }
        public TradeHistoryOperationModel Trade { get; set; }

        public static HistoryOperationModel Create(
            string id,
            DateTime dateTime,
            TradeHistoryOperationModel trade = null,
            CashInHistoryOperationModel cashIn = null,
            CashOutHistoryOperationModel cashout = null)
        {
            return new HistoryOperationModel
            {
                DateTime = dateTime,
                Id = id,
                CashIn = cashIn,
                CashOut = cashout,
                Trade = trade
            };
        }
    }

    public class BaseCashOperationModel
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

    public class CashInHistoryOperationModel : BaseCashOperationModel
    {

    }

    public class CashOutHistoryOperationModel : BaseCashOperationModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CashOutState CashOutState { get; set; }
    }

    public class TradeHistoryOperationModel
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