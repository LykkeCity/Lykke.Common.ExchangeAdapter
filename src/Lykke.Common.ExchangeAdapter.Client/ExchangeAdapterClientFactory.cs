using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.HttpClientGenerator;

namespace Lykke.Common.ExchangeAdapter.Client
{
    public class ExchangeAdapterClientFactory
    {
        private readonly IReadOnlyDictionary<string, AdapterEndpoint> _adapters;

        private static readonly ConcurrentDictionary<string, ISpotController> Instances
            = new ConcurrentDictionary<string, ISpotController>();

        public ExchangeAdapterClientFactory(
            IReadOnlyDictionary<string, AdapterEndpoint> adapters)
        {
            _adapters = adapters;
        }

        public ISpotController this[string adapter] => Instances.GetOrAdd(adapter, CreateNewClient);

        private ISpotController CreateNewClient(string adapter)
        {
            if (!_adapters.TryGetValue(adapter, out var endpoint))
            {
                throw new ArgumentException($"No service endpoint defined for {adapter}", nameof(adapter));
            }

             var builder = new HttpClientGeneratorBuilder(endpoint.Uri.ToString())
                 .WithAdditionalDelegatingHandler(new XApiKeyHandler(endpoint.XApiKey))
                 .WithoutRetries()
                 .Create();


             return builder.Generate<ISpotController>();
        }
    }
}