using Lykke.Service.OperationsHistory.HistoryWriter.Model;

namespace Lykke.Service.OperationsHistory.HistoryWriter.Abstractions
{
    public interface IHistoryMapper<in T>
    {
        HistoryEntry MapFrom(T source);
    }
}