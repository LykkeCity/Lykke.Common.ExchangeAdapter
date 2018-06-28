using System.ComponentModel.DataAnnotations;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class MarketOrderRequest
    {
        private string _instrument;

        /// <summary>
        /// name of instrument (asset pair)
        /// </summary>
        [JsonProperty("instrument")]
        [Required]
        public string Instrument
        {
            get => _instrument;
            set => _instrument = value?.ToUpperInvariant();
        }

        /// <summary>
        /// volume of order
        /// </summary>
        [JsonProperty("amount")]
        [Required]
        [PositiveDecimal]
        public decimal Volume { get; set; }

        /// <summary>
        /// side of trade: Buy, Sell
        /// </summary>
        [JsonProperty("tradeType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [StrictEnumChecker]
        [Required]
        public TradeType TradeType { get; set; } //needs validation


    }
}