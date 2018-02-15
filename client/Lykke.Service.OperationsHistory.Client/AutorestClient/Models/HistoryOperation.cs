// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.OperationsHistory.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class HistoryOperation
    {
        /// <summary>
        /// Initializes a new instance of the HistoryOperation class.
        /// </summary>
        public HistoryOperation()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the HistoryOperation class.
        /// </summary>
        /// <param name="type">Possible values include: 'CashIn', 'CashOut',
        /// 'Trade', 'LimitTrade', 'LimitOrderEvent'</param>
        /// <param name="state">Possible values include: 'InProgress',
        /// 'Finished', 'Canceled', 'Failed'</param>
        public HistoryOperation(System.DateTime dateTime, HistoryOperationType type, HistoryOperationState state, double amount, string id = default(string), string asset = default(string), string assetPair = default(string), double? price = default(double?))
        {
            Id = id;
            DateTime = dateTime;
            Type = type;
            State = state;
            Amount = amount;
            Asset = asset;
            AssetPair = assetPair;
            Price = price;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DateTime")]
        public System.DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'CashIn', 'CashOut', 'Trade',
        /// 'LimitTrade', 'LimitOrderEvent'
        /// </summary>
        [JsonProperty(PropertyName = "Type")]
        public HistoryOperationType Type { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'InProgress', 'Finished',
        /// 'Canceled', 'Failed'
        /// </summary>
        [JsonProperty(PropertyName = "State")]
        public HistoryOperationState State { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Amount")]
        public double Amount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Asset")]
        public string Asset { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "AssetPair")]
        public string AssetPair { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Price")]
        public double? Price { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
        }
    }
}
