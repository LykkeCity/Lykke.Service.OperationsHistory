using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Service.OperationsHistory.Core.Entities;

namespace Lykke.Service.OperationsHistory.Services.InMemoryCache
{
    public class CacheModel
    {
        public DateTime LastUpdated;
        public IQueryable<IHistoryLogEntryEntity> Records;
    }
}
