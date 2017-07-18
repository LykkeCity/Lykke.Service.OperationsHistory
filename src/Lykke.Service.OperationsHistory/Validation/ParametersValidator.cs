using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.OperationsHistory.Validation
{
    public static class ParametersValidator
    {
        public static bool ValidatePageIndex(int pageIndex)
        {
            return pageIndex > 0;
        }

        public static bool ValidateClientId(string clientId)
        {
            return !string.IsNullOrWhiteSpace(clientId);
        }

        public static bool ValidateOperationType(string operationType)
        {
            return !string.IsNullOrWhiteSpace(operationType);
        }

        public static bool ValidateAssetId(string assetId)
        {
            return !string.IsNullOrWhiteSpace(assetId);
        }

        public static bool ValidateTop(int top)
        {
            return (top <= 1000) && (top >= 1);
        }

        public static bool ValidateSkip(int skip)
        {
            return skip >= 0;
        }

        public static bool ValidateId(string id)
        {
            return !string.IsNullOrWhiteSpace(id);
        }
    }
}
