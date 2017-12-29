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

namespace Lykke.Service.OperationsHistory.Services
{
    public class HistoryOperationAdapter : IHistoryOperationAdapter
    {
        private readonly CachedDataDictionary<string, Asset> _assetsCache;
        private readonly CachedDataDictionary<string, AssetPair> _assetPairsCache;
        private readonly ILog _log;

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
            var asset = await GetAssetByIdAsync(historyEntity.Currency);

            var legacyOperationType = (OperationType) Enum.Parse(typeof(OperationType), historyEntity.OpType);

            CashInHistoryOperation cashIn = null;
            CashOutHistoryOperation cashOut = null;
            TradeHistoryOperation trade = null;

            switch (legacyOperationType)
            {
                case OperationType.CashInOut:
                    var cashInOut = JsonConvert.DeserializeObject<CashOperationDto>(historyEntity.CustomData);
                    cashIn = cashInOut?.ConvertToCashIn(asset);
                    cashOut = cashInOut?.ConvertToCashOut(asset);
                    break;
                case OperationType.CashOutAttempt:
                    var cashOutRequest = JsonConvert.DeserializeObject<CashOutRequestDto>(historyEntity.CustomData);
                    cashOut = cashOutRequest?.ConvertToCashOut(asset);
                    break;
                case OperationType.ClientTrade:
                    var clientTrade = JsonConvert.DeserializeObject<ClientTradeDto>(historyEntity.CustomData);
                    trade = clientTrade?.ConvertToTrade(asset);
                    break;
                case OperationType.LimitTradeEvent:
                    var limitTrade = JsonConvert.DeserializeObject<LimitTradeEventDto>(historyEntity.CustomData);
                    var limitAssetPair = await GetAssetPairByIdAsync(limitTrade?.AssetPair);
                    var limitAsset = await GetAssetByIdAsync(limitAssetPair?.QuotingAssetId);
                    trade = limitTrade?.ConvertToTrade(limitAsset);
                    break;
                case OperationType.TransferEvent:
                    var transfer = JsonConvert.DeserializeObject<TransferEventDto>(historyEntity.CustomData);
                    if (historyEntity.ClientId == transfer.ClientId)
                    {
                        cashIn = transfer.ConvertToCashIn(asset);
                    }
                    if (historyEntity.ClientId == transfer.FromId)
                    {
                        cashOut = transfer.ConvertToCashOut(asset);
                    }
                    break;
                default:
                    throw new Exception($"Unknown operation type: {legacyOperationType.ToString()}");
            }

            var operationId = cashIn?.Id ?? cashOut?.Id ?? trade?.Id;

            return HistoryOperation.Create(
                id: operationId,
                dateTime: historyEntity.DateTime,
                cashIn: cashIn,
                cashout: cashOut,
                trade: trade
            );
        }

        private async Task<Asset> GetAssetByIdAsync(string assetId)
        {
            if (string.IsNullOrEmpty(assetId)) return null;

            var cachedValues = await _assetsCache.Values();

            return cachedValues.FirstOrDefault(x => x.Id == assetId);
        }

        private async Task<AssetPair> GetAssetPairByIdAsync(string assetPairId)
        {
            if (string.IsNullOrEmpty(assetPairId)) return null;

            var cachedValues = await _assetPairsCache.Values();

            return cachedValues.FirstOrDefault(x => x.Id == assetPairId);
        }
    }
}