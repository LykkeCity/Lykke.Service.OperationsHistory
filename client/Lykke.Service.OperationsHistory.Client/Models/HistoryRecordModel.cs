using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.OperationsHistory.Client.Models
{
    public class HistoryRecordModel
    {
        public DateTime DateTime { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string OpType { get; set; }
        public string ClientId { get; set; }
        public string CustomData { get; set; }
    }
}
