using System;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public sealed class Order
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("instrument")]
        public string Instrument { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("originalAmount")]
        public decimal OriginalAmount { get; set; }

        [JsonProperty("tradeType")]
        public TradeType TradeType { get; set; }

        [JsonProperty("createdTime")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("avgExecutionPrice")]
        public decimal AvgExecutionPrice { get; set; }

        [JsonProperty("status")]
        public OrderStatus Status { get; set; }

        [JsonProperty("executedAmount")]
        public decimal ExecutedAmount { get; set; }

        [JsonProperty("remainingAmount")]
        public decimal RemainingAmount { get; set; }
    }
}
