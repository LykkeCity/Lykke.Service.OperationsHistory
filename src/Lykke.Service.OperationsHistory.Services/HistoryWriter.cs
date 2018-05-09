using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsRepository.Contract.History;

namespace Lykke.Service.OperationsHistory.Services
{
    public class HistoryWriter : IHistoryWriter
    {
        private readonly IHistoryLogEntryRepository _historyRepository;

        public HistoryWriter(IHistoryLogEntryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task SaveAsync(OperationsHistoryMessage historyRecord)
        {
            var existing = await _historyRepository.GetAsync(historyRecord.ClientId, historyRecord.Id);

            if (existing == null)
            {
                await _historyRepository.AddAsync(
                    historyRecord.DateTime, 
                    historyRecord.Amount, 
                    historyRecord.Currency,
                    historyRecord.ClientId, 
                    historyRecord.Data, 
                    historyRecord.OpType, 
                    historyRecord.Id);
            }
            else
            {
                await _historyRepository.UpdateAsync(
                    historyRecord.ClientId, 
                    historyRecord.Id, 
                    historyRecord.Data);
            }
        }
    }
}