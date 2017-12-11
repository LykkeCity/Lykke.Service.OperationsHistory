using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;
using Common.Log;
using Lykke.Service.OperationsHistory.Core.Domain;

namespace Lykke.Service.OperationsHistory.Services.InMemoryCache
{
    public class InMemoryCache: IHistoryCache
    {
        private readonly IHistoryLogEntryRepository _repository;
        private readonly ConcurrentDictionary<string, CacheModel> _storage;
        private readonly ILog _log;

        public InMemoryCache(IHistoryLogEntryRepository repository, ILog log)
        {
            _repository = repository;
            _storage = new ConcurrentDictionary<string, CacheModel>();
            _log = log;
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetRecordsByWalletId(string walletId)
        {
            if (_storage.TryGetValue(walletId, out CacheModel cachedValue))
            {
                return cachedValue.Records.Values;
            }

            var newCachedValue = await Load(walletId);

            return newCachedValue == null ? new List<IHistoryLogEntryEntity>() : newCachedValue.Records.Values;
        }

        public void AddOrUpdate(IHistoryLogEntryEntity item)
        {
            if (_storage.TryGetValue(item.ClientId, out CacheModel cachedCollection))
            {
                cachedCollection.Records.AddOrUpdate(item.Id, item, (key, oldValue) => item);

                return;
            }

            _log?.WriteWarningAsync(nameof(InMemoryCache), nameof(AddOrUpdate), $"walletId = {item.ClientId}",
                "No cache for walletId, new item will be ignored");
        }

        public async Task WarmUp(string walletId)
        {
            if (!_storage.ContainsKey(walletId))
            {
                await Load(walletId);
            }
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

            return _storage.AddOrUpdate(walletId, cacheModel, (key, oldValue) => cacheModel);
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetAsync(string walletId, string operationType, string assetId, PaginationInfo paging = null)
        {
            var walletRecords = await GetRecordsByWalletId(walletId);

            var operationIsEmpty = string.IsNullOrWhiteSpace(operationType);
            var assetIsEmpty = string.IsNullOrWhiteSpace(assetId);

            var result = walletRecords
                .Where(r => operationIsEmpty || r.OpType == operationType)
                .Where(r => assetIsEmpty || r.Currency == assetId);

            if (paging != null)
            {
                result = result
                    .Skip(paging.Skip)
                    .Take(paging.Take);
            }

            return result.OrderByDescending(x => x.DateTime);
        }
    }
}
