using System;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Client.Models;

namespace Lykke.Service.OperationsHistory.Client
{
    public interface IOperationsHistoryClient
    {
        Task<OperationsHistoryResponse> GetByClientId(string clientId, string operationType, string assetId, int take, int skip);
        Task<OperationsHistoryResponse> GetByDateRange(DateTime dateFrom, DateTime? dateTo, string operationType);
        Task<OperationsHistoryResponse> GetByWalletId(string walletId, string operationType, string assetId, int take, int skip);
    }
}