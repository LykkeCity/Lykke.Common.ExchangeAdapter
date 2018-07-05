using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public sealed class OrderBook
    {
        private class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                if (y == null) return -1;
                return y.CompareTo(x);
            }
        }

        private static bool CompareDictionaries<TK, TV>(
            IDictionary<TK, TV> first,
            IDictionary<TK, TV> second)
        {
            return first.Count == second.Count
                   && second.All(
                       entry => first.TryGetValue(entry.Key, out var f)
                                && entry.Value.Equals(f));
        }

        private bool Equals(OrderBook other)
        {
            var asksEqual = CompareDictionaries(Asks, other.Asks);

            var bidsEqual = CompareDictionaries(Bids, other.Bids);

            return asksEqual && bidsEqual && string.Equals(Source, other.Source)
                   && string.Equals(Asset, other.Asset);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OrderBook && Equals((OrderBook) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Asks != null ? Asks.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Bids != null ?  Bids.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Asset != null ? Asset.GetHashCode() : 0);
                return hashCode;
            }
        }

        private string _asset;
        private ImmutableSortedDictionary<decimal, decimal> _asks
            = ImmutableSortedDictionary<decimal, decimal>.Empty;
        private ImmutableSortedDictionary<decimal, decimal> _bids
            = ImmutableSortedDictionary.Create<decimal, decimal>(DescComparer);

        public OrderBook()
        {
        }

        public decimal BestAskPrice => Asks.Keys.FirstOrDefault();
        public decimal BestBidPrice => Bids.Keys.FirstOrDefault();

        private  static readonly DescendingComparer<decimal> DescComparer = new DescendingComparer<decimal>();

        private OrderBook(string source,
            string asset,
            DateTime timestamp,
            ImmutableSortedDictionary<decimal, decimal> asks,
            ImmutableSortedDictionary<decimal, decimal> bids)
        {
            Source = source;
            Timestamp = timestamp;
            Asset = asset;
            Asks = asks;
            Bids = bids;
        }

        private OrderBook(string source,
            string asset,
            DateTime timestamp,
            IEnumerable<KeyValuePair<decimal, decimal>> asks,
            IEnumerable<KeyValuePair<decimal, decimal>> bids)
        {
            Source = source;
            Timestamp = timestamp;
            Asset = asset;
            Asks = ImmutableSortedDictionary.CreateRange(asks);
            Bids = ImmutableSortedDictionary.CreateRange(DescComparer, bids);
        }

        public OrderBook(
            string source,
            string asset,
            DateTime timestamp,
            IEnumerable<OrderBookItem> asks,
            IEnumerable<OrderBookItem> bids)
        {
            Source = source;
            Asset = asset;
            Timestamp = timestamp;

            Asks = ImmutableSortedDictionary.CreateRange(
                asks.Where(x => x.Price != 0M)
                    .GroupBy(x => x.Price)
                    .ToDictionary(x => x.Key, x => x.Sum(i => i.Volume)));

            Bids = ImmutableSortedDictionary.CreateRange(
                DescComparer,
                bids.Where(x => x.Price != 0M)
                    .GroupBy(x => x.Price)
                    .ToDictionary(x => x.Key, x => x.Sum(i => i.Volume)));
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

        [JsonProperty("asks"), JsonConverter(typeof(OrderBookItemsConverter))]
        public ImmutableSortedDictionary<decimal, decimal> Asks
        {
            get => _asks;
            set => _asks = value;
        }


        [JsonProperty("bids"), JsonConverter(typeof(OrderBookItemsConverter))]
        public ImmutableSortedDictionary<decimal, decimal> Bids
        {
            get => _bids;
            set => _bids = value;
        }

        public OrderBook Clone(DateTime timestamp)
        {
            return new OrderBook(
                Source,
                Asset,
                timestamp,
                Asks,
                Bids);
        }

        public OrderBook Truncate(int depth)
        {
            return new OrderBook(
                Source,
                Asset,
                Timestamp,
                Asks.Take(depth),
                Bids.Take(depth));
        }

        public void UpdateAsk(decimal price, decimal volume)
        {
            UpdateOrderBook(ref _asks, price, volume);
        }

        private static void UpdateOrderBook(
            ref ImmutableSortedDictionary<decimal, decimal> c, decimal price, decimal volume)
        {
            c = volume == 0 ? c.Remove(price) : c.SetItem(price, volume);
        }

        public void UpdateBid(decimal price, decimal volume)
        {
            UpdateOrderBook(ref _bids, price, volume);
        }
    }

    public sealed class OrderBookItemsConverter : JsonConverter<ImmutableSortedDictionary<decimal, decimal>>
    {
        public override void WriteJson(
            JsonWriter writer,
            ImmutableSortedDictionary<decimal, decimal> value,
            JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var item in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("price");
                writer.WriteRawValue(JsonConvert.ToString(item.Key));

                writer.WritePropertyName("volume");
                writer.WriteRawValue(JsonConvert.ToString(item.Value));

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public override ImmutableSortedDictionary<decimal, decimal> ReadJson(
            JsonReader reader,
            Type objectType,
            ImmutableSortedDictionary<decimal, decimal> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (!hasExistingValue)
            {
                throw new JsonSerializationException("Expected non empty ImmutableSortedDictionary instance");
            }

            return ImmutableSortedDictionary.CreateRange(existingValue.KeyComparer, ReadJsonObject(reader));
        }

        private static IEnumerable<KeyValuePair<decimal, decimal>> ReadJsonObject(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                if (reader.Read())
                {
                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        var obj = JObject.Load(reader);
                        yield return new KeyValuePair<decimal, decimal>(
                            obj["price"].Value<decimal>(),
                            obj["volume"].Value<decimal>());
                        if (!reader.Read()) break;
                    }
                }
            }
        }
    }
}
