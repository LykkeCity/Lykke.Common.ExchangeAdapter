using System.Collections.Generic;

namespace Lykke.Common.ExchangeAdapter.Server.Settings
{
    public sealed class OrderBookProcessingSettings
    {
        public IReadOnlyCollection<string> AllowedAnomalisticAssets { get; set; }

        public int MaxEventPerSecondByInstrument { get; set; }
        public int OrderBookDepth { get; set; }
        public RmqOutput OrderBooks { get; set; }
        public RmqOutput TickPrices { get; set; }
    }
}