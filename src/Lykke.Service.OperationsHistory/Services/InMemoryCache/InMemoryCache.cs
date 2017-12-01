using System;
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

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllPagedAsync(string clientId, int page)
        {
            return await InternalGetAllAsync(
                clientId, 
                GetTopValueForPagedApi(),
                GetSkipValueForPagedApi(page));
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, int top, int skip)
        {
            return await InternalGetAllAsync(clientId, top, skip);
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllPagedAsync(string clientId, string assetId, string operationType, int page)
        {
            return await InternalGetAllAsync(
                clientId, 
                assetId, 
                operationType, 
                GetTopValueForPagedApi(), 
                GetSkipValueForPagedApi(page));
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, string assetId, string operationType, int top, int skip)
        {
            return await InternalGetAllAsync(clientId, assetId, operationType, top, skip);
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllByOpTypePagedAsync(string clientId, string operationType, int page)
        {
            return await InternalGetAllByOpTypeAsync(
                clientId, 
                operationType, 
                GetTopValueForPagedApi(),
                GetSkipValueForPagedApi(page));
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllByOpTypeAsync(string clientId, string operationType, int top, int skip)
        {
            return await InternalGetAllByOpTypeAsync(clientId, operationType, top, skip);
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllByAssetPagedAsync(string clientId, string assetId, int page)
        {
            return await InternalGetAllByAssetAsync(
                clientId, 
                assetId, 
                GetTopValueForPagedApi(),
                GetSkipValueForPagedApi(page));
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllByAssetAsync(string clientId, string assetId, int top, int skip)
        {
            return await InternalGetAllByAssetAsync(clientId, assetId, top, skip);
        }

        private async Task<IEnumerable<HistoryEntryResponse>> InternalGetAllAsync(string clientId, int top, int skip)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var pagedResult = clientRecords
                .Skip(skip)
                .Take(top)
                .ToList();

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(pagedResult);
        }

        private async Task<IEnumerable<HistoryEntryResponse>> InternalGetAllAsync(string clientId, string assetId, string operationType,
            int top, int skip)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var pagedResult = clientRecords
                .Where(r => r.Currency == assetId && r.OpType == operationType)
                .Skip(skip)
                .Take(top)
                .ToList();

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(pagedResult);
        }

        private async Task<IEnumerable<HistoryEntryResponse>> InternalGetAllByOpTypeAsync(string clientId, string operationType, int top, int skip)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var pagedResult = clientRecords
                .Where(r => r.OpType == operationType)
                .Skip(skip)
                .Take(top)
                .ToList();

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(pagedResult);
        }

        private async Task<IEnumerable<HistoryEntryResponse>> InternalGetAllByAssetAsync(string clientId, string assetId, int top, int skip)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var pagedResult = clientRecords
                .Where(r => r.Currency == assetId)
                .Skip(skip)
                .Take(top)
                .ToList();

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(pagedResult);
        }

        private int GetSkipValueForPagedApi(int page)
        {
            return (page - 1) * _settings.ValuesPerPage;
        }

        private int GetTopValueForPagedApi()
        {
            return _settings.ValuesPerPage;
        }

        public async Task<IEnumerable<IHistoryLogEntryEntity>> GetRecordsByClient(string clientId)
        {
            if (_storage.TryGetValue(clientId, out CacheModel cachedValue))
            {
                return cachedValue.Records.Values;
            }

            var recordsFromStorage = await _repository.GetByClientIdAsync(clientId);
            var newCacheObject = new CacheModel
            {
                Records = new ConcurrentDictionary<string, IHistoryLogEntryEntity>(
                    recordsFromStorage
                        .OrderBy(r => r.DateTime)
                        .Select(x => new KeyValuePair<string, IHistoryLogEntryEntity>(x.Id, x)))
            };

            var newCachedValue = _storage.AddOrUpdate(clientId, newCacheObject, (key, oldValue) => newCacheObject);

            return newCachedValue.Records.Values;
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
    }
}
