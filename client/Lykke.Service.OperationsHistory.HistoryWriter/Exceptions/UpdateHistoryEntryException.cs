using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.OperationsHistory.HistoryWriter.Exceptions
{
    /// <summary>
    /// Represents an error occured during history log entry update
    /// </summary>
    public class UpdateHistoryEntryException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the UpdateHistoryEntryException with the specified message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public UpdateHistoryEntryException(string message) : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of the UpdateHistoryEntryException with the specified message
        /// and the reference to the inner exception that is the cause of the exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">Reference to inner exception</param>
        public UpdateHistoryEntryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
