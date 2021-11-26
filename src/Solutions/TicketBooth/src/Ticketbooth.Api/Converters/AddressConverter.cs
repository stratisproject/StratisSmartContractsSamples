using NBitcoin;
using Newtonsoft.Json;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;

namespace Ticketbooth.Api.Converters
{
    public class AddressConverter : JsonConverter
    {
        private readonly Network _network;

        public AddressConverter(Network network)
        {
            _network = network;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Address);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ((string)reader.Value).ToAddress(_network);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((Address)value).ToUint160().ToBase58Address(_network));
        }
    }
}
