using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Entities;
using Common.Log;
using Lykke.Service.OperationsHistory.Core.Domain;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsRepository.Contract;

namespace Lykke.Service.OperationsHistory.Services
{
    public class InMemoryCache: IHistoryOperationsCache
    {
        private class CacheModel
        {
            public ConcurrentDictionary<string, HistoryOperation> Records;
        }

        private readonly IHistoryLogEntryRepository _repository;
        private readonly ConcurrentDictionary<string, CacheModel> _cache;
        private readonly IHistoryOperationAdapter _adapter;
        private readonly ILog _log;

        public InMemoryCache(
            IHistoryLogEntryRepository repository, 
            IHistoryOperationAdapter adapter, 
            ILog log)
        {
            _repository = repository;
            _cache = new ConcurrentDictionary<string, CacheModel>();
            _adapter = adapter;
            _log = log;
        }

        public async Task<IEnumerable<HistoryOperation>> GetRecordsByWalletId(string walletId)
        {
            if (_cache.TryGetValue(walletId, out CacheModel cachedValue))
            {
                return cachedValue.Records.Values;
            }

            var newCachedValue = await Load(walletId);

            return newCachedValue?.Records.Values ?? Enumerable.Empty<HistoryOperation>();
        }

        public async Task AddOrUpdate(string walletId, HistoryOperation item)
        {
            if (!_cache.TryGetValue(walletId, out CacheModel cachedCollection))
            {
                cachedCollection = await Load(walletId);
            }
            // make sure new item added to cache even if it was loaded from repository
            cachedCollection?.Records.AddOrUpdate(item.Id, item, (key, oldValue) => item);
        }

        public async Task RemoveIfLoaded(string walletId, string operationId)
        {
            if (!_cache.TryGetValue(walletId, out CacheModel cachedCollection))
            {
                return;
            }

            cachedCollection?.Records.TryRemove(operationId, out HistoryOperation _);
        }

        public async Task WarmUp(string walletId)
        {
            if (!_cache.ContainsKey(walletId))
            {
                await Load(walletId);
            }
        }

        public async Task<IEnumerable<HistoryOperation>> GetAsync(string walletId,
            HistoryOperationType[] operationTypes = null, string assetId = null, string assetPairId = null,
            PaginationInfo paging = null)
        {
            var walletRecords = await GetRecordsByWalletId(walletId);
            
            var result = walletRecords
                .Where(HistoryOperationFilterPredicates.IfTypeEquals(operationTypes))
                .Where(HistoryOperationFilterPredicates.IfAssetEquals(assetId))
                .Where(HistoryOperationFilterPredicates.IfAssetPairEquals(assetPairId))
                .OrderByDescending(x => x.DateTime);

            if (paging != null)
            {
                return result
                    .Skip(paging.Skip)
                    .Take(paging.Take);
            }

            return result;
        }

        private async Task<CacheModel> Load(string walletId)
        {
            var records = await _repository.GetByWalletIdAsync(walletId);
            
            if (!records.Any())
                return null;
            
            var adaptedOperations = await Task.WhenAll(records.Select(x => _adapter.ExecuteAsync(x)));

            var cacheModel = new CacheModel
            {
                Records = new ConcurrentDictionary<string, HistoryOperation>(
                    adaptedOperations
                        .OrderByDescending(r => r.DateTime)
                        .Select(x => new KeyValuePair<string, HistoryOperation>(x.Id, x)))
            };

            return _cache.AddOrUpdate(walletId, cacheModel, (key, oldValue) => cacheModel);
        }
    }
}
