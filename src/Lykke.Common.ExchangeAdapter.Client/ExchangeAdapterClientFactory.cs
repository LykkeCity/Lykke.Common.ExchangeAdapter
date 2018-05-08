using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.HttpClientGenerator;

namespace Lykke.Common.ExchangeAdapter.Client
{
    public class ExchangeAdapterClientFactory
    {
        private readonly IReadOnlyDictionary<Adapter, AdapterEndpoint> _adapters;

        private static readonly ConcurrentDictionary<Adapter, ISpotController> Instances
            = new ConcurrentDictionary<Adapter, ISpotController>();

        public ExchangeAdapterClientFactory(
            IReadOnlyDictionary<Adapter, AdapterEndpoint> adapters)
        {
            _adapters = adapters;
        }

        public ISpotController this[Adapter adapter] => Instances.GetOrAdd(adapter, CreateNewClient);

        private ISpotController CreateNewClient(Adapter adapter)
        {
            if (!_adapters.TryGetValue(adapter, out var endpoint))
            {
                throw new ArgumentException($"No service endpoint defined for {adapter:G}", nameof(adapter));
            }

             var builder = new HttpClientGeneratorBuilder(endpoint.Uri.ToString())
                 .WithAdditionalDelegatingHandler(new XApiKeyHandler(endpoint.XApiKey))
                 .Create();


             return builder.Generate<ISpotController>();
        }
    }
}