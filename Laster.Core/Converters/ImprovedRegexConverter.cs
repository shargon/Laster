using Newtonsoft.Json;
using System;

namespace Laster.Core.Converters
{
    public class ImprovedRegexConverter : Newtonsoft.Json.Converters.RegexConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}