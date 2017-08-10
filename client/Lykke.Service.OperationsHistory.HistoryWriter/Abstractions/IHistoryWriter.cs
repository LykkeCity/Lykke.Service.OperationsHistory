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
        ///     ArgumentNullException  
        /// </summary>
        /// <param name="newEntry"></param>
        Task Push(HistoryEntry newEntry);
        /// <summary>
        /// Updates blockchain hash in history log entry with given id
        /// Exceptions:
        ///     UpdateHistoryEntryException in case any error during hash update
        ///     ArgumentNullException
        /// </summary>
        /// <param name="id">Id of the record</param>
        /// <param name="hash">Hash value></param>
        /// <returns></returns>
        Task UpdateBlockChainHash(string id, string hash);
        /// <summary>
        ///  Updates state in history log entry with given id
        ///  Exceptions:
        ///     UpdateHistoryEntryException in case any error during state update
        ///     ArgumentNullException
        /// </summary>
        /// <param name="id">Id of the record</param>
        /// <param name="state">State value</param>
        /// <returns></returns>
        Task UpdateState(string id, int state);
    }
}