using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Services
{
    public class InMemoryCache: IHistoryCache
    {
        private readonly ILog _logger;
        public InMemoryCache(ILog logger)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, int page)
        {
            return new List<HistoryEntryResponse>()
            {
                new HistoryEntryResponse
                {
                    DateTime = DateTime.Now,
                    Currency = "Test",
                    OpType = "Test",
                    Amount = 0,
                    CustomData = String.Empty
                }
            };
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, string assetId, string operatonType, int page)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllByOpTypeAsync(string clientId, string operationType, int page)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HistoryEntryResponse>> GetAllByAssetAsync(string clientId, string assetId, int page)
        {
            throw new NotImplementedException();
        }
    }
}
