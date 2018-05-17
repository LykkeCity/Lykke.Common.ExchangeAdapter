using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Settings
{
    public class CredentialsConverter<T> : JsonConverter<IReadOnlyDictionary<string, T>>
    {
        public override void WriteJson(JsonWriter writer, IReadOnlyDictionary<string, T> value, JsonSerializer serializer)
        {
            throw new NotSupportedException("Current converted is not intented to update JSON");
        }

        public override IReadOnlyDictionary<string, T> ReadJson(
            JsonReader reader,
            Type objectType,
            IReadOnlyDictionary<string, T> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return existingValue;
            }


            IEnumerable<KeyValuePair<string, T>> pairs;

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return existingValue;
                case JsonToken.StartArray:
                    pairs = serializer.Deserialize<IEnumerable<KeyValuePair<string, T>>>(reader);
                    break;
                case JsonToken.StartObject:
                    pairs = serializer.Deserialize<IDictionary<string, T>>(reader).AsEnumerable();
                    break;
                default:
                    throw new JsonSerializationException($"JsonToken {reader.TokenType:G} is not supported for Credentials");
            }

            var result = new Dictionary<string, T>();

            foreach (var kv in (existingValue ?? Enumerable.Empty<KeyValuePair<string, T>>()).Concat(pairs))
            {
                result[kv.Key] = kv.Value;
            }

            return result;
        }
    }
}