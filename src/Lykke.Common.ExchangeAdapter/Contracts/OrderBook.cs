﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public sealed class OrderBook
    {
        private bool Equals(OrderBook other)
        {
            return Equals(_asks, other._asks) && Equals(_bids, other._bids) && string.Equals(Source, other.Source)
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
                var hashCode = (_asks != null ? _asks.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_bids != null ? _bids.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Asset != null ? Asset.GetHashCode() : 0);
                return hashCode;
            }
        }

        private readonly IDictionary<decimal, OrderBookItem> _asks = new Dictionary<decimal, OrderBookItem>();
        private readonly IDictionary<decimal, OrderBookItem> _bids = new Dictionary<decimal, OrderBookItem>();

        public OrderBook()
        {
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
            _asks = asks.GroupBy(x => x.Price).ToDictionary(x => x.Key,
                x => new OrderBookItem(x.Key, x.Sum(c => c.Volume)));
            _bids = bids.GroupBy(x => x.Price).ToDictionary(x => x.Key,
                x => new OrderBookItem(x.Key, x.Sum(c => c.Volume)));
        }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("asks")]
        public IEnumerable<OrderBookItem> Asks => _asks.Values;

        [JsonProperty("bids")]
        public IEnumerable<OrderBookItem> Bids => _bids.Values;

        public OrderBook Clone(DateTime timestamp)
        {
            return new OrderBook(
                Source,
                Asset,
                timestamp,
                Asks.ToArray(),
                Bids.ToArray());
        }

        public void UpdateAsk(decimal price, decimal volume)
        {
            UpdateOrderBook(_asks, price, volume);
        }

        private static void UpdateOrderBook(IDictionary<decimal, OrderBookItem> c, decimal price, decimal volume)
        {
            if (volume == 0)
            {
                c.Remove(price);
            }
            else
            {
                c[price] = new OrderBookItem(price, volume);
            }
        }

        public void UpdateBid(decimal price, decimal volume)
        {
            UpdateOrderBook(_bids, price, volume);
        }
    }
}
