using System.Collections.Generic;
using Newtonsoft.Json;

namespace Signature.Shared.Models
{
    public class Agreement
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fileHash")]
        public string FileHash { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("signedFileHash")]
        public object SignedFileHash { get; set; }

        [JsonProperty("signers")]
        public List<string> Signers { get; set; }

        [JsonProperty("totalSignDone")]
        public int TotalSignDone { get; set; }

        [JsonProperty("isSigned")]
        public bool IsSigned { get; set; }
    }
}
