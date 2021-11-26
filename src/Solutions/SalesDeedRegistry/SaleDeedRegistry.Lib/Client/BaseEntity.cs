namespace SaleDeedRegistry.Lib
{
    public class SaleDeedRegistryBaseRequest
    {
        public int GasPrice { get; set; }
        public int GasLimit { get; set; }
        public int Amount { get; set; }
        public double FeeAmount { get; set; }
        public string WalletName { get; set; }
        public string WalletPassword { get; set; }
        public string Sender { get; set; }

        public string BuyerAddress { get; set; }
        public string OwnerAddress { get; set; }
        public string AssetId { get; set; }
    }
}
