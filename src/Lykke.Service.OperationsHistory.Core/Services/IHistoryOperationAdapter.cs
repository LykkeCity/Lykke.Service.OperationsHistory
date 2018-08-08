using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsRepository.Contract.History;
using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.Core.Services
{
    public interface IOperationAdapter<TSource, TDest>
    {
        Task<TDest> ExecuteAsync(TSource src);
    }

    public interface IHistoryOperationAdapter : IOperationAdapter<HistoryLogEntryEntity, HistoryOperation>
    {
        
    }

    public interface IHistoryMessageAdapter : IOperationAdapter<OperationsHistoryMessage, HistoryOperation>
    {
        
    }
}