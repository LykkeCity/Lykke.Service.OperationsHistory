﻿using System;

namespace Lykke.Service.OperationsHistory.Job.Model
{
    public class HistoryQueueEntry
    {
        public DateTime DateTime { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string OpType { get; set; }
        public string ClientId { get; set; }
        public string CustomData { get; set; }
    }
}
