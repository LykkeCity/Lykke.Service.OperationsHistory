using Common;
using Common.Log;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsRepository.Contract;
using System;
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

        public async Task<HistoryOperation> ExecuteAsync(HistoryLogEntryEntity historyEntity)
        {
            var asset = await GetAssetByIdAsync(historyEntity.Currency);

            var legacyOperationType = (OperationType) Enum.Parse(typeof(OperationType), historyEntity.OpType);
            
            switch (legacyOperationType)
            {
                case OperationType.CashInOut:
                    var cashInOut = JsonConvert.DeserializeObject<CashOperationDto>(historyEntity.CustomData);
                    return cashInOut?.ConvertToHistoryOperation(asset);
                case OperationType.CashOutAttempt:
                    var cashOutRequest = JsonConvert.DeserializeObject<CashOutRequestDto>(historyEntity.CustomData);
                    return cashOutRequest?.ConvertToHistoryOperation(asset);
                case OperationType.ClientTrade:
                    var clientTrade = JsonConvert.DeserializeObject<ClientTradeDto>(historyEntity.CustomData);
                    return clientTrade?.ConvertToHistoryOperation(asset);
                case OperationType.LimitTradeEvent:
                    var limitTrade = JsonConvert.DeserializeObject<LimitTradeEventDto>(historyEntity.CustomData);
                    var limitAssetPair = await GetAssetPairByIdAsync(limitTrade?.AssetPair);
                    var limitAsset = await GetAssetByIdAsync(limitAssetPair?.BaseAssetId);
                    return limitTrade?.ConvertToHistoryOperation(limitAsset);
                case OperationType.TransferEvent:
                    var transfer = JsonConvert.DeserializeObject<TransferEventDto>(historyEntity.CustomData);
                    return transfer?.ConvertToHistoryOperation(asset);
                default:
                    throw new Exception($"Unknown operation type: {legacyOperationType.ToString()}");
            }
        }

        private async Task<Asset> GetAssetByIdAsync(string assetId)
        {
            if (string.IsNullOrEmpty(assetId)) return null;

            var asset = await _assetsCache.GetItemAsync(assetId);
            return asset;
        }

        private async Task<AssetPair> GetAssetPairByIdAsync(string assetPairId)
        {
            if (string.IsNullOrEmpty(assetPairId)) return null;

            var assetPair = await _assetPairsCache.GetItemAsync(assetPairId);
            return assetPair;
        }
    }
}