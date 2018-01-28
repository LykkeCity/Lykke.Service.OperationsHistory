using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.OperationsHistory.AutorestClient;
using Lykke.Service.OperationsHistory.AutorestClient.Models;
using Lykke.Service.OperationsHistory.Client.Models;
using Microsoft.Rest;

namespace Lykke.Service.OperationsHistory.Client
{
    public class OperationsHistoryClient : IOperationsHistoryClient, IDisposable
    {
        private readonly ILog _log;
        private OperationsHistoryAPI _apiClient;

        public OperationsHistoryClient(string serviceUrl, ILog log)
        {
            _log = log;
            _apiClient = new OperationsHistoryAPI(new Uri(serviceUrl));
        }

        public void Dispose()
        {
            if (_apiClient == null)
                return;
            _apiClient.Dispose();
            _apiClient = null;
        }

        private OperationsHistoryResponse PrepareResponseMultiple(HttpOperationResponse<object> serviceResponse)
        {
            var error = serviceResponse.Body as ErrorResponse;
            var result = serviceResponse.Body as IList<HistoryOperation>;

            if (error != null)
            {
                return new OperationsHistoryResponse
                {
                    Error = new ErrorModel
                    {
                        Message = error.ErrorMessage
                    }
                };
            }

            if (result != null)
            {
                return new OperationsHistoryResponse
                {
                    Records = result
                };
            }

            throw new ArgumentException("Unknown response object");
        }

        public async Task<OperationsHistoryResponse> GetByClientId(string clientId, HistoryOperationType? operationType = null, 
            string assetId = null, int take = 100, int skip = 0)
        {
            var response =
                await _apiClient.GetByClientIdWithHttpMessagesAsync(clientId, take, skip, operationType, assetId);

            return PrepareResponseMultiple(response);
        }

        public async Task<OperationsHistoryResponse> GetByDateRange(DateTime dateFrom, DateTime? dateTo,
            HistoryOperationType? operationType = null, string assetId = null)
        {
            var actualDateTo = dateTo ?? DateTime.UtcNow;

            var response = await _apiClient.GetByDatesWithHttpMessagesAsync(dateFrom, actualDateTo, operationType, assetId);

            return PrepareResponseMultiple(response);
        }

        public async Task<OperationsHistoryResponse> GetByWalletId(string walletId, HistoryOperationType? operationType = null, string assetId = null, int take = 100, int skip = 0)
        {
            var response =
                await _apiClient.GetByWalletIdWithHttpMessagesAsync(walletId, take, skip, operationType, assetId);

            return PrepareResponseMultiple(response);
        }

        public async Task<HistoryOperation> GetByOperationId(string walletId, string operationId)
        {
            var response = await _apiClient.GetByOperationIdWithHttpMessagesAsync(walletId, operationId);

            return response.Body as HistoryOperation;
        }
    }
}