using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public sealed class OrderBookItemsConverter : JsonConverter<ImmutableSortedDictionary<decimal, decimal>>
    {
        public override void WriteJson(
            JsonWriter writer,
            ImmutableSortedDictionary<decimal, decimal> value,
            JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var item in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("price");
                writer.WriteRawValue(JsonConvert.ToString(item.Key));

                writer.WritePropertyName("volume");
                writer.WriteRawValue(JsonConvert.ToString(item.Value));

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public override ImmutableSortedDictionary<decimal, decimal> ReadJson(
            JsonReader reader,
            Type objectType,
            ImmutableSortedDictionary<decimal, decimal> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (!hasExistingValue)
            {
                throw new JsonSerializationException("Expected non empty ImmutableSortedDictionary instance");
            }

            var range = ReadJsonObject(reader).GroupBy(x => x.Key).Select(grouping => KeyValuePair.Create(
                grouping.Key,
                grouping.Sum(t => t.Value)));

            return ImmutableSortedDictionary.CreateRange(
                existingValue.KeyComparer,
                range);
        }

        private static IEnumerable<KeyValuePair<decimal, decimal>> ReadJsonObject(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                if (reader.Read())
                {
                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            decimal p = 0;
                            decimal v = 0;
                            reader.Read();


                            while (reader.TokenType == JsonToken.PropertyName)
                            {
                                if ((string) reader.Value == "price")
                                    p = reader.ReadAsDecimal() ?? 0;
                                else
                                {
                                    v = reader.ReadAsDecimal() ?? 0;
                                }

                                reader.Read();
                            }

                            yield return KeyValuePair.Create(p, v);

                            reader.Read();
                        }
                    }
                }
            }
        }
    }
}