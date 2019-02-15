using System;
using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;

namespace Lykke.Common.ExchangeAdapter.Client
{
    /// <summary>
    /// Extension for client registration.
    /// </summary>
    [PublicAPI]
    public static class AutofacExtension
    {
        /// <summary>
        /// Registers <see cref="IExchangeAdapterClientFactory"/> in Autofac container using exchange adapters settings.
        /// </summary>
        /// <param name="builder">Autofac container builder.</param>
        /// <param name="adapters">The exchange adapters settings.</param>
        public static void RegisterIndexHedgingEngineClient(
            [NotNull] this ContainerBuilder builder,
            [NotNull] IReadOnlyDictionary<string, AdapterEndpoint> adapters)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (adapters == null)
                throw new ArgumentNullException(nameof(adapters));

            builder.RegisterInstance(new ExchangeAdapterClientFactory(adapters))
                .As<IExchangeAdapterClientFactory>()
                .SingleInstance();
        }
    }
}