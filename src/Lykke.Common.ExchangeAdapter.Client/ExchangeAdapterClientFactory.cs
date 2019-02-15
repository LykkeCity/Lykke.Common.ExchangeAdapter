using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.HttpClientGenerator;

namespace Lykke.Common.ExchangeAdapter.Client
{
    /// <inheritdoc />
    [PublicAPI]
    public class ExchangeAdapterClientFactory : IExchangeAdapterClientFactory
    {
        private readonly IReadOnlyDictionary<string, AdapterEndpoint> _adapters;

        private static readonly ConcurrentDictionary<string, ISpotController> SpotControllers
            = new ConcurrentDictionary<string, ISpotController>();

        private static readonly ConcurrentDictionary<string, IOrderBookController> OrderBookControllers
            = new ConcurrentDictionary<string, IOrderBookController>();

        /// <summary>
        /// Initializes a new instance of <see cref="ExchangeAdapterClientFactory"/>.
        /// </summary>
        /// <param name="adapters">The exchange adapters settings.</param>
        public ExchangeAdapterClientFactory(IReadOnlyDictionary<string, AdapterEndpoint> adapters)
        {
            _adapters = adapters;
        }

        /// <inheritdoc />
        public ISpotController GetSpotController(string adapter)
        {
            return SpotControllers.GetOrAdd(adapter, CreateNewClient<ISpotController>);
        }

        /// <inheritdoc />
        public IOrderBookController GetOrderBookController(string adapter)
        {
            return OrderBookControllers.GetOrAdd(adapter, CreateNewClient<IOrderBookController>);
        }

        private T CreateNewClient<T>(string adapter)
        {
            if (!_adapters.TryGetValue(adapter, out AdapterEndpoint endpoint))
                throw new ArgumentException($"No service endpoint defined for {adapter}", nameof(adapter));

            var builder = new HttpClientGeneratorBuilder(endpoint.Uri.ToString())
                .WithAdditionalDelegatingHandler(new XApiKeyHandler(endpoint.XApiKey))
                .WithAdditionalDelegatingHandler(new HandleBusinessExceptionsHandler())
                .WithoutRetries()
                .Create();

            return builder.Generate<T>();
        }
    }
}