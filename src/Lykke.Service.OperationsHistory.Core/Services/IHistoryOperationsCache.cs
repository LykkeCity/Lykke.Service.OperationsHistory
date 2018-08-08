using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Domain;

namespace Lykke.Service.OperationsHistory.Core.Services
{
    public interface IOperationsCache<TItem>
    {
        Task<IEnumerable<TItem>> GetAsync(string walletId, HistoryOperationType[] operationTypes = null, string assetId = null, string assetPairId = null, PaginationInfo paging = null);

        Task AddOrUpdate(string walletId, TItem item);

        Task RemoveIfLoaded(string walletId, string operationId);

        Task WarmUp(string walletId);
    }

    public interface IHistoryOperationsCache : IOperationsCache<HistoryOperation>
    {
        
    }
}