using System.Collections.Generic;

namespace SaleDeedRegistry.Wallet.Entities
{
    public class Payment
    {
        public string destinationAddress { get; set; }
        public ulong amount { get; set; }
    }

    public class TransactionsHistory
    {
        public string type { get; set; }
        public string id { get; set; }
        public ulong amount { get; set; }
        public List<Payment> payments { get; set; }
        public int fee { get; set; }
        public int confirmedInBlock { get; set; }
        public string timestamp { get; set; }
        public string toAddress { get; set; }
        public int? blockIndex { get; set; }
    }

    public class HistoryViewModel : TransactionsHistory
    {
        public int coinType { get; set; }
        public string accountName { get; set; }
    }

    public class History
    {
        public string accountName { get; set; }
        public string accountHdPath { get; set; }
        public int coinType { get; set; }
        public List<TransactionsHistory> transactionsHistory { get; set; }
    }

    public class WalletHistoryRoot
    {
        public List<History> history { get; set; }
    }
}
