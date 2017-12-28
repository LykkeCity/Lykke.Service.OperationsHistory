using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Domain;

namespace Lykke.Service.OperationsHistory.Core
{
    public interface IOperationsCache<TItem>
    {
        Task<IEnumerable<TItem>> GetAsync(string walletId, string operationType = null, string assetId = null, PaginationInfo paging = null);

        Task AddOrUpdate(string walletId, TItem item);

        Task WarmUp(string walletId);
    }

    public interface IHistoryOperationsCache : IOperationsCache<HistoryOperation>
    {
        
    }
}