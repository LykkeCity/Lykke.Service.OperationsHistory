using System.Collections.Concurrent;
using Lykke.Service.OperationsHistory.Core.Entities;

namespace Lykke.Service.OperationsHistory.Services.InMemoryCache
{
    public class CacheModel
    {
        public ConcurrentDictionary<string, IHistoryLogEntryEntity> Records;
    }
}
