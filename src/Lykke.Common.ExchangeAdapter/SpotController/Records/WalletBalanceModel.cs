using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class WalletBalanceModel
    {
        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        //[JsonProperty("available")]
        //public decimal Available { get; set; }

        [JsonProperty("reserved")]
        public decimal Reserved { get; set; } //Balance - available

        //[JsonProperty("type")]
        //public string Type { get; set; }
    }
}