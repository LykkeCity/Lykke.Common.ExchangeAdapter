using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class CancelLimitOrderResponse
    {
        [JsonProperty("orderId")]
        [Required]
        public long OrderId { get; set; }
    }
}