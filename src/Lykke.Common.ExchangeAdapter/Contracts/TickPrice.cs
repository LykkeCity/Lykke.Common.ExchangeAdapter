using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    /// <summary>
    /// Represents a best ask and bid price of asset pair.
    /// </summary>
    public class TickPrice
    {
        /// <summary>
        /// The name of exchange.
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// The asset pair.
        /// </summary>
        [JsonProperty("asset")]
        public string Asset { get; set; }

        /// <summary>
        /// The time of prices. 
        /// </summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The best sell price.
        /// </summary>
        [JsonProperty("ask")]
        public decimal Ask { get; set; }

        /// <summary>
        /// The best buy price.
        /// </summary>
        [JsonProperty("bid")]
        public decimal Bid { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((TickPrice) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Source != null ? Source.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Asset != null ? Asset.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Ask.GetHashCode();
                hashCode = (hashCode * 397) ^ Bid.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Creates a new <see cref="TickPrice"/> by using order book.
        /// </summary>
        /// <param name="orderBook">The source order book.</param>
        /// <returns>The new instance of <see cref="TickPrice"/>.</returns>
        public static TickPrice FromOrderBook(OrderBook orderBook)
        {
            return new TickPrice
            {
                Source = orderBook.Source,
                Asset = orderBook.Asset,
                Timestamp = orderBook.Timestamp,
                Ask = orderBook.BestAskPrice,
                Bid = orderBook.BestBidPrice
            };
        }

        private bool Equals(TickPrice other)
        {
            return string.Equals(Source, other.Source)
                   && string.Equals(Asset, other.Asset)
                   && Ask == other.Ask && Bid == other.Bid;
        }
    }
}