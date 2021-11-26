using System;
using System.Configuration;
using System.Windows.Forms;
using SaleDeedRegistry.Wallet.Wallet;

namespace SaleDeedRegistry.Wallet
{
    public partial class FrmWalletInfo : Form
    {
        public FrmWalletInfo()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            txtWalletName.Focus();
        }

        private void btnLoadWalletInfo_Click(object sender, EventArgs e)
        {
            try
            {
                string url = ConfigurationManager.AppSettings.Get("StratisNodeBaseUrl");
                IWalletServiceProxy walletServiceProxy = new WalletServiceProxy(url);
                var response = walletServiceProxy.GetWalletInfo(txtWalletName.Text.Trim());
                if (response != null)
                {
                    txtFilePath.Text = response.walletFilePath;
                    txtNetwork.Text = response.network;
                    txtChainTip.Text = response.chainTip.ToString();
                    txtCreationTime.Text = response.creationTime.ToString();
                    txtIsDecrypted.Text = response.isDecrypted.ToString();
                    txtLastBlockSync.Text = response.lastBlockSyncedHeight.ToString();
                    txtConnectedNodes.Text = response.connectedNodes.ToString();
                    txtChainSynced.Text = response.isChainSynced.ToString();
                }
                else
                {
                    MessageBox.Show("Problem in fetching the wallet info. Please contact the support team");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtChainSynced.Text = "";
            txtChainTip.Text = "";
            txtConnectedNodes.Text = "";
            txtCreationTime.Text = "";
            txtFilePath.Text = "";
            txtIsDecrypted.Text = "";
            txtLastBlockSync.Text = "";
            txtNetwork.Text = "";
            txtWalletName.Text = "";

            // Set focus on the wallet name
            txtWalletName.Focus();
        }

        private void FrmWalletInfo_Load(object sender, EventArgs e)
        {

        }
    }
}
