using System.ComponentModel.DataAnnotations;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    /// <summary>
    /// Represents a market order creation information.
    /// </summary>
    public class MarketOrderRequest
    {
        /// <summary>
        /// The asset pair.
        /// </summary>
        [Required]
        [JsonProperty("instrument")]
        public string Instrument { get; set; }

        /// <summary>
        /// The limit order volume.
        /// </summary>
        [Required]
        [PositiveDecimal]
        [JsonProperty("amount")]
        public decimal Volume { get; set; }

        /// <summary>
        /// The limit order type.
        /// </summary>
        [Required]
        [StrictEnumChecker]
        [JsonProperty("tradeType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeType TradeType { get; set; }
    }
}