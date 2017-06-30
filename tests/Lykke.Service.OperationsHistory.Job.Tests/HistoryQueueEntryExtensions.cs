using Lykke.Service.OperationsHistory.Job.Model;

namespace Lykke.Service.OperationsHistory.Job.Tests
{
    public static class HistoryQueueEntryExtensions
    {
        public static HistoryQueueEntry EmptyClientId(this HistoryQueueEntry o)
        {
            o.ClientId = string.Empty;
            return o;
        }

        public static HistoryQueueEntry EmptyCurrency(this HistoryQueueEntry o)
        {
            o.Currency = string.Empty;
            return o;
        }

        public static HistoryQueueEntry EmptyOperationType(this HistoryQueueEntry o)
        {
            o.OpType = string.Empty;
            return o;
        }
    }
}
