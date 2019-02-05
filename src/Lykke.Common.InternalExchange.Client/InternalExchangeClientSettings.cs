using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Common.InternalExchange.Client
{
    /// <summary>
    /// Internal exchange client settings.
    /// </summary>
    [PublicAPI]
    public class InternalExchangeClientSettings
    {
        /// <summary>
        /// Service url.
        /// </summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }
    }
}