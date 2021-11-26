using Newtonsoft.Json;

namespace Signature.Shared.Models
{    
    public class TransactionResponse
    {
        [JsonProperty("fee")]
        public int Fee { get; set; }

        [JsonProperty("hex")]
        public string Hex { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }
    }
}
