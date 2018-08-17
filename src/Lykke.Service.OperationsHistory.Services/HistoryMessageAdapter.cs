using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsRepository.Contract.History;

namespace Lykke.Service.OperationsHistory.Services
{
    public class HistoryMessageAdapter : IHistoryMessageAdapter
    {
        private readonly IHistoryOperationAdapter _operationAdapter;

        public HistoryMessageAdapter(IHistoryOperationAdapter operationAdapter)
        {
            _operationAdapter = operationAdapter;
        }

        public Task<HistoryOperation> ExecuteAsync(OperationsHistoryMessage src)
        {
            var historyLogEntry = new HistoryLogEntryEntity
            {
                Id = src.Id,
                ClientId = src.ClientId,
                CustomData = src.Data,
                DateTime = src.DateTime,
                OpType = src.OpType,
                Amount = src.Amount,
                Currency = src.Currency
            };

            return _operationAdapter.ExecuteAsync(historyLogEntry);
        }
    }
}