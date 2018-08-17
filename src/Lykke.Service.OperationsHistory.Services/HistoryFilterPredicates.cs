using System;
using System.Linq;
using Lykke.Service.OperationsHistory.Core;

namespace Lykke.Service.OperationsHistory.Services
{
    public static class HistoryOperationFilterPredicates
    {
        public static Func<HistoryOperation, bool> IfTypeEquals(HistoryOperationType? operationType)
        {
            return operation => operationType == null || operation.Type == operationType;
        }

        public static Func<HistoryOperation, bool> IfAssetEquals(string assetId)
        {
            return operation =>
            {
                if (string.IsNullOrWhiteSpace(assetId)) return true;
                
                return operation.Asset == assetId;
            };
        }

        public static Func<HistoryOperation, bool> IfAssetPairEquals(string assetPairId)
        {
            return operation =>
            {
                if (string.IsNullOrWhiteSpace(assetPairId)) return true;
                
                return operation.AssetPair == assetPairId;
            };
        }

        public static Func<HistoryOperation, bool> IfTypeEquals(HistoryOperationType[] operationTypes)
        {
            return operation => operationTypes == null || !operationTypes.Any()  || operationTypes.Contains(operation.Type);
        }
    }
}