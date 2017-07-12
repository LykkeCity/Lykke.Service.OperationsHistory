// Code generated by Microsoft (R) AutoRest Code Generator 1.1.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.Service.OperationsHistory.AutorestClient.Models
{
    using Lykke.Service;
    using Lykke.Service.OperationsHistory;
    using Lykke.Service.OperationsHistory.AutorestClient;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class HistoryEntryResponse
    {
        /// <summary>
        /// Initializes a new instance of the HistoryEntryResponse class.
        /// </summary>
        public HistoryEntryResponse()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the HistoryEntryResponse class.
        /// </summary>
        public HistoryEntryResponse(System.DateTime? dateTime = default(System.DateTime?), double? amount = default(double?), string currency = default(string), string opType = default(string), string customData = default(string))
        {
            DateTime = dateTime;
            Amount = amount;
            Currency = currency;
            OpType = opType;
            CustomData = customData;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DateTime")]
        public System.DateTime? DateTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Amount")]
        public double? Amount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "OpType")]
        public string OpType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CustomData")]
        public string CustomData { get; set; }

    }
}