using Newtonsoft.Json;

namespace Signature.Shared.Models
{
    public class GasConsumed
    {
        [JsonProperty("value")]
        public int Value { get; set; }
    }
}
