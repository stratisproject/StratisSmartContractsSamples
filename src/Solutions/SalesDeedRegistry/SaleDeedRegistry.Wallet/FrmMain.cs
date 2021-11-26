using System;
using System.Windows.Forms;

namespace SaleDeedRegistry.Wallet
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnCreateMnuemonic_Click(object sender, EventArgs e)
        {
            FrmMnuemonic frmMnuemonic = new FrmMnuemonic();
            frmMnuemonic.ShowDialog();
        }

        private void btnCreateWallet_Click(object sender, EventArgs e)
        {
            FrmCreateWallet frmCreateWallet = new FrmCreateWallet();
            frmCreateWallet.ShowDialog();
        }

        private void btnLoadWallet_Click(object sender, EventArgs e)
        {
            FrmLoadWallet frmLoadWallet = new FrmLoadWallet();
            frmLoadWallet.ShowDialog();
        }

        private void btnRecoverWallet_Click(object sender, EventArgs e)
        {
            FrmRecoverWallet frmRecoverWallet = new FrmRecoverWallet();
            frmRecoverWallet.ShowDialog();
        }

        private void btnWalletInfo_Click(object sender, EventArgs e)
        {
            FrmWalletInfo frmWalletInfo = new FrmWalletInfo();
            frmWalletInfo.ShowDialog();
        }

        private void btnWalletBalance_Click(object sender, EventArgs e)
        {
            FrmWalletBalance frmWalletBalance = new FrmWalletBalance();
            frmWalletBalance.ShowDialog();
        }

        private void btnTransactionHistory_Click(object sender, EventArgs e)
        {
            FrmTransactionHistory frmTransactionHistory = new FrmTransactionHistory();
            frmTransactionHistory.ShowDialog();
        }
    }
}
