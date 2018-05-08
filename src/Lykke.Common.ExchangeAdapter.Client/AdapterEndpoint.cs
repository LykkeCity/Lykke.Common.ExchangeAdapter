using System;

namespace Lykke.Common.ExchangeAdapter.Client
{
    public struct AdapterEndpoint
    {
        public readonly Uri Uri;
        public readonly string XApiKey;

        public AdapterEndpoint(string xApiKey, Uri uri)
        {
            XApiKey = xApiKey;
            Uri = uri;
        }
    }
}