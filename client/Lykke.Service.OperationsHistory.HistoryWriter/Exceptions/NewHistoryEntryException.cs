using System;

namespace Lykke.Service.OperationsHistory.HistoryWriter.Exceptions
{
    /// <summary>
    /// Represents an error occured during history queue push
    /// </summary>
    public class NewHistoryEntryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NewHistoryEntryException with the specified message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public NewHistoryEntryException(string message) : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of the NewHistoryEntryException with the specified message
        /// and the reference to the inner exception that is the cause of the exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">Reference to inner exception</param>
        public NewHistoryEntryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
