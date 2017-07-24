using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.HistoryWriter.Abstractions;
using Lykke.Service.OperationsHistory.HistoryWriter.Model;

namespace Lykke.Service.OperationsHistory.HistoryWriter.Implementation
{
    public abstract class RepositoryWriter<TRepo, TRepoItem> : IHistoryMapper<TRepoItem>
    {
        protected readonly TRepo Repo;
        protected readonly IHistoryWriter HistoryWriter;

        protected RepositoryWriter(TRepo repo, IHistoryWriter historyWriter)
        {
            Repo = repo;
            HistoryWriter = historyWriter;
        }

        public HistoryEntry MapFrom(TRepoItem source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return DoMapFrom(source);
        }

        protected virtual HistoryEntry DoMapFrom(TRepoItem source)
        {
            throw new NotImplementedException();
        }
    }
}
