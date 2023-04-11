using Newtonsoft.Json;
using System;

namespace MoravianStar.WebAPI.JsonConverters
{
    /// <summary>
    /// Custom <see cref="string"/> type json converter, that is used for implementing various logics around serialization and deserialization of <see cref="string"/> properties.
    /// </summary>
    public class CustomStringTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                if (reader.Value != null)
                {
                    return (reader.Value as string).Trim();
                }
            }
            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string str = (string)value;
            if (str == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(str);
            }
        }
    }
}