using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.HttpClientGenerator;
using Refit;

namespace Lykke.Common.ExchangeAdapter.Client
{
    public class ExchangeAdapterClientFactory
    {
        private readonly IReadOnlyDictionary<Adapter, string> _credentials;

        private const string AdaptersNamespace = "lykke-exchange-adapter";
        private const string KubernetesSvcSuffix = "svc.cluster.local";

        private static readonly IReadOnlyDictionary<Adapter, string> DefaultServices = new Dictionary<Adapter, string>
        {
            {Adapter.BitFinex, "bitfinex-adapter"},
            {Adapter.CexIo, "cexio-adapter"},
            {Adapter.Icm, "icm-adapter"},
            {Adapter.KuCoin, "kucoin-adapter"},
            {Adapter.Lykke, "lykke-adapter"}
        };

        private static readonly ConcurrentDictionary<Adapter, ISpotController> Clients
            = new ConcurrentDictionary<Adapter, ISpotController>();

        private readonly IReadOnlyDictionary<Adapter, Uri> _adapterEndpoints;

        public ExchangeAdapterClientFactory(
            IReadOnlyDictionary<Adapter, string> credentials,
            IReadOnlyDictionary<Adapter, Uri> adapterEndpoints = null)
        {
            _credentials = credentials;

            _adapterEndpoints = adapterEndpoints ?? DefaultEndpoints().ToDictionary(x => x.Key, x => x.Value);
        }

        private static IEnumerable<KeyValuePair<Adapter, Uri>> DefaultEndpoints()
        {
            foreach (var svc in DefaultServices)
            {
                var uriBuilder = new UriBuilder
                {
                    Scheme = "http",
                    Host = $"{svc.Value}.{AdaptersNamespace}.{KubernetesSvcSuffix}"
                };

                yield return KeyValuePair.Create(svc.Key, uriBuilder.Uri);
            }
        }

        public ISpotController this[Adapter adapter] => Clients.GetOrAdd(adapter, CreateNewClient);

        private ISpotController CreateNewClient(Adapter adapter)
        {
            if (!_adapterEndpoints.TryGetValue(adapter, out var endpoint))
            {
                throw new ArgumentException($"No service endpoint defined for {adapter:G}", nameof(adapter));
            }

            if (!_credentials.TryGetValue(adapter, out var apiKey))
            {
                throw new ArgumentException($"No API KEY specified for {adapter:G}", nameof(adapter));
            }

             var builder = new HttpClientGeneratorBuilder(endpoint.ToString())
                 .WithAdditionalDelegatingHandler(new XApiKeyHandler(apiKey))
                 .Create();


             return builder.Generate<ISpotController>();
        }
    }
}