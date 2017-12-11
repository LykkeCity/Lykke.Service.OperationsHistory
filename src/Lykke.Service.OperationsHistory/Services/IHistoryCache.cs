using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.OperationsHistory.Core.Domain;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Services
{
    public interface IHistoryCache
    {
        Task<IEnumerable<IHistoryLogEntryEntity>> GetAsync(string walletId, string operationType, string assetId, PaginationInfo paging = null);

        void AddOrUpdate(IHistoryLogEntryEntity item);

        Task WarmUp(string clientId);
    }
}