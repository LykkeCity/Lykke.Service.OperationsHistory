using System;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.HistoryWriter.Abstractions;
using Lykke.Service.OperationsHistory.HistoryWriter.Exceptions;
using Lykke.Service.OperationsHistory.HistoryWriter.Model;
 using Newtonsoft.Json;

namespace Lykke.Service.OperationsHistory.HistoryWriter.Implementation
{
    /// <summary>
    /// Represents writer that pushes messages to history queue
    /// </summary>
    public class HistoryWriter: IHistoryWriter
    {
        private readonly string _connectionString;
        private AzureQueueExt _queue;
        /// <summary>
        /// Represents history queue
        /// </summary>
        protected AzureQueueExt Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = new AzureQueueExt(_connectionString, Constants.InQueueName);
                }

                return _queue;
            }
        }
        /// <summary>
        /// Initializes a new instance of the HistoryWriter with the given Azure connection 
        /// string and ILog implementation
        /// Exceptions:
        ///     - ArgumentNullException
        /// </summary>
        /// <param name="connectionString">Azure account connection string</param>
        public HistoryWriter(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }
        /// <summary>
        /// Pushes message to the history queue.
        /// Exceptions:
        ///     - ArgumentNullException
        ///     - NewHistoryEntryException
        /// </summary>
        /// <param name="newEntry">New history entry</param>
        /// <returns></returns>
        public async Task Push(HistoryEntry newEntry)
        {
            ValidateEntry(newEntry);
            await DoPush(newEntry);
        }
        /// <summary>
        /// Validating new history entry
        /// Exceptions:
        ///     - ArgumentNullException
        /// </summary>
        /// <param name="entryToValidate"></param>
        protected virtual void ValidateEntry(HistoryEntry entryToValidate)
        {
            if (entryToValidate == null)
            {
                throw new ArgumentNullException(nameof(entryToValidate));
            }
            if (String.IsNullOrEmpty(entryToValidate.ClientId))
            {
                throw new ArgumentNullException(entryToValidate.ClientId);
            }
            if (String.IsNullOrEmpty(entryToValidate.OpType))
            {
                throw new ArgumentNullException(entryToValidate.OpType);
            }
            if (String.IsNullOrEmpty(entryToValidate.Currency))
            {
                throw new ArgumentNullException(entryToValidate.Currency);
            }
        }
        /// <summary>
        /// Internal methos for pushing messages to the queue
        /// </summary>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        protected virtual async Task DoPush(HistoryEntry newEntry)
        {
            try
            {
                var json = JsonConvert.SerializeObject(newEntry);
                await Queue.PutRawMessageAsync(json);
            }
            catch (Exception e)
            {
                throw new NewHistoryEntryException("Queue.PutMessageAsync exception", e);
            }
        }
    }
}

// minor change to check TC buildv
