using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.OperationsHistory.Core;

namespace Lykke.Service.OperationsHistory.Services
{
    public static class HistoryOperationTypeConverter
    {
        public static HistoryOperationType?[] GetHistoryOperationTypes(IEnumerable<string> types)
        {
            return types?
                .Select(s =>
                    Enum.TryParse(s, true, out HistoryOperationType ot)
                        ? ot
                        : (HistoryOperationType?)null).ToArray();
        }
    }
}
