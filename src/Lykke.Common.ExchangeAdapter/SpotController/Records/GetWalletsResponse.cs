using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class GetWalletsResponse
    {
        [JsonProperty("wallets")]
        public IReadOnlyCollection<WalletBalanceModel> Wallets { get; set; }
    }
}