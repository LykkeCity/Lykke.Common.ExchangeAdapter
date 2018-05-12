using System;
using Lykke.Common.ExchangeAdapter.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class OrderModel
    {
        /// <summary>
        /// OrderId
        /// </summary>
        [JsonProperty("orderId")]
        public string Id { get; set; }

        /// <summary>
        /// The instrument (pair) name the order belongs to
        /// </summary>
        [JsonProperty("instrument")]
        public string Symbol { get; set; }

        /// <summary>
        /// The price the order was issued at
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// What was the order originally submitted for?
        /// </summary>
        [JsonProperty("originalAmount")]
        public decimal OriginalVolume { get; set; }

        /// <summary>
        /// type of trade: “Buy”, “Sell”
        /// </summary>
        [JsonProperty("tradeType"), JsonConverter(typeof(StringEnumConverter))]
        public TradeType TradeType { get; set; }

        /// <summary>
        /// The date & time the order was submitted
        /// </summary>
        [JsonProperty("createdTime")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The average price at which this order as been executed so far. 0 if the order has not been executed at all
        /// </summary>
        [JsonProperty("avgExecutionPrice")]
        public decimal AvgExecutionPrice { get; set; }

        [JsonProperty("status")]
        public OrderStatus ExecutionStatus { get; set; }

        /// <summary>
        /// How much of the order has been executed so far in its history
        /// </summary>
        [JsonProperty("executedAmount")]
        public decimal ExecutedVolume { get; set; }

        /// <summary>
        /// How much is still remaining to be submitted
        /// </summary>
        [JsonProperty("remainingAmount")]
        public decimal RemainingAmount { get; set; }
    }
}