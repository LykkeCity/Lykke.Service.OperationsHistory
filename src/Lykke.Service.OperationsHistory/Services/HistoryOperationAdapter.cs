using Common;
using Common.Log;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsRepository.Contract;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lykke.Service.OperationsRepository.Contract.Cash;
using Lykke.Service.OperationsRepository.Contract.Abstractions;

namespace Lykke.Service.OperationsHistory.Services
{
    //todo: simplify adapter

    public class HistoryOperationAdapter : IHistoryOperationAdapter
    {
        private readonly CachedDataDictionary<string, Asset> _assetsCache;
        private readonly CachedDataDictionary<string, AssetPair> _assetPairsCache;
        private readonly ILog _log;

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public HistoryOperationAdapter(
            CachedDataDictionary<string, Asset> assetsCache,
            CachedDataDictionary<string, AssetPair> assetPairsCache,
            ILog log)
        {
            _assetsCache = assetsCache ?? throw new ArgumentNullException(nameof(assetsCache));
            _assetPairsCache = assetPairsCache ?? throw new ArgumentNullException(nameof(assetPairsCache));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<HistoryOperation> Execute(HistoryLogEntryEntity historyEntity)
        {
            if (string.IsNullOrWhiteSpace(historyEntity.CustomData)) return null;

            var asset = (await _assetsCache.Values()).FirstOrDefault(x => x.Id == historyEntity.Currency);
            if (asset == null)
            {
                await _log.WriteWarningAsync(nameof(HistoryOperationAdapter), nameof(Execute),
                    $"Unable to find asset in dictionary for assetId = {historyEntity.Currency}, walletId = {historyEntity.ClientId}"); 
                return null;
            }

            var legacyOperationType = (OperationType)Enum.Parse(typeof(OperationType), historyEntity.OpType);

            CashInHistoryOperation cashIn = null;
            CashOutHistoryOperation cashOut = null;
            TradeHistoryOperation trade = null;

            switch (legacyOperationType)
            {
                case OperationType.CashInOut:
                    var cashInOut = JsonConvert.DeserializeObject<CashOperationDto>(historyEntity.CustomData);
                    cashIn = ConvertToCashIn(cashInOut, asset);
                    cashOut = ConvertToCashOut(cashInOut, asset);
                    break;
                case OperationType.CashOutAttempt:
                    var cashOutRequest = JsonConvert.DeserializeObject<CashOutRequestDto>(historyEntity.CustomData);
                    cashOut = ConvertToCashOut(cashOutRequest, asset);
                    break;
                case OperationType.ClientTrade:
                    var clientTrade = JsonConvert.DeserializeObject<ClientTradeDto>(historyEntity.CustomData);
                    trade = ConvertToTrade(clientTrade, asset);
                    break;
                case OperationType.LimitTradeEvent:
                    var limitTrade = JsonConvert.DeserializeObject<LimitTradeEventDto>(historyEntity.CustomData);
                    trade = await ConvertToTradeAsync(limitTrade, historyEntity.ClientId);
                    break;
                case OperationType.TransferEvent:
                    var transfer = JsonConvert.DeserializeObject<TransferEventDto>(historyEntity.CustomData);
                    if (historyEntity.ClientId == transfer.ClientId)
                    {
                        cashIn = ConvertToCashIn(transfer, asset);
                    }
                    if (historyEntity.ClientId == transfer.FromId)
                    {
                        cashOut = ConvertToCashOut(transfer, asset);
                    }
                    break;
                default:
                    throw new Exception($"Unknown operation type: {legacyOperationType.ToString()}");
            }

            var operationId = cashIn?.Id ??
                              cashOut?.Id ??
                              trade?.Id;

            return HistoryOperation.Create(
                id: operationId,
                dateTime: historyEntity.DateTime,
                cashIn: cashIn,
                cashout: cashOut,
                trade: trade
            );
        }

        private static CashInHistoryOperation ConvertToCashIn(ICashInOutOperation operation,
            Asset asset)
        {
            if (operation.Amount < 0) return null;

            var isSettled = operation.IsSettled ?? !string.IsNullOrEmpty(operation.BlockChainHash);

            var amount = operation.Amount.TruncateDecimalPlaces(asset.GetDisplayAccuracy());

            return new CashInHistoryOperation
            {
                DateTime = operation.DateTime.ToString(DateTimeFormat),
                Id = operation.Id,
                Amount = Math.Abs(amount),
                Asset = operation.AssetId,
                BlockChainHash = operation.BlockChainHash ?? string.Empty,
                IsRefund = operation.IsRefund,
                AddressFrom = operation.AddressFrom,
                AddressTo = operation.AddressTo,
                IsSettled = isSettled,
                Type = operation.Type.ToString(),
                State = operation.State,
                ContextOperationType = nameof(HistoryOperationType.CashIn)
            };
        }

        public static CashOutHistoryOperation ConvertToCashOut(ICashInOutOperation operation,
            Asset asset)
        {
            if (operation.Amount >= 0) return null;

            var isSettled = operation.IsSettled ?? !string.IsNullOrEmpty(operation.BlockChainHash);

            var amount = operation.Amount.TruncateDecimalPlaces(asset.GetDisplayAccuracy());

            return new CashOutHistoryOperation
            {
                DateTime = operation.DateTime.ToString(DateTimeFormat),
                Id = operation.Id,
                Amount = -Math.Abs(amount),
                Asset = operation.AssetId,
                BlockChainHash = operation.BlockChainHash ?? string.Empty,
                IsRefund = operation.IsRefund,
                AddressFrom = operation.AddressFrom,
                AddressTo = operation.AddressTo,
                IsSettled = isSettled,
                Type = operation.Type.ToString(),
                State = operation.State,
                ContextOperationType = nameof(HistoryOperationType.CashOut),
                CashOutState = CashOutState.Regular
            };
        }

        public static CashOutHistoryOperation ConvertToCashOut(ITransferEvent operation, Asset asset)
        {
            var isSettled = operation.IsSettled ?? !string.IsNullOrEmpty(operation.BlockChainHash);

            var amount = operation.Amount.TruncateDecimalPlaces(asset.GetDisplayAccuracy());

            return new CashOutHistoryOperation
            {
                Asset = operation.AssetId,
                AddressFrom = operation.AddressFrom,
                AddressTo = operation.AddressTo,
                Id = operation.Id,
                DateTime = operation.DateTime.ToString(DateTimeFormat),
                Type = string.Empty,
                BlockChainHash = operation.BlockChainHash ?? string.Empty,
                State = operation.State,
                IsSettled = isSettled,
                Amount = -Math.Abs(amount),
                ContextOperationType = nameof(HistoryOperationType.CashOut),
                IsRefund = false,
                CashOutState = CashOutState.Regular
            };
        }

        public static CashInHistoryOperation ConvertToCashIn(ITransferEvent operation, Asset asset)
        {
            var isSettled = operation.IsSettled ?? !string.IsNullOrEmpty(operation.BlockChainHash);

            var amount = operation.Amount.TruncateDecimalPlaces(asset.GetDisplayAccuracy());

            return new CashInHistoryOperation
            {
                Asset = operation.AssetId,
                AddressFrom = operation.AddressFrom,
                AddressTo = operation.AddressTo,
                Type = string.Empty,
                Id = operation.Id,
                DateTime = operation.DateTime.ToString(DateTimeFormat),
                BlockChainHash = operation.BlockChainHash ?? string.Empty,
                State = operation.State,
                IsSettled = isSettled,
                Amount = Math.Abs(amount),
                ContextOperationType = nameof(HistoryOperationType.CashIn),
                IsRefund = false
            };
        }

        public static CashOutHistoryOperation ConvertToCashOut(ICashOutRequest operation, Asset asset)
        {
            var isSettled = !string.IsNullOrEmpty(operation.BlockchainHash);

            return new CashOutHistoryOperation
            {
                Asset = operation.AssetId,
                AddressFrom = null,
                Type = nameof(CashOperationType.None),
                CashOutState = CashOutState.Request,
                Id = operation.Id,
                DateTime = operation.DateTime.ToString(DateTimeFormat),
                ContextOperationType = nameof(HistoryOperationType.CashOut),
                BlockChainHash = operation.BlockchainHash ?? string.Empty,
                State = operation.State,
                IsSettled = isSettled,
                Amount = operation.Amount.TruncateDecimalPlaces(asset.GetDisplayAccuracy()),
                IsRefund = false,
                AddressTo = null
            };
        }

        public static TradeHistoryOperation ConvertToTrade(ILimitTradeEvent operation, AssetPair assetPair,
            int accuracy)
        {
            var isBuy = operation.OrderType == OrderType.Buy;

            return new TradeHistoryOperation
            {
                DateTime = operation.CreatedDt.ToString(DateTimeFormat),
                Id = operation.Id,
                Asset = operation.AssetId,
                MarketOrderId = null,
                LimitOrderId = operation.OrderId,
                Volume = Math.Abs(operation.Volume).TruncateDecimalPlaces(accuracy, isBuy),
                ContextOperationType = nameof(HistoryOperationType.Trade),
                State = string.Empty,
                IsSettled = false
            };

        }

        public static TradeHistoryOperation ConvertToTrade(IClientTrade operation, Asset asset)
        {
            var isSettled = !string.IsNullOrEmpty(operation.BlockChainHash);

            return new TradeHistoryOperation
            {
                DateTime = operation.DateTime.ToString(DateTimeFormat),
                Id = operation.Id,
                Asset = operation.AssetId,
                Volume = operation.Amount.TruncateDecimalPlaces(asset.Accuracy),
                IsSettled = isSettled,
                State = operation.State.ToString(),
                MarketOrderId = operation.MarketOrderId,
                LimitOrderId = operation.LimitOrderId,
                ContextOperationType = nameof(HistoryOperationType.Trade)
            };
        }

        private async Task<TradeHistoryOperation> ConvertToTradeAsync(LimitTradeEventDto model, string walletId)
        {
            if (model == null) return null;

            var assetPair = (await _assetPairsCache.Values()).FirstOrDefault(x => x.Id == model.AssetPair);
            if (assetPair == null)
            {
                await _log.WriteWarningAsync(nameof(HistoryOperationAdapter), nameof(ConvertToTradeAsync),
                    $"Unable to find asset pair in dictionary for assetPairId = {model.AssetPair}, walletId = {walletId}");
                return null;
            }

            var asset = (await _assetsCache.Values()).FirstOrDefault(x => x.Id == assetPair.QuotingAssetId);
            if (asset == null)
            {
                await _log.WriteWarningAsync(nameof(HistoryOperationAdapter), nameof(ConvertToTradeAsync),
                    $"Unable to find asset in dictionary for limit trade quoting assetId = {assetPair.QuotingAssetId}, walletId = {walletId}");
                return null;
            }

            return ConvertToTrade(model, assetPair, asset.GetDisplayAccuracy());
        }
    }
}