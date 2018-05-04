using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public sealed class OrderBook
    {
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

        private static void UpdateOrderBook(
            IDictionary<decimal, OrderBookItem> collection,
            decimal price,
            decimal volume)
        {
            if (volume == 0)
            {
                collection.Remove(price);
            }
            else
            {
                collection[price] = new OrderBookItem(price, volume);
            }
        }

        public void UpdateAsk(decimal price, decimal volume)
        {
            UpdateOrderBook(_asks, price, volume);
        }

        public void UpdateBid(decimal price, decimal volume)
        {
            UpdateOrderBook(_bids, price, volume);
        }
    }
}
