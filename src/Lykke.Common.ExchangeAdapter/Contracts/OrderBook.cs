using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    /// <summary>
    /// Represents an order book.
    /// </summary>
    public sealed class OrderBook
    {
        private static readonly DescendingComparer<decimal> DescComparer = new DescendingComparer<decimal>();

        private ImmutableSortedDictionary<decimal, decimal> _asks
            = ImmutableSortedDictionary<decimal, decimal>.Empty;

        private ImmutableSortedDictionary<decimal, decimal> _bids
            = ImmutableSortedDictionary.Create<decimal, decimal>(DescComparer);

        /// <summary>
        /// Initializes a new instance of <see cref="OrderBook"/>.
        /// </summary>
        public OrderBook()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OrderBook"/> with details.
        /// </summary>
        /// <param name="source">The name of exchange.</param>
        /// <param name="asset">The asset pair.</param>
        /// <param name="timestamp">The creation time of order book. </param>
        /// <param name="asks">The levels of sell orders in order book.</param>
        /// <param name="bids">The levels of buy orders in order book.</param>
        private OrderBook(
            string source,
            string asset,
            DateTime timestamp,
            IEnumerable<KeyValuePair<decimal, decimal>> asks,
            IEnumerable<KeyValuePair<decimal, decimal>> bids)
        {
            Source = source;
            Asset = asset;
            Timestamp = timestamp;
            AskLevels = ImmutableSortedDictionary.CreateRange(asks);
            BidLevels = ImmutableSortedDictionary.CreateRange(DescComparer, bids);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OrderBook"/> with details.
        /// </summary>
        /// <param name="source">The name of exchange.</param>
        /// <param name="asset">The asset pair.</param>
        /// <param name="timestamp">The creation time of order book. </param>
        /// <param name="asks">The levels of sell orders in order book.</param>
        /// <param name="bids">The levels of buy orders in order book.</param>
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

        private OrderBook(
            string source,
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
        /// The creation time of order book. 
        /// </summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The levels of sell orders in order book.
        /// </summary>
        [JsonProperty("asks")]
        [JsonConverter(typeof(OrderBookItemsConverter))]
        public ImmutableSortedDictionary<decimal, decimal> AskLevels
        {
            get => _asks;
            set => _asks = value;
        }

        /// <summary>
        /// The levels of buy orders in order book.
        /// </summary>
        [JsonProperty("bids")]
        [JsonConverter(typeof(OrderBookItemsConverter))]
        public ImmutableSortedDictionary<decimal, decimal> BidLevels
        {
            get => _bids;
            set => _bids = value;
        }

        /// <summary>
        /// The best sell price in order book.
        /// </summary>
        [JsonIgnore]
        public decimal BestAskPrice => AskLevels.Keys.FirstOrDefault();

        /// <summary>
        /// The best buy price in order book.
        /// </summary>
        [JsonIgnore]
        public decimal BestBidPrice => BidLevels.Keys.FirstOrDefault();

        /// <summary>
        /// The levels of sell orders in order book.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<OrderBookItem> Asks => AskLevels.Select(kv => new OrderBookItem(kv.Key, kv.Value));

        /// <summary>
        /// The levels of buy orders in order book.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<OrderBookItem> Bids => BidLevels.Select(kv => new OrderBookItem(kv.Key, kv.Value));

        /// <summary>
        /// Creates a new order book that is a copy of the current instance.
        /// </summary>
        /// <param name="timestamp">The creation time of order book.</param>
        /// <returns>The new instance of <see cref="OrderBook"/>.</returns>
        public OrderBook Clone(DateTime timestamp)
        {
            return new OrderBook(
                Source,
                Asset,
                timestamp,
                AskLevels,
                BidLevels);
        }

        /// <summary>
        /// Creates a new order book that is a copy of the current instance with truncated number of levels.
        /// </summary>
        /// <param name="depth">The number of <see cref="Asks"/> and <see cref="Bids"/> levels.</param>
        /// <returns>The new instance of <see cref="OrderBook"/>.</returns>
        public OrderBook Truncate(int depth)
        {
            return new OrderBook(
                Source,
                Asset,
                Timestamp,
                AskLevels.Take(depth),
                BidLevels.Take(depth));
        }

        /// <summary>
        /// Updates level of sell order if <paramref name="volume"/> is greater than zero, otherwise removes level.   
        /// </summary>
        /// <param name="price">The price of level.</param>
        /// <param name="volume">The volume of level.</param>
        public void UpdateAsk(decimal price, decimal volume)
        {
            UpdateOrderBook(ref _asks, price, volume);
        }

        /// <summary>
        /// Updates level of buy order if <paramref name="volume"/> is greater than zero, otherwise removes level.   
        /// </summary>
        /// <param name="price">The price of level.</param>
        /// <param name="volume">The volume of level.</param>
        public void UpdateBid(decimal price, decimal volume)
        {
            UpdateOrderBook(ref _bids, price, volume);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is OrderBook book && Equals(book);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AskLevels != null ? AskLevels.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (BidLevels != null ? BidLevels.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Asset != null ? Asset.GetHashCode() : 0);
                return hashCode;
            }
        }

        private bool Equals(OrderBook other)
        {
            if (!string.Equals(Asset, other.Asset))
                return false;

            if (!string.Equals(Source, other.Source))
                return false;

            return CompareDictionaries(AskLevels, other.AskLevels) && CompareDictionaries(BidLevels, other.BidLevels);
        }

        private static void UpdateOrderBook(ref ImmutableSortedDictionary<decimal, decimal> c, decimal price,
            decimal volume)
        {
            c = volume == 0 ? c.Remove(price) : c.SetItem(price, volume);
        }

        private static bool CompareDictionaries<TK, TV>(IDictionary<TK, TV> first, IDictionary<TK, TV> second)
        {
            if (first.Count != second.Count)
                return false;

            return second.All(entry => first.TryGetValue(entry.Key, out TV value) && entry.Value.Equals(value));
        }

        private class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                if (y == null)
                    return -1;

                return y.CompareTo(x);
            }
        }
    }
}