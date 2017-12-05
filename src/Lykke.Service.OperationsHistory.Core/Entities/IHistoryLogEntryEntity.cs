using System;

namespace Lykke.Service.OperationsHistory.Core.Entities
{
    public interface IHistoryLogEntryEntity
    {
        string Id { get; set; }
        DateTime DateTime { get; set; }
        double Amount { get; set; }
        string AssetId { get; set; }
        string ClientId { get; set; }
        string CustomData { get; set; }
        string OpType { get; set; }
    }
}