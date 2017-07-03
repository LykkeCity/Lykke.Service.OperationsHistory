using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.HistoryWriter.Model;

namespace Lykke.Service.OperationsHistory.HistoryWriter.Abstractions
{
    /// <summary>
    /// History writer interface to be used for pushing new entries into history queue
    /// </summary>
    public interface IHistoryWriter
    {
        /// <summary>
        /// Add new entry to the history queue.
        /// Exceptions: 
        ///     NewHistoryEntryException in case any error during push  
        /// </summary>
        /// <param name="newEntry"></param>
        Task Push(HistoryEntry newEntry);
    }
}