using System;
using System.Configuration;
using System.Windows.Forms;
using SaleDeedRegistry.Wallet.Wallet;

namespace SaleDeedRegistry.Wallet
{
    public partial class FrmRecoverWallet : Form
    {
        public FrmRecoverWallet()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void FrmRecoverWallet_Load(object sender, EventArgs e)
        {

        }

        private void btnRecover_Click(object sender, EventArgs e)
        {
            try
            {
                string url = ConfigurationManager.AppSettings.Get("StratisNodeBaseUrl");
                IWalletServiceProxy walletServiceProxy = new WalletServiceProxy(url);
                var response = walletServiceProxy.RecoverWallet(txtUserName.Text.Trim(),
                    txtPassword.Text.Trim(), txtMnumonic.Text,
                    DateTime.Parse(txtCreatedDate.Text));
                if (response)
                {
                    MessageBox.Show("Successful");
                }
                else
                {
                    MessageBox.Show("Problem in recovering the wallet. Please contact the support team");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
