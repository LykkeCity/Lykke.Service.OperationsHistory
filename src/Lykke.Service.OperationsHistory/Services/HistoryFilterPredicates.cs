using System;
using Lykke.Service.OperationsHistory.Core;

namespace Lykke.Service.OperationsHistory.Services
{
    public static class HistoryOperationFilterPredicates
    {
        public static Func<HistoryOperation, bool> IfTypeEquals(string operationType)
        {
            return operation =>
            {
                if (string.IsNullOrWhiteSpace(operationType)) return true;
                switch (operationType)
                {
                    case "CashIn": return operation.CashIn != null;
                    case "CashOut": return operation.CashOut != null;
                    case "Trade": return operation.Trade != null;
                    default: return false;
                }
            };
        }

        public static Func<HistoryOperation, bool> IfAssetEquals(string assetId)
        {
            return operation =>
            {
                if (string.IsNullOrWhiteSpace(assetId)) return true;
                var operationAssetId = operation.CashIn?.Asset ?? operation.CashOut?.Asset ?? operation.Trade?.Asset;
                return operationAssetId == assetId;
            };
        }
    }
}