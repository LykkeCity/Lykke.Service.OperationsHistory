using System;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.AutorestClient.Models;
using Lykke.Service.OperationsHistory.Client.Models;

namespace Lykke.Service.OperationsHistory.Client
{
    public interface IOperationsHistoryClient
    {
        /// <summary>
        /// Getting history for particular client identifier
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="operationType">The type of the operation</param>
        /// <param name="assetId">Asset identifier</param>
        /// <param name="take">How many maximum items have to be returned</param>
        /// <param name="skip">How many items skip before returning</param>
        /// <returns></returns>
        Task<OperationsHistoryResponse> GetByClientId(string clientId, HistoryOperationType? operationType, string assetId, string assetPairId, int take, int skip);

        /// <summary>
        /// Getting history by date range, note: internal cache is not used in this case
        /// </summary>
        /// <param name="dateFrom">The date of the operation will be equal or greater than</param>
        /// <param name="dateTo">The date of the operation will be less than</param>
        /// <param name="operationType">The type of the operation</param>
        /// <param name="assetId">Asset identifier</param>
        /// <returns></returns>
        Task<OperationsHistoryResponse> GetByDateRange(DateTime dateFrom, DateTime? dateTo, HistoryOperationType? operationType, string assetId, string assetPairId);

        /// <summary>
        /// Getting history by wallet identifier
        /// </summary>
        /// <param name="walletId">Wallet identifier</param>
        /// <param name="operationType">The type of the operation</param>
        /// <param name="assetId">Asset identifier</param>
        /// <param name="take">How many maximum items have to be returned</param>
        /// <param name="skip">How many items skip before returning</param>
        /// <returns></returns>
        Task<OperationsHistoryResponse> GetByWalletId(string walletId, HistoryOperationType? operationType, string assetId, string assetPairId, int take, int skip);

        /// <summary>
        /// Getting history record by operation identifier
        /// </summary>
        /// <param name="walletId">Wallet identifier</param>
        /// <param name="operationId">operation identifier</param>
        /// <returns></returns>
        Task<HistoryOperation> GetByOperationId(string walletId, string operationId);

        /// <summary>
        /// Deletes history record by clientId and operation Id
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <param name="operationId">Operation Id</param>
        /// <returns></returns>
        Task<HistoryOperation> DeleteByClientIdOperationId(string clientId, string operationId);
    }
}