﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public OperationsHistoryClient(string serviceUrl, ILog log)
        {
            _log = log;
            _mapper = CreateMapper();
            _apiClient = new OperationsHistoryAPI(new Uri(serviceUrl));
        }

        public void Dispose()
        {
            if (_apiClient == null)
                return;
            _apiClient.Dispose();
            _apiClient = null;
        }

        public static IMapper CreateMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HistoryEntryResponse, HistoryRecordModel>()
                    .ForMember(dest => dest.ClientId, opt => opt.Ignore());
            });

            return mapperConfiguration.CreateMapper();
        }

        private OperationsHistoryResponse PrepareClientResponse(HttpOperationResponse<object> serviceResponse)
        {
            var error = serviceResponse.Body as ErrorResponse;
            var result = serviceResponse.Body as IList<HistoryEntryResponse>;

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
                    Records = _mapper.Map<IList<HistoryRecordModel>>(result)
                };
            }

            throw new ArgumentException("Unknown response object");
        }

        public async Task<OperationsHistoryResponse> GetByClientId(string clientId, string operationType = null, 
            string assetId = null, int take = 100, int skip = 0)
        {
            var response =
                await _apiClient.GetByClientIdWithHttpMessagesAsync(clientId, take, skip, operationType, assetId);

            return PrepareClientResponse(response);
        }

        public async Task<OperationsHistoryResponse> GetByDateRange(DateTime dateFrom, DateTime? dateTo,
            string operationType = null)
        {
            var actualDateTo = dateTo ?? DateTime.UtcNow;

            var response = await _apiClient.GetByDatesWithHttpMessagesAsync(dateFrom, actualDateTo, operationType);

            return PrepareClientResponse(response);
        }
    }
}