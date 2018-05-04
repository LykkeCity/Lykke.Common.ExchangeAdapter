using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public class Wallet
    {
        [JsonProperty("asset")]
        public string Asset { get; set; }
        [JsonProperty("balance")]
        public decimal Balance { get; set; }
        [JsonProperty("reserved")]
        public decimal Reserved { get; set; }
    }
}
