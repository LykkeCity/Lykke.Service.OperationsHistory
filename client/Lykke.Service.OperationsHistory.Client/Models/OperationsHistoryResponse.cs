using System.Collections.Generic;

namespace Lykke.Service.OperationsHistory.Client.Models
{
    public class OperationsHistoryResponse
    {
        public ErrorModel Error { get; set; }
        public IList<HistoryRecordModel> Records { get; set; }
    }
}