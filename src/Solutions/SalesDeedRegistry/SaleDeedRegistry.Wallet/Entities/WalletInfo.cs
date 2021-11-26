namespace SaleDeedRegistry.Wallet.Entities
{
    public class WalletInfo
    {
        public string walletFilePath { get; set; }
        public string network { get; set; }
        public string creationTime { get; set; }
        public bool isDecrypted { get; set; }
        public int lastBlockSyncedHeight { get; set; }
        public int chainTip { get; set; }
        public bool isChainSynced { get; set; }
        public int connectedNodes { get; set; }
    }
}
