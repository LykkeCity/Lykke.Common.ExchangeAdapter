using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Common.InternalExchange.Client.Models
{
    /// <summary>
    /// Represents order.
    /// </summary>
    [PublicAPI]
    public class OrderModel
    {
        /// <summary>
        /// Id of the order.
        /// </summary>
        public string OrderId { set; get; }

        /// <summary>
        /// Status of the order.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus Status { set; get; }

        /// <summary>
        /// Id of the wallet.
        /// </summary>
        public string WalletId { set; get; }

        /// <summary>
        /// Id of the asset pair.
        /// </summary>
        public string AssetPair { set; get; }

        /// <summary>
        /// Direction of the order.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType Type { set; get; }

        /// <summary>
        /// Desired volume.
        /// </summary>
        public decimal OriginalVolume { set; get; }

        /// <summary>
        /// Desired price.
        /// </summary>
        public decimal OriginalPrice { set; get; }

        /// <summary>
        /// Actual price of execution.
        /// </summary>
        public decimal? ExecutedPrice { set; get; }

        /// <summary>
        /// Actual volume of execution.
        /// </summary>
        public decimal? ExecutedVolume { set; get; }

        /// <summary>
        /// Rejection reason, in case order is rejected.
        /// </summary>
        public string RejectReason { set; get; }

        /// <summary>
        /// DateTime when order was created.
        /// </summary>
        public DateTime CreatedDate { set; get; }
    }
}