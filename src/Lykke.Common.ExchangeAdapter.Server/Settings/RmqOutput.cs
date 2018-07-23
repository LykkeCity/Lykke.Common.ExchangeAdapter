using Lykke.SettingsReader.Attributes;

namespace Lykke.Common.ExchangeAdapter.Server.Settings
{
        public sealed class RmqOutput
        {
            [AmqpCheck]
            public string ConnectionString { get; set; }
            public string Exchanger { get; set; }

            [Optional]
            public bool Durable { get; set; } = true;

            [Optional]
            public bool Enabled { get; set; } = true;
        }
}