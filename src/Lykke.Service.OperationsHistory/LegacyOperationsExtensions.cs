using Common;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsRepository.Contract;
using Lykke.Service.OperationsRepository.Contract.Abstractions;
using System;

namespace Lykke.Service.OperationsHistory
{
    public static class LegacyOperationsExtensions
    {
        private static string _dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        private static int _assetDefaultDisplayAccuracy = 2;

        public static bool GetIsSettled(this IBaseCashBlockchainOperation operation)
        {
            return operation.IsSettled ?? !string.IsNullOrEmpty(operation.BlockChainHash);
        }

        public static double Normalize(this double amount, Asset asset, bool toUpper = false)
        {
            return amount.TruncateDecimalPlaces(asset?.GetDisplayAccuracy() ?? _assetDefaultDisplayAccuracy, toUpper);
        }

        public static CashInHistoryOperation ConvertToCashIn(this ICashInOutOperation operation, Asset asset)
        {
            var amount = operation.Amount.Normalize(asset);

            if (amount < 0) return null;

            return new CashInHistoryOperation
            {
                DateTime = operation.DateTime.ToString(_dateTimeFormat),
                Id = operation.Id,
                Amount = Math.Abs(amount),
                Asset = operation.AssetId,
                BlockChainHash = operation.BlockChainHash ?? string.Empty,
                IsRefund = operation.IsRefund,
                AddressFrom = operation.AddressFrom,
                AddressTo = operation.AddressTo,
                IsSettled = operation.GetIsSettled(),
                Type = operation.Type.ToString(),
                State = operation.State,
                ContextOperationType = nameof(HistoryOperationType.CashIn)
            };
        }

        public static CashOutHistoryOperation ConvertToCashOut(this ICashInOutOperation operation, Asset asset)
        {
            var amount = operation.Amount.Normalize(asset);

            if (amount >= 0) return null;

            return new CashOutHistoryOperation
            {
                DateTime = operation.DateTime.ToString(_dateTimeFormat),
                Id = operation.Id,
                Amount = -Math.Abs(amount),
                Asset = operation.AssetId,
                BlockChainHash = operation.BlockChainHash ?? string.Empty,
                IsRefund = operation.IsRefund,
                AddressFrom = operation.AddressFrom,
                AddressTo = operation.AddressTo,
                IsSettled = operation.GetIsSettled(),
                Type = operation.Type.ToString(),
                State = operation.State,
                ContextOperationType = nameof(HistoryOperationType.CashOut),
                CashOutState = CashOutState.Regular
            };
        }

        public static CashOutHistoryOperation ConvertToCashOut(this ITransferEvent operation, Asset asset)
        {
            var amount = operation.Amount.Normalize(asset);

            return new CashOutHistoryOperation
            {
                Asset = operation.AssetId,
                AddressFrom = operation.AddressFrom,
                AddressTo = operation.AddressTo,
                Id = operation.Id,
                DateTime = operation.DateTime.ToString(_dateTimeFormat),
                Type = string.Empty,
                BlockChainHash = operation.BlockChainHash ?? string.Empty,
                State = operation.State,
                IsSettled = operation.GetIsSettled(),
                Amount = -Math.Abs(amount),
                ContextOperationType = nameof(HistoryOperationType.CashOut),
                IsRefund = false,
                CashOutState = CashOutState.Regular
            };
        }

        public static CashInHistoryOperation ConvertToCashIn(this ITransferEvent operation, Asset asset)
        {
            var amount = operation.Amount.Normalize(asset);

            return new CashInHistoryOperation
            {
                Asset = operation.AssetId,
                AddressFrom = operation.AddressFrom,
                AddressTo = operation.AddressTo,
                Type = string.Empty,
                Id = operation.Id,
                DateTime = operation.DateTime.ToString(_dateTimeFormat),
                BlockChainHash = operation.BlockChainHash ?? string.Empty,
                State = operation.State,
                IsSettled = operation.GetIsSettled(),
                Amount = Math.Abs(amount),
                ContextOperationType = nameof(HistoryOperationType.CashIn),
                IsRefund = false
            };
        }

        public static CashOutHistoryOperation ConvertToCashOut(this ICashOutRequest operation, Asset asset)
        {
            var amount = operation.Amount.Normalize(asset);

            return new CashOutHistoryOperation
            {
                Asset = operation.AssetId,
                AddressFrom = null,
                Type = nameof(CashOperationType.None),
                CashOutState = CashOutState.Request,
                Id = operation.Id,
                DateTime = operation.DateTime.ToString(_dateTimeFormat),
                ContextOperationType = nameof(HistoryOperationType.CashOut),
                BlockChainHash = operation.BlockchainHash ?? string.Empty,
                State = operation.State,
                IsSettled = !string.IsNullOrEmpty(operation.BlockchainHash),
                Amount = amount,
                IsRefund = false,
                AddressTo = null
            };
        }

        public static TradeHistoryOperation ConvertToTrade(this ILimitTradeEvent operation, Asset asset)
        {
            var isBuy = operation.OrderType == OrderType.Buy;

            var volume = operation.Volume.Normalize(asset, isBuy);

            return new TradeHistoryOperation
            {
                DateTime = operation.CreatedDt.ToString(_dateTimeFormat),
                Id = operation.Id,
                Asset = operation.AssetId,
                MarketOrderId = null,
                LimitOrderId = operation.OrderId,
                Volume = Math.Abs(volume),
                ContextOperationType = nameof(HistoryOperationType.Trade),
                State = string.Empty,
                IsSettled = false
            };

        }

        public static TradeHistoryOperation ConvertToTrade(this IClientTrade operation, Asset asset)
        {
            var volume = operation.Amount.Normalize(asset);

            return new TradeHistoryOperation
            {
                DateTime = operation.DateTime.ToString(_dateTimeFormat),
                Id = operation.Id,
                Asset = operation.AssetId,
                Volume = volume,
                IsSettled = !string.IsNullOrEmpty(operation.BlockChainHash),
                State = operation.State.ToString(),
                MarketOrderId = operation.MarketOrderId,
                LimitOrderId = operation.LimitOrderId,
                ContextOperationType = nameof(HistoryOperationType.Trade)
            };
        }
        
        private static HistoryOperationState GetState(TransactionStates state)
        {
            if (state == TransactionStates.InProcessOffchain ||
                state == TransactionStates.InProcessOffchain)
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
                operation.Price);
        }

        public static HistoryOperation ConvertToHistoryOperation(this ILimitTradeEvent operation, Asset asset)
        {
            var isBuy = operation.OrderType == OrderType.Buy;

            var volume = operation.Volume.Normalize(asset, isBuy);
            Console.WriteLine(operation.Status.ToString());
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
                operation.Id,
                operation.DateTime,
                type,
                type == HistoryOperationType.CashIn ? HistoryOperationState.Finished : GetState(operation.State),
                Math.Abs(amount),
                operation.AssetId,
                null,
                null);
        }
    }
}