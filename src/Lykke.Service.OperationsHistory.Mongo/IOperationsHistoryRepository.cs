using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsRepository.Contract;

namespace Lykke.Service.OperationsHistory.Mongo
{
    public interface IOperationsHistoryRepository
    {
        Task AddOrUpdateAsync(string clientId, string walletId, HistoryOperation operation, string customData);

        Task<OperationsHistoryEntity> GetByIdAsync(string clientId, string id);
        
        Task<IEnumerable<OperationsHistoryEntity>> GetByClientIdAsync(
            string clientId,
            string walletId,
            HistoryOperationType? operationType,
            string assetId,
            string assetPairId,
            int take, 
            int skip);

        Task DeleteIfExistsAsync(string clientId, string id);
    }
}