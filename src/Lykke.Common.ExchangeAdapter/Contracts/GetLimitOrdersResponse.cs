using System.Collections.Generic;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public sealed class GetLimitOrdersResponse
    {
        [JsonProperty("Orders")]
        public IReadOnlyCollection<Order> Orders { get; set; }
    }
}