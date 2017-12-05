using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.Models;
using Common.Log;

namespace Lykke.Service.OperationsHistory.Services.InMemoryCache
{
    public class InMemoryCache: IHistoryCache
    {
        private readonly IHistoryLogEntryRepository _repository;
        private readonly OperationsHistorySettings _settings;
        private readonly ConcurrentDictionary<string, CacheModel> _storage;
        private readonly ILog _log;

        public InMemoryCache(IHistoryLogEntryRepository repository, OperationsHistorySettings setting, ILog log)
        {
            _repository = repository;
            _settings = setting;
            _storage = new ConcurrentDictionary<string, CacheModel>();
            _log = log;
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetRecordsByClient(string clientId)
        {
            if (_storage.TryGetValue(clientId, out CacheModel cachedValue))
            {
                return cachedValue.Records.Values;
            }

            var newCachedValue = await Load(clientId);

            return newCachedValue == null ? new List<IHistoryLogEntryEntity>() : newCachedValue.Records.Values;
        }

        public void AddOrUpdate(IHistoryLogEntryEntity item)
        {
            if (_storage.TryGetValue(item.ClientId, out CacheModel cachedCollection))
            {
                cachedCollection.Records.AddOrUpdate(item.Id, item, (key, oldValue) => item);

                return;
            }

            _log?.WriteWarningAsync(nameof(InMemoryCache), nameof(AddOrUpdate), $"clientId = {item.ClientId}",
                "No cache for clientId, new item will be ignored");
        }

        public async Task WarmUp(string clientId)
        {
            if (!_storage.ContainsKey(clientId))
            {
                await Load(clientId);
            }
        }

        private async Task<CacheModel> Load(string clientId)
        {
            var records = await _repository.GetByClientIdAsync(clientId);

            if (!records.Any())
                return null;

            var cacheModel = new CacheModel
            {
                Records = new ConcurrentDictionary<string, IHistoryLogEntryEntity>(
                    records
                        .OrderBy(r => r.DateTime)
                        .Select(x => new KeyValuePair<string, IHistoryLogEntryEntity>(x.Id, x)))
            };

            return _storage.AddOrUpdate(clientId, cacheModel, (key, oldValue) => cacheModel);
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAsync(string clientId, string operationType, string assetId, int take, int skip)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var result = clientRecords
                .Where(r => string.IsNullOrWhiteSpace(operationType) || r.OpType == operationType)
                .Where(r => string.IsNullOrWhiteSpace(assetId) || r.Currency == assetId)
                .Skip(skip)
                .Take(take);

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(result);
        }
    }
}
