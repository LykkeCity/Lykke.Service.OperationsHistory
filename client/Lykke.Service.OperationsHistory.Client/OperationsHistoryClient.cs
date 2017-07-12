using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.OperationsHistory.AutorestClient;
using Lykke.Service.OperationsHistory.AutorestClient.Models;

namespace Lykke.Service.OperationsHistory.Client
{
    public class OperationsHistoryClient : IOperationsHistoryClient, IDisposable
    {
        private readonly ILog _log;
        private readonly IOperationsHistoryAPI _apiClient;

        public OperationsHistoryClient(string serviceUrl, ILog log)
        {
            _log = log;
            _apiClient = new OperationsHistoryAPI(new Uri(serviceUrl));
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }

        public Task<IList<HistoryEntryResponse>> AllAsync(string clientId, int top, int skip)
        {
            throw new NotImplementedException();
        }

        public Task<IList<HistoryEntryResponse>> AllAsync(string clientId, int page)
        {
            throw new NotImplementedException();
        }

        public Task<IList<HistoryEntryResponse>> ByOperationAsync(string clientId, string operationType, int top, int skip)
        {
            throw new NotImplementedException();
        }

        public Task<IList<HistoryEntryResponse>> ByOperationAsync(string clientId, string operationType, int page)
        {
            throw new NotImplementedException();
        }

        public Task<IList<HistoryEntryResponse>> ByOperationAndAssetAsync(string clientId, string operationType, string assetId, int top, int skip)
        {
            throw new NotImplementedException();
        }

        public Task<IList<HistoryEntryResponse>> ByOperationAndAssetAsync(string clientId, string operationType, string assetId, int page)
        {
            throw new NotImplementedException();
        }

        public Task<IList<HistoryEntryResponse>> ByAssetAsync(string clientId, string assetId, int top, int skip)
        {
            throw new NotImplementedException();
        }

        public Task<IList<HistoryEntryResponse>> ByAssetAsync(string clientId, string assetId, int page)
        {
            throw new NotImplementedException();
        }
    }
}
