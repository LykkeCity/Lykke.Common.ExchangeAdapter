using System;
using System.Collections.Generic;
using Lykke.Common.ExchangeAdapter.Settings;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Common.ExchangeAdapter.Tests
{
    public sealed class settings_reader_tests
    {
        public class CredentialsItem
        {
            public Guid Token { get; set; }
        }

        public sealed class Settings
        {
            [JsonConverter(typeof(CredentialsConverter<CredentialsItem>))]
            public IDictionary<string, CredentialsItem> Credentials { get; set; }
        }

        [Test]
        public void deserializes_dictionary()
        {
            const string serialized = @"
{
    ""Credentials"":
    {
        ""key1"":
        {
            ""Token"": ""6E564569-38C1-4743-B27E-7DFDA3B32F26""
        }
    }
}";

            var deserialized = JsonConvert.DeserializeObject<Settings>(serialized);

            Assert.IsNotNull(deserialized);
            Assert.IsNotNull(deserialized.Credentials);
            Assert.AreEqual(1, deserialized.Credentials.Count);
            Assert.AreEqual(new Guid("6E564569-38C1-4743-B27E-7DFDA3B32F26"), deserialized.Credentials["key1"].Token);
        }

        [Test]
        public void deserializes_array()
        {
            const string serialized = @"
{
    ""Credentials"":
    [
        {
            ""Key"": ""key1"",
            ""Value"":
            {
                ""Token"": ""6E564569-38C1-4743-B27E-7DFDA3B32F26""
            }
        }
    ]
}";

            var deserialized = JsonConvert.DeserializeObject<Settings>(serialized);

            Assert.IsNotNull(deserialized);
            Assert.IsNotNull(deserialized.Credentials);
            Assert.AreEqual(1, deserialized.Credentials.Count);
            Assert.AreEqual(new Guid("6E564569-38C1-4743-B27E-7DFDA3B32F26"), deserialized.Credentials["key1"].Token);
        }
    }
}