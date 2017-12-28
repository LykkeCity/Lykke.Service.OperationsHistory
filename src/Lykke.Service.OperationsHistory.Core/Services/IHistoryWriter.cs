using System.Threading.Tasks;
using Lykke.Service.OperationsRepository.Contract.History;

namespace Lykke.Service.OperationsHistory.Core.Services
{
    public interface IHistoryWriter
    {
        Task SaveAsync(OperationsHistoryMessage historyRecord);
    }
}