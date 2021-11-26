using Newtonsoft.Json;
using System;

namespace Ticketbooth.Api.Converters
{
    public class ByteArrayToHexConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var hexString = (string)reader.Value;
            int characterCount = hexString.Length;
            byte[] result = new byte[characterCount / 2];
            for (int i = 0; i < characterCount; i += 2)
            {
                result[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(BitConverter.ToString((byte[])value).Replace("-", string.Empty));
        }
    }
}
