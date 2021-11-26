using System;
using System.Configuration;
using System.Windows.Forms;
using SaleDeedRegistry.Wallet.Wallet;

namespace SaleDeedRegistry.Wallet
{
    public partial class FrmMnuemonic : Form
    {
        public FrmMnuemonic()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void FrmMnuemonic_Load(object sender, EventArgs e)
        {
            cmbWordCount.SelectedIndex = 0;
            cmbLanguage.SelectedIndex = 0;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try 
            {
                string url = ConfigurationManager.AppSettings.Get("StratisNodeBaseUrl");
                IWalletServiceProxy walletServiceProxy = new WalletServiceProxy(url);
                string mnemonic = walletServiceProxy.CreateMnemonic(cmbLanguage.Text,
                    int.Parse(cmbWordCount.Text));
                richTextBox1.Text = mnemonic.Replace("\"","");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
