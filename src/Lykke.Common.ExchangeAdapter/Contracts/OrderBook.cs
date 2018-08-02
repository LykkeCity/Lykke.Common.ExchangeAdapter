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
            var asksEqual = CompareDictionaries(AskLevels, other.AskLevels);

            var bidsEqual = CompareDictionaries(BidLevels, other.BidLevels);

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
                var hashCode = (AskLevels != null ? AskLevels.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BidLevels != null ?  BidLevels.GetHashCode() : 0);
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

        [JsonIgnore]
        public decimal BestAskPrice => AskLevels.Keys.FirstOrDefault();
        [JsonIgnore]
        public decimal BestBidPrice => BidLevels.Keys.FirstOrDefault();

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
            AskLevels = asks;
            BidLevels = bids;
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
            AskLevels = ImmutableSortedDictionary.CreateRange(asks);
            BidLevels = ImmutableSortedDictionary.CreateRange(DescComparer, bids);
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

            AskLevels = ImmutableSortedDictionary.CreateRange(
                asks.Where(x => x.Price != 0M)
                    .GroupBy(x => x.Price)
                    .ToDictionary(x => x.Key, x => x.Sum(i => i.Volume)));

            BidLevels = ImmutableSortedDictionary.CreateRange(
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
        public ImmutableSortedDictionary<decimal, decimal> AskLevels
        {
            get => _asks;
            set => _asks = value;
        }

        [JsonIgnore]
        public IEnumerable<OrderBookItem> Asks => AskLevels.Select(kv => new OrderBookItem(kv.Key, kv.Value));
        [JsonIgnore]
        public IEnumerable<OrderBookItem> Bids => BidLevels.Select(kv => new OrderBookItem(kv.Key, kv.Value));

        [JsonProperty("bids"), JsonConverter(typeof(OrderBookItemsConverter))]
        public ImmutableSortedDictionary<decimal, decimal> BidLevels
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
                AskLevels,
                BidLevels);
        }

        public OrderBook Truncate(int depth)
        {
            return new OrderBook(
                Source,
                Asset,
                Timestamp,
                AskLevels.Take(depth),
                BidLevels.Take(depth));
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
}
