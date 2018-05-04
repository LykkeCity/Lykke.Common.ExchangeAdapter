using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public sealed class GetLimitOrdersResponse
    {
        [JsonProperty("Orders")]
        public IReadOnlyCollection<Order> Orders { get; set; }
    }
}