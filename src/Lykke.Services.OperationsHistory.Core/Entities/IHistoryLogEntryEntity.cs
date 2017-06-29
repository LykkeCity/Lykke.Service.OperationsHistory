using System;

namespace Lykke.Services.OperationsHistory.Core.Entities
{
    public interface IHistoryLogEntryEntity
    {
        DateTime DateTime { get; set; }
        decimal Amount { get; set; }
        string Currency { get; set; }
        string ClientId { get; set; }
        string CustomData { get; set; }
    }
}