using System.Collections.Generic;

namespace SaleDeedRegistry.Lib.Client
{
    public class ReceiptResponse
    {
        public string transactionHash { get; set; }
        public string blockHash { get; set; }
        public string postState { get; set; }
        public int gasUsed { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public object newContractAddress { get; set; }
        public bool success { get; set; }
        public string returnValue { get; set; }
        public string bloom { get; set; }
        public object error { get; set; }
        public List<object> logs { get; set; }
    }
}
