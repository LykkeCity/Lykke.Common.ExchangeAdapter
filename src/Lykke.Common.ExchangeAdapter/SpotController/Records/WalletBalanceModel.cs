using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    /// <summary>
    /// Represents an asset balance details.
    /// </summary>
    public class WalletBalanceModel
    {
        /// <summary>
        /// The asset.
        /// </summary>
        [JsonProperty("asset")]
        public string Asset { get; set; }

        /// <summary>
        /// The amount of asset.
        /// </summary>
        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        /// <summary>
        /// The reserved amount of asset.
        /// </summary>
        [JsonProperty("reserved")]
        public decimal Reserved { get; set; }
    }
}