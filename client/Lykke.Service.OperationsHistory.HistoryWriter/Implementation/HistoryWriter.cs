﻿using System;
using System.Threading.Tasks;
using AzureStorage.Queue;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
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
        private HistoryLogEntryRepository _repo;
        private readonly ILog _log;
        /// <summary>
        /// Represents history queue
        /// </summary>
        protected AzureQueueExt Queue => _queue ?? (_queue = new AzureQueueExt(_connectionString, Constants.InQueueName));

        /// <summary>
        /// History log repository
        /// </summary>
        protected HistoryLogEntryRepository Repository =>
            _repo ?? (_repo = new HistoryLogEntryRepository(
                new AzureTableStorage<HistoryLogEntryEntity>(_connectionString, Constants.OutTableName, _log)));

        /// <summary>
        /// Initializes a new instance of the HistoryWriter with the given Azure connection 
        /// string and ILog implementation
        /// Exceptions:
        ///     - ArgumentNullException
        /// </summary>
        /// <param name="connectionString">Azure account connection string</param>
        /// <param name="log"></param>
        public HistoryWriter(string connectionString, ILog log)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _log = log;
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
        /// Updates log entry hash value
        /// </summary>
        /// <param name="id">Id of the request</param>
        /// <param name="hash">Hash value</param>
        /// <returns></returns>
        public async Task UpdateBlockChainHash(string id, string hash)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                await Repository.UpdateBlockchainHashAsync(id, hash);
            }
            catch (Exception e)
            {
                throw new UpdateHistoryEntryException("Repository UpdateBlockchainHashAsync exception", e);
            }
        }
        /// <summary>
        /// Updates log entry state value
        /// </summary>
        /// <param name="id">Id of the request</param>
        /// <param name="state">State value</param>
        /// <returns></returns>
        public async Task UpdateState(string id, int state)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                await Repository.UpdateStateAsync(id, state);
            }
            catch (Exception e)
            {
                throw new UpdateHistoryEntryException("Repository UpdateStateAsync exception", e);
            }
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
                throw new NewHistoryEntryException("Queue.PutRawMessageAsync exception", e);
            }
        }
    }
}
