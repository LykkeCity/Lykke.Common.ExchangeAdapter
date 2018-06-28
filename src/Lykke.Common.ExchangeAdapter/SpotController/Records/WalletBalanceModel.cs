using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class WalletBalanceModel
    {
        private string _asset;

        [JsonProperty("asset")]
        public string Asset
        {
            get => _asset;
            set => _asset = value?.ToUpperInvariant();
        }

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