using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Services.InMemoryCache
{
    public class InMemoryCache: IHistoryCache
    {
        private readonly IHistoryLogEntryRepository _repository;
        private readonly OperationsHistorySettings _settings;
        private readonly IDictionary<string, CacheModel> _storage;

        public InMemoryCache(IHistoryLogEntryRepository repository, OperationsHistorySettings setting)
        {
            _repository = repository;
            _settings = setting;
            _storage = new Dictionary<string, CacheModel>();
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
            var needUpdateOrCreate = true;
            if (_storage.TryGetValue(clientId, out CacheModel cachedValue))
            {
                needUpdateOrCreate = DateTime.UtcNow.Subtract(cachedValue.LastUpdated).TotalSeconds >= _settings.CacheExpiration;
            }

            if (needUpdateOrCreate)
            {
                var records = await _repository.GetAllAsync(clientId);
                var updatedObject = new CacheModel
                {
                    LastUpdated = DateTime.UtcNow,
                    Records = records.OrderBy(r => r.DateTime).AsQueryable()
                };

                if (_storage.Keys.Contains(clientId))
                {
                    _storage[clientId] = updatedObject;
                }
                else
                {
                    _storage.Add(clientId, updatedObject);
                }
            }

            return _storage[clientId].Records;
        }
    }
}
