using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public struct OrderBookItem
    {
        public OrderBookItem(decimal price, decimal volume)
        {
            Price = price;
            Volume = volume;
        }

        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("volume")]
        public decimal Volume { get; set; }
    }
}
