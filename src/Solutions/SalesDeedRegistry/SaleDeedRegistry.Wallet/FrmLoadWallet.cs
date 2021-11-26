using SaleDeedRegistry.Wallet.Wallet;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace SaleDeedRegistry.Wallet
{
    public partial class FrmLoadWallet : Form
    {
        public FrmLoadWallet()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                string url = ConfigurationManager.AppSettings.Get("StratisNodeBaseUrl");
                IWalletServiceProxy walletServiceProxy = new WalletServiceProxy(url);
                var response = walletServiceProxy.LoadWallet(txtUserName.Text.Trim(),
                    txtPassword.Text.Trim());
                if (response)
                {
                    MessageBox.Show("Successful");
                }
                else
                {
                    MessageBox.Show("Problem in loading the wallet. Please contact the support team");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
