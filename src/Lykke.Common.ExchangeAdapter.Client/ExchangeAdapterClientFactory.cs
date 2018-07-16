using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.HttpClientGenerator;

namespace Lykke.Common.ExchangeAdapter.Client
{
    public class ExchangeAdapterClientFactory
    {
        private readonly IReadOnlyDictionary<string, AdapterEndpoint> _adapters;

        private static readonly ConcurrentDictionary<string, ISpotController> SpotControllers
            = new ConcurrentDictionary<string, ISpotController>();

        private static readonly ConcurrentDictionary<string, IOrderBookController> OrderBookControllers
            = new ConcurrentDictionary<string, IOrderBookController>();

        public ExchangeAdapterClientFactory(
            IReadOnlyDictionary<string, AdapterEndpoint> adapters)
        {
            _adapters = adapters;
        }

        [Obsolete("Use explicit method to get a client GetSpotController/GetOrderBookController")]
        public ISpotController this[string adapter] =>
            SpotControllers.GetOrAdd(adapter, CreateNewClient<ISpotController>);

        private T CreateNewClient<T>(string adapter)
        {
            if (!_adapters.TryGetValue(adapter, out var endpoint))
            {
                throw new ArgumentException($"No service endpoint defined for {adapter}", nameof(adapter));
            }

             var builder = new HttpClientGeneratorBuilder(endpoint.Uri.ToString())
                 .WithAdditionalDelegatingHandler(new XApiKeyHandler(endpoint.XApiKey))
                 .WithoutRetries()
                 .Create();

             return builder.Generate<T>();
        }

        public ISpotController GetSpotController(string adapter)
        {
            return SpotControllers.GetOrAdd(adapter, CreateNewClient<ISpotController>);
        }

        public IOrderBookController GetOrderBookController(string adapter)
        {
            return OrderBookControllers.GetOrAdd(adapter, CreateNewClient<IOrderBookController>);
        }
    }
}