using System.Collections.Generic;

namespace SaleDeedRegistry.Wallet.Entities
{
    public class Address
    {
        public string address { get; set; }
        public bool isUsed { get; set; }
        public bool isChange { get; set; }
        public long amountConfirmed { get; set; }
        public int amountUnconfirmed { get; set; }
        public object addresses { get; set; }
    }

    public class Balance
    {
        public string accountName { get; set; }
        public string accountHdPath { get; set; }
        public int coinType { get; set; }
        public long amountConfirmed { get; set; }
        public int amountUnconfirmed { get; set; }
        public long spendableAmount { get; set; }
        public List<Address> addresses { get; set; }
    }

    public class WalletBalanceRoot
    {
        public List<Balance> balances { get; set; }
    }
}
