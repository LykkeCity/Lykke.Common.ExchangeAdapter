using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public class TickPrice
    {
        private string _asset;

        private bool Equals(TickPrice other)
        {
            return string.Equals(Source, other.Source)
                   && string.Equals(Asset, other.Asset)
                   && Ask == other.Ask && Bid == other.Bid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TickPrice) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Asset != null ? Asset.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Ask.GetHashCode();
                hashCode = (hashCode * 397) ^ Bid.GetHashCode();
                return hashCode;
            }
        }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("asset")]
        public string Asset
        {
            get => _asset;
            set => _asset = value?.ToUpperInvariant();
        }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("ask")]
        public decimal Ask { get; set; }

        [JsonProperty("bid")]
        public decimal Bid { get; set; }

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
    }
}
