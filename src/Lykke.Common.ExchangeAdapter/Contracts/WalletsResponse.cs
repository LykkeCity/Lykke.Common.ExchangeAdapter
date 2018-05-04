using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public sealed class WalletsResponse
    {
        [JsonProperty("wallets")]
        public IReadOnlyCollection<Wallet> Wallets { get; set; }
    }
}
