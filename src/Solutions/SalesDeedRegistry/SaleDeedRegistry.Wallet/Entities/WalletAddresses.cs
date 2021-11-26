using System.Collections.Generic;

namespace SaleDeedRegistry.Wallet.Entities
{
    public class WalletAddresses
    {
        public string address { get; set; }
        public bool isUsed { get; set; }
        public bool isChange { get; set; }
        public long amountConfirmed { get; set; }
        public int amountUnconfirmed { get; set; }
        public object addresses { get; set; }
    }

    public class WalletAddressesRoot
    {
        public List<WalletAddresses> addresses { get; set; }
    }
}
