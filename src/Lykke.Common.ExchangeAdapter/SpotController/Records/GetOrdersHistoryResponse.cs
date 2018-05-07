﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public class GetOrdersHistoryResponse
    {
        [JsonProperty("Orders")]
        public IReadOnlyCollection<OrderModel> Orders { get; set; }
    }
}