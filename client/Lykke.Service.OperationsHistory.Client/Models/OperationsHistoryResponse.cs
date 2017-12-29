using System.Collections.Generic;
using Lykke.Service.OperationsHistory.AutorestClient.Models;

namespace Lykke.Service.OperationsHistory.Client.Models
{
    public class OperationsHistoryResponse
    {
        public ErrorModel Error { get; set; }
        public IList<HistoryOperation> Records { get; set; }
    }
}