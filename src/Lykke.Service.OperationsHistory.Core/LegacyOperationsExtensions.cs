﻿using Common;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsRepository.Contract;
using Lykke.Service.OperationsRepository.Contract.Abstractions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Lykke.Service.OperationsHistory.Core
{
    public static class LegacyOperationsExtensions
    {
        private static string _dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        private static int _assetDefaultDisplayAccuracy = 2;

        private static double Normalize(this double amount, Asset asset, bool toUpper = false)
        {
            return amount.TruncateDecimalPlaces(asset?.GetDisplayAccuracy() ?? _assetDefaultDisplayAccuracy, toUpper);
        }
        
        private static HistoryOperationState GetState(TransactionStates state)
        {
            if (state == TransactionStates.InProcessOffchain ||
                state == TransactionStates.InProcessOnchain)
                return HistoryOperationState.InProgress;

            return HistoryOperationState.Finished;
        }
        
        private static HistoryOperationState GetState(OrderStatus state)
        {
            if (state == OrderStatus.Cancelled)
                return HistoryOperationState.Canceled;
            if (state == OrderStatus.Matched)
                return HistoryOperationState.Finished;
            if (state == OrderStatus.Processing ||
                state == OrderStatus.InOrderBook)
                return HistoryOperationState.InProgress;
            return HistoryOperationState.Failed;
        }
        
        public static HistoryOperation ConvertToHistoryOperation(this ICashInOutOperation operation, Asset asset)
        {
            var amount = operation.Amount.Normalize(asset);

            var type = amount > 0 ? HistoryOperationType.CashIn : HistoryOperationType.CashOut;
            
            return HistoryOperation.Create(
                operation.Id,
                operation.DateTime,
                type,
                type == HistoryOperationType.CashIn ? HistoryOperationState.Finished : GetState(operation.State),
                Math.Abs(amount),
                operation.AssetId,
                null,
                null);
        }
        
        public static HistoryOperation ConvertToHistoryOperation(this ICashOutRequest operation, Asset asset)
        {
            var amount = operation.Amount.Normalize(asset);

            return HistoryOperation.Create(
                operation.Id,
                operation.DateTime,
                HistoryOperationType.CashOut,
                GetState(operation.State),
                Math.Abs(amount),
                operation.AssetId,
                null,
                null);
        }

        public static HistoryOperation ConvertToHistoryOperation(this IClientTrade operation, Asset asset)
        {
            var volume = operation.Amount.Normalize(asset);

            return HistoryOperation.Create(
                operation.Id,
                operation.DateTime,
                operation.IsLimitOrderResult ? HistoryOperationType.LimitTrade : HistoryOperationType.Trade,
                GetState(operation.State),
                volume,
                operation.AssetId,
                operation.AssetPairId,
                operation.Price,
                operation.FeeSize,
                operation.FeeType);
        }

        public static HistoryOperation ConvertToHistoryOperation(this ILimitTradeEvent operation, Asset asset)
        {
            var isBuy = operation.OrderType == OrderType.Buy;

            var volume = operation.Volume.Normalize(asset, isBuy);
            
            return HistoryOperation.Create(
                operation.Id,
                operation.CreatedDt,
                HistoryOperationType.LimitOrderEvent,
                GetState(operation.Status),
                volume,
                operation.AssetId,
                operation.AssetPair,
                operation.Price);
        }
        
        public static HistoryOperation ConvertToHistoryOperation(this ITransferEvent operation, Asset asset)
        {
            var amount = operation.Amount.Normalize(asset);

            var type = amount > 0 ? HistoryOperationType.CashIn : HistoryOperationType.CashOut;
            
            return HistoryOperation.Create(
                MakeGuidFromPair(operation.ClientId, operation.Id).ToString(),
                operation.DateTime,
                type,
                type == HistoryOperationType.CashIn ? HistoryOperationState.Finished : GetState(operation.State),
                Math.Abs(amount),
                operation.AssetId,
                null,
                null);
        }
        
        private static Guid MakeGuidFromPair(string s1, string s2)
        {
            var arr = new byte[16];

            Array.Copy(new SHA256Managed()
                .ComputeHash(
                    Encoding.ASCII.GetBytes(
                        string.Concat(s1, s2))), 0, arr, 0, 16);
            
            return new Guid(arr);
        }
    }
    
    public static class AssetExtensions
    {
        public static int GetDisplayAccuracy(this Asset asset)
        {
            return asset.DisplayAccuracy ?? asset.Accuracy;
        }
    }
}
