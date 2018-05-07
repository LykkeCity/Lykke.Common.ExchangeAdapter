using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class ReplaceLimitOrderRequest : LimitOrderRequest
    {
        /// <summary>
        /// id of order for cancel(repalce)
        /// </summary>
        [JsonProperty("orderId")]
        public long OrderIdToCancel { get; set; }
    }
}