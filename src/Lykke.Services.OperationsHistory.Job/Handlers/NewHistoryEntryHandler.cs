using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.JobTriggers.Triggers.Bindings;
using Lykke.Services.OperationsHistory.Core;
using Lykke.Services.OperationsHistory.Core.Entities;
using Lykke.Services.OperationsHistory.Job.Model;

namespace Lykke.Services.OperationsHistory.Job.Handlers
{
    public class NewHistoryEntryHandler
    {
        private readonly ILog _log;
        private readonly IHistoryLogEntryRepository _repo;

        public NewHistoryEntryHandler(ILog log, IHistoryLogEntryRepository repo)
        {
            _log = log;
            _repo = repo;
        }

        [QueueTrigger(Constants.InQueueName, 100, true)]
        public async Task ProcessNewEntry(HistoryQueueEntry message, QueueTriggeringContext context)
        {
            if (!Validate(message))
            {
                ToPoison(context);
                return;
            }

            await ToRepository(message);
        }

        public virtual void ToPoison(QueueTriggeringContext context)
        {
            context.MoveMessageToPoison();
        }

        public virtual Task ToRepository(HistoryQueueEntry message)
        {
            return _repo.AddAsync(
                message.DateTime,
                message.Amount,
                message.Currency,
                message.ClientId,
                message.CustomData,
                message.OpType);
        }

        public static bool Validate(HistoryQueueEntry message)
        {
            if (string.IsNullOrEmpty(message.Currency))
            {
                return false;
            }
            if (string.IsNullOrEmpty(message.ClientId))
            {
                return false;
            }
            if (string.IsNullOrEmpty(message.OpType))
            {
                return false;
            }

            return true;
        }
    }
}
