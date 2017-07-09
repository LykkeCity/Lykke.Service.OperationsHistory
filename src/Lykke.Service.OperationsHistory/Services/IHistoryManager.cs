﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Services
{
    public interface IHistoryManager
    {
        Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllAsync(string clientId, string assetId, string operatonType, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllByOpTypeAsync(string clientId, string operationType, int page);
        Task<IEnumerable<HistoryEntryResponse>> GetAllByAssetAsync(string clientId, string assetId, int page);
    }
}