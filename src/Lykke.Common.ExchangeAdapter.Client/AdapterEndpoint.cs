using System;
using JetBrains.Annotations;

namespace Lykke.Common.ExchangeAdapter.Client
{
    /// <summary>
    /// Represents an exchange adapter endpoint settings.
    /// </summary>
    [PublicAPI]
    public struct AdapterEndpoint
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AdapterEndpoint"/>.
        /// </summary>
        /// <param name="xApiKey">The security token to authorize on exchange adapter.</param>
        /// <param name="uri">The address of exchange adapter.</param>
        public AdapterEndpoint(string xApiKey, Uri uri)
        {
            XApiKey = xApiKey;
            Uri = uri;
        }

        /// <summary>
        /// The address of exchange adapter.
        /// </summary>
        public string XApiKey { get; }

        /// <summary>
        /// The security token to authorize on exchange adapter.
        /// </summary>
        public Uri Uri { get; }
    }
}