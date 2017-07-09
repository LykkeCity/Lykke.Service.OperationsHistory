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
        public async Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, int page)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var pagedResult = clientRecords
                .Skip((page - 1) * _settings.ValuesPerPage)
                .Take(_settings.ValuesPerPage)
                .ToList();

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(pagedResult);
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, string assetId, string operationType, int page)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var pagedResult = clientRecords
                .Where(r => r.Currency == assetId && r.OpType == operationType)
                .Skip((page - 1) * _settings.ValuesPerPage)
                .Take(_settings.ValuesPerPage)
                .ToList();

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(pagedResult);
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllByOpTypeAsync(string clientId, string operationType, int page)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var pagedResult = clientRecords
                .Where(r => r.OpType == operationType)
                .Skip((page - 1) * _settings.ValuesPerPage)
                .Take(_settings.ValuesPerPage)
                .ToList();

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(pagedResult);
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllByAssetAsync(string clientId, string assetId, int page)
        {
            var clientRecords = await GetRecordsByClient(clientId);

            var pagedResult = clientRecords
                .Where(r => r.Currency == assetId)
                .Skip((page - 1) * _settings.ValuesPerPage)
                .Take(_settings.ValuesPerPage)
                .ToList();

            return Mapper.Map<IEnumerable<HistoryEntryResponse>>(pagedResult);
        }

        private async Task<IEnumerable<IHistoryLogEntryEntity>> GetRecordsByClient(string clientId)
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
