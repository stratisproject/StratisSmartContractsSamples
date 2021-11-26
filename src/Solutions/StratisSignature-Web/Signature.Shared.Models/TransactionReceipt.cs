using System.Collections.Generic;
using Newtonsoft.Json;

namespace Signature.Shared.Models
{
    public class TransactionReceipt
    {
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }

        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        [JsonProperty("postState")]
        public string PostState { get; set; }

        [JsonProperty("gasUsed")]
        public int GasUsed { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("newContractAddress")]
        public object NewContractAddress { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("returnValue")]
        public string ReturnValue { get; set; }

        [JsonProperty("bloom")]
        public string Bloom { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("logs")]
        public List<object> Logs { get; set; }
    }
}
