using System;

namespace Lykke.Service.OperationsHistory.HistoryWriter.Model
{
    /// <summary>
    /// DTO to write to History Service Queue
    /// </summary>
    public class HistoryEntry
    {
        /// <summary>
        /// Timestamp of the operation
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Total amount of the operation
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Currency of the operation
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Operation type
        /// </summary>
        public string OpType { get; set; }
        /// <summary>
        /// Client identifier
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Custom json object serialized to string
        /// </summary>
        public string CustomData { get; set; }
    }
}