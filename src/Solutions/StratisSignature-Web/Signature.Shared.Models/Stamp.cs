using Newtonsoft.Json;

namespace Signature.Shared.Models
{
    public class Stamp
    {
        [JsonProperty("agreementId")]
        public string AgreementId { get; set; }

        [JsonProperty("signer")]
        public string Signer { get; set; }

        [JsonProperty("digitalSign")]
        public string DigitalSign { get; set; }

        [JsonProperty("blockNumber")]
        public int BlockNumber { get; set; }
    }
}
