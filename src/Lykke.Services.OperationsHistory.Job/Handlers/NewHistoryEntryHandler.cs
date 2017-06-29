using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.JobTriggers.Triggers.Bindings;
using Lykke.Services.OperationsHistory.Core;

namespace Lykke.Services.OperationsHistory.Job.Handlers
{
    public class NewHistoryEntryHandler
    {
        private readonly ILog _log;
        public NewHistoryEntryHandler(ILog log)
        {
            _log = log;
        }

        [QueueTrigger(Constants.InQueueName, 100, true)]
        public async Task ProcessNewEntry(string message, QueueTriggeringContext context)
        {
            await _log.WriteInfoAsync(GetType().Name, "NULL", "NULL", message);
        }
    }
}
