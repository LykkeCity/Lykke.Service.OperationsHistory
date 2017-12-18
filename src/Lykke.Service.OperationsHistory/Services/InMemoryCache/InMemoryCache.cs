using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Entities;
using Common.Log;
using Lykke.Service.OperationsHistory.Core.Domain;

namespace Lykke.Service.OperationsHistory.Services.InMemoryCache
{
    public class InMemoryCache: IHistoryCache
    {
        private readonly IHistoryLogEntryRepository _repository;
        private readonly ConcurrentDictionary<string, CacheModel> _cache;
        private readonly ILog _log;

        public InMemoryCache(IHistoryLogEntryRepository repository, ILog log)
        {
            _repository = repository;
            _cache = new ConcurrentDictionary<string, CacheModel>();
            _log = log;
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetRecordsByWalletId(string walletId)
        {
            if (_cache.TryGetValue(walletId, out CacheModel cachedValue))
            {
                return cachedValue.Records.Values;
            }

            var newCachedValue = await Load(walletId);

            return newCachedValue == null ? new List<IHistoryLogEntryEntity>() : newCachedValue.Records.Values;
        }

        public async Task AddOrUpdate(IHistoryLogEntryEntity item)
        {
            if (!_cache.TryGetValue(item.ClientId, out CacheModel cachedCollection))
            {
                cachedCollection = await Load(item.ClientId);
            }
            // make sure new item added to cache even if it was loaded from repository
            cachedCollection?.Records.AddOrUpdate(item.Id, item, (key, oldValue) => item);
        }

        public async Task WarmUp(string walletId)
        {
            if (!_cache.ContainsKey(walletId))
            {
                await Load(walletId);
            }
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetAsync(string walletId, string operationType, string assetId, PaginationInfo paging = null)
        {
            var walletRecords = await GetRecordsByWalletId(walletId);

            var operationIsEmpty = string.IsNullOrWhiteSpace(operationType);
            var assetIsEmpty = string.IsNullOrWhiteSpace(assetId);

            var result = walletRecords
                .Where(r => operationIsEmpty || r.OpType == operationType)
                .Where(r => assetIsEmpty || r.Currency == assetId)
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

            var cacheModel = new CacheModel
            {
                Records = new ConcurrentDictionary<string, IHistoryLogEntryEntity>(
                    records
                        .OrderByDescending(r => r.DateTime)
                        .Select(x => new KeyValuePair<string, IHistoryLogEntryEntity>(x.Id, x)))
            };

            return _cache.AddOrUpdate(walletId, cacheModel, (key, oldValue) => cacheModel);
        }
    }
}
