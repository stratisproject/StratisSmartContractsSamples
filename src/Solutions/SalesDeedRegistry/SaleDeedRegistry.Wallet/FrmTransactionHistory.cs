using System;
using System.Configuration;
using System.Windows.Forms;
using System.Collections.Generic;
using SaleDeedRegistry.Wallet.Wallet;
using SaleDeedRegistry.Wallet.Entities;

namespace SaleDeedRegistry.Wallet
{
    public partial class FrmTransactionHistory : Form
    {
        private List<HistoryViewModel> historyViewModels = new List<HistoryViewModel>();

        public FrmTransactionHistory()
        {
            InitializeComponent();

            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            dataGridView1.AutoGenerateColumns = false;
            cmbPaymentType.SelectedIndex = 0;
        }

        private void FrmTransactionHistory_Load(object sender, EventArgs e)
        {
            SetDataGridColumns();
        }

        private void SetDataGridColumns()
        {
            dataGridView1.Columns.Add("accountName", "Account Name");
            dataGridView1.Columns.Add("type", "Type");
            dataGridView1.Columns.Add("toAddress", "To Address");
            dataGridView1.Columns.Add("amount", "Amount");
            dataGridView1.Columns.Add("fee", "Fee");
            dataGridView1.Columns.Add("blockIndex", "Block Index");
            dataGridView1.Columns.Add("coinType", "Coin Type");
            dataGridView1.Columns.Add("confirmedInBlock", "Confirmed In Block");
            dataGridView1.Columns.Add("timestamp", "Timestamp");
            
            dataGridView1.Columns[0].DataPropertyName = "accountName";
            dataGridView1.Columns[1].DataPropertyName = "type";

            dataGridView1.Columns[2].DataPropertyName = "toAddress";
            dataGridView1.Columns[2].Width = 250;

            dataGridView1.Columns[3].DataPropertyName = "amount";
            dataGridView1.Columns[4].DataPropertyName = "fee";
            dataGridView1.Columns[5].DataPropertyName = "blockIndex";
            dataGridView1.Columns[6].DataPropertyName = "coinType";
            dataGridView1.Columns[7].DataPropertyName = "confirmedInBlock";
            dataGridView1.Columns[8].DataPropertyName = "timestamp";
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            // Pull the wallet history
            try
            {
                historyViewModels.Clear();

                string url = ConfigurationManager.AppSettings.Get("StratisNodeBaseUrl");
                IWalletServiceProxy walletServiceProxy = new WalletServiceProxy(url);
                WalletHistoryRoot walletHistoryRoot = walletServiceProxy.GetWalletHistory(txtWalletName.Text.Trim());
                
                if (walletHistoryRoot != null)
                {
                    foreach(var history in walletHistoryRoot.history)
                    {
                        foreach(var transactionHistory in history.transactionsHistory)
                        {
                            historyViewModels.Add(new HistoryViewModel
                            {
                                accountName = history.accountName,
                                amount = transactionHistory.amount,
                                blockIndex = transactionHistory.blockIndex,
                                coinType = history.coinType,
                                confirmedInBlock = transactionHistory.confirmedInBlock,
                                fee = transactionHistory.fee,
                                id = transactionHistory.id,
                                payments = transactionHistory.payments,
                                timestamp = transactionHistory.timestamp,
                                toAddress = transactionHistory.toAddress,
                                type = transactionHistory.type
                            });
                        }
                    }

                    ComboBoxSelectedIndexChanged();
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
            dataGridView1.DataSource = null;
        }

        private void cmbPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxSelectedIndexChanged();
        }

        /// <summary>
        /// Handle the ComboBox SelectedIndex Changed
        /// </summary>
        private void ComboBoxSelectedIndexChanged()
        {
            if (cmbPaymentType.SelectedIndex >= 0 &&
                           cmbPaymentType.SelectedItem != null)
            {
                switch (cmbPaymentType.Text)
                {
                    case "all":
                        if (historyViewModels.Count > 0)
                            dataGridView1.DataSource = historyViewModels;
                        break;
                    case "send":
                        var filteredResult1 = historyViewModels.FindAll(h => h.type == cmbPaymentType.Text);
                        dataGridView1.DataSource = filteredResult1;
                        break;
                    case "received":
                        var filteredResult2 = historyViewModels.FindAll(h => h.type == cmbPaymentType.Text);
                        dataGridView1.DataSource = filteredResult2;
                        break;
                    default:
                        dataGridView1.DataSource = historyViewModels;
                        break;
                }
            }
        }
    }
}
