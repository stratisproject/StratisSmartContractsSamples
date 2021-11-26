using System;
using System.Configuration;
using System.Windows.Forms;
using SaleDeedRegistry.Wallet.Wallet;

namespace SaleDeedRegistry.Wallet
{
    public partial class FrmCreateWallet : Form
    {
        public FrmCreateWallet()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMnemonic.Text))
                {
                    MessageBox.Show("Please specify the Mnemonic", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMnemonic.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtWalletName.Text))
                {
                    MessageBox.Show("Please specify the Wallet Name", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtWalletName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please specify the Wallet Password", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Focus();
                    return;
                }


                string url = ConfigurationManager.AppSettings.Get("StratisNodeBaseUrl");
                IWalletServiceProxy walletServiceProxy = new WalletServiceProxy(url);
                var response = walletServiceProxy.CreateWallet(txtMnemonic.Text.Trim(),
                    txtPassword.Text.Trim(), txtPassphrase.Text.Trim(),
                    txtWalletName.Text.Trim());
                if (response)
                {
                    MessageBox.Show("Successful");
                }
                else
                {
                    MessageBox.Show("Problem in creating the wallet. Please contact the support team");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
