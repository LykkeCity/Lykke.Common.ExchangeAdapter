using System;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public struct OrderBookItem
    {
        public OrderBookItem(decimal price, decimal volume)
        {
            if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price), "Price should be greater than zero");

            Price = price;

            // some adapters use negative volume to distinguish bids and asks
            // fixing that issue in one place
            Volume = Math.Abs(volume);
        }

        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("volume")]
        public decimal Volume { get; set; }
    }
}
