using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class OrderIdResponse
    {
        [JsonProperty("id")]
        public string OrderId { get; set; }
    }
}