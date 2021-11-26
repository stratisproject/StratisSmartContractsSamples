using SaleDeedRegistry.Wallet.Entities;
using SaleDeedRegistry.Wallet.Wallet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace SaleDeedRegistry.Wallet
{
    public partial class FrmWalletBalance : Form
    {
        List<Address> Addresses = new List<Address>();

        public FrmWalletBalance()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen; 
            dataGridView1.AutoGenerateColumns = false;
        }

        private void SetDataGridColumns()
        {
            dataGridView1.Columns.Add("address", "Address");
            dataGridView1.Columns.Add("isUsed", "Is Used");
            dataGridView1.Columns.Add("isChange", "Is Change");
            dataGridView1.Columns.Add("amountConfirmed", "Amount Confirmed");
            dataGridView1.Columns.Add("amountUnconfirmed", "Amount Unconfirmed");

            dataGridView1.Columns[0].DataPropertyName = "address";
            dataGridView1.Columns[0].Width = 250;
            dataGridView1.Columns[1].DataPropertyName = "isUsed";

            dataGridView1.Columns[2].DataPropertyName = "isChange";
            dataGridView1.Columns[3].DataPropertyName = "amountConfirmed";
            dataGridView1.Columns[4].DataPropertyName = "amountUnconfirmed";
        }

        private void FrmWalletBalance_Load(object sender, System.EventArgs e)
        {
            SetDataGridColumns();
        }

        private void btnGetBalance_Click(object sender, EventArgs e)
        {
            try
            {
                string url = ConfigurationManager.AppSettings.Get("StratisNodeBaseUrl");
                IWalletServiceProxy walletServiceProxy = new WalletServiceProxy(url);
                var response = walletServiceProxy.GetWalletBalance(txtWalletName.Text.Trim());
                if (response != null && response.balances.Count > 0)
                {
                    dataGridView1.DataSource = response.balances[0].addresses;

                    txtAccountName.Text = response.balances[0].accountName;
                    txtAccountHDPath.Text = response.balances[0].accountHdPath;
                    txtAccountConfirmed.Text = response.balances[0].amountConfirmed.ToString();
                    txtAccountUnConfirmed.Text = response.balances[0].amountUnconfirmed.ToString();
                    txtCoinType.Text = response.balances[0].coinType.ToString();
                    txtSpendableAmount.Text = response.balances[0].spendableAmount.ToString();
                }
                else
                {
                    dataGridView1.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtWalletName.Text = "";
            txtSpendableAmount.Text = "";
            txtCoinType.Text = "";
            txtAccountUnConfirmed.Text = "";
            txtAccountName.Text = "";
            txtAccountHDPath.Text = "";
            txtAccountConfirmed.Text = "";
        }
    }
}
