using System;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using SaleDeedRegistry.Lib.Actors;
using SaleDeedRegistry.Lib.Client;
using SaleDeedRegistry.Desktop.Constants;
using System.Collections.Generic;

namespace SaleDeedRegistry.Desktop.SaleDeed
{
    public partial class FrmSaleDeedRegistry : Form
    {
        private const string PaidApplicationTransferFee = "Paid Application Transfer Fee";
        private const string InProgressState = "In-Progress";
        private const string StateTheReviewProcess = "Started Review Process";
        private const string CompleteTheReviewProcess = "Completed Review Process";
        private const string TransferOwnershipComplete = "Transfer Ownership Complete";
        private const string ApplicationRejected = "Application Rejected";

        private readonly Payee payee;
        private PropertyBuyer propertyBuyer;
        private PropertySeller propertySeller;
        private readonly StringBuilder responseStringBuilder;

        private Supervisor supervisor;
        private ReceiptResponse receiptResponse;
        private List<Button> stateButtons;

        public FrmSaleDeedRegistry()
        {
            InitializeComponent();
            payee = new Payee();
            responseStringBuilder = new StringBuilder();
            stateButtons = new List<Button>
            {
                btnInitApplication,
                btnStartReviewProcess,
                btnCompleteReviewProcess,
                btnPayApplicationTransferFee,
                btnTransferOwnership,
                btnRejectApplication
            };
        }

        private void FrmSaleDeedRegistry_Load(object sender, System.EventArgs e)
        {
            btnStartReviewProcess.Enabled = false;
            btnCompleteReviewProcess.Enabled = false;
            btnPayApplicationTransferFee.Enabled = false;
            btnTransferOwnership.Enabled = false;
            btnRejectApplication.Enabled = false;
            btnReApply.Enabled = false;
            lblState.Text = "";
        }

        private async void btnInitApplication_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAssetID.Text))
            {
                MessageBox.Show("Please specify the AssetId", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAssetID.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtBuyerAddress.Text))
            {
                MessageBox.Show("Please specify the Buyer Address", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBuyerAddress.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtSellerAddress.Text))
            {
                MessageBox.Show("Please specify the Seller Address", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSellerAddress.Focus();
                return;
            }

            lblState.Visible = true;
            lblState.Text = "Please wait....";

            supervisor = new Supervisor(txtAssetID.Text.Trim(), 
                txtBuyerAddress.Text.Trim(), txtSellerAddress.Text.Trim());
            receiptResponse = await supervisor.InitApplication();

            if (receiptResponse != null && receiptResponse.success)
            {
                OutputResponseInformation(receiptResponse);
                lblState.Text = InProgressState;
                btnInitApplication.Enabled = false;
                btnStartReviewProcess.Enabled = true;
                btnRejectApplication.Enabled = true;
            }
        }

        /// <summary>
        /// Output the ReceiptResponse information on to the RichTextBox
        /// </summary>
        /// <param name="receiptResponse">ReceiptResponse</param>
        private void OutputResponseInformation(ReceiptResponse receiptResponse)
        {
            responseStringBuilder.AppendLine(DateTime.Now.ToString());
            responseStringBuilder.AppendLine(JsonConvert.SerializeObject(receiptResponse));
            richTextBox1.Text = responseStringBuilder.ToString();
            responseStringBuilder.AppendLine("\n");
        }

        private async void btnStartReviewProcess_Click(object sender, System.EventArgs e)
        {
            lblState.Text = "Please wait....";
            receiptResponse = await supervisor.StartTheReviewProcess();
            if (receiptResponse != null && receiptResponse.success)
            {
                OutputResponseInformation(receiptResponse);
                lblState.Text = StateTheReviewProcess;
                btnStartReviewProcess.Enabled = false;
                btnCompleteReviewProcess.Enabled = true;
                btnRejectApplication.Enabled = true;
            }
        }

        private async void btnCompleteReviewProcess_Click(object sender, System.EventArgs e)
        {
            lblState.Text = "Please wait....";
            receiptResponse = await supervisor.CompleteTheReviewProcess();
            if (receiptResponse != null && receiptResponse.success)
            {
                OutputResponseInformation(receiptResponse);
                lblState.Text = CompleteTheReviewProcess;
                btnCompleteReviewProcess.Enabled = false;
                btnPayApplicationTransferFee.Enabled = true;
            }
        }

        private async void btnPayApplicationTransferFee_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAssetID.Text))
            {
                MessageBox.Show("Please specify the AssetId", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAssetID.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtBuyerAddress.Text))
            {
                MessageBox.Show("Please specify the Buyer Address", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBuyerAddress.Focus();
                return;
            }
            
            lblState.Text = "Please wait....";
            propertyBuyer = new PropertyBuyer(txtBuyerAddress.Text.Trim(),
                txtBuyerAddress.Text.Trim());
            receiptResponse = await propertyBuyer.PayTransferFee(payee.GetPayee(),
                txtAssetID.Text.Trim());
            if (receiptResponse != null && receiptResponse.success)
            {
                OutputResponseInformation(receiptResponse);
                lblState.Text = PaidApplicationTransferFee;
                btnPayApplicationTransferFee.Enabled = false;
                btnTransferOwnership.Enabled = true;
            }
        }

        private async void btnTransferOwnership_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSellerAddress.Text))
            {
                MessageBox.Show("Please specify the Seller Address", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSellerAddress.Focus();
                return;
            }

            lblState.Text = "Please wait....";
            propertySeller = new PropertySeller(txtSellerAddress.Text.Trim());

            receiptResponse = await supervisor.TransferOwnership(propertySeller.GetOwnerAddress(),
                propertyBuyer.GetBuyerAddress());
            if (receiptResponse != null && receiptResponse.success)
            {
                OutputResponseInformation(receiptResponse);
                btnTransferOwnership.Enabled = false;
                lblState.Text = TransferOwnershipComplete;
            }
        }

        private async void btnRejectApplication_Click(object sender, EventArgs e)
        {
            lblState.Text = "Please wait....";
            receiptResponse = await supervisor.Reject();
            if (receiptResponse != null && receiptResponse.success)
            {
                OutputResponseInformation(receiptResponse);
                lblState.Text = ApplicationRejected;

                btnStartReviewProcess.Enabled = false;
                btnCompleteReviewProcess.Enabled = false;
                btnPayApplicationTransferFee.Enabled = false;
                btnTransferOwnership.Enabled = false;
                
                btnReApply.Enabled = true;
                btnRejectApplication.Enabled = false;
            }
        }

        private async void btnReApply_Click(object sender, EventArgs e)
        {
            lblState.Text = "Please wait....";
            receiptResponse = await supervisor.ReApply();
            if (receiptResponse != null && receiptResponse.success)
            {
                OutputResponseInformation(receiptResponse);
                lblState.Text = InProgressState;

                btnStartReviewProcess.Enabled = false;
                btnCompleteReviewProcess.Enabled = false;
                btnPayApplicationTransferFee.Enabled = false;
                btnTransferOwnership.Enabled = false;

                btnReApply.Enabled = false;
                btnInitApplication.Enabled = true;
            }
        }

        /// <summary>
        /// Disable State Buttons
        /// </summary>
        private void DisableStateButtons()
        {
            foreach(Button btn in stateButtons)
            {
                btn.Enabled = false;
            }
        }

        /// <summary>
        /// Evaulate the State and then set the State Buttons Accordingly
        /// </summary>
        /// <param name="propertyStateType">PropertyStateType</param>
        private void SetStates(PropertyStateType propertyStateType)
        {
            // First disable all State/Action Buttons
            DisableStateButtons();

            if(propertyStateType == PropertyStateType.NotStarted)
            {
                btnInitApplication.Enabled = true;
            }
            else
            {
                if(propertyStateType == PropertyStateType.Rejected)
                {
                    btnReApply.Enabled = true;
                }
                else
                {
                    stateButtons[(int)propertyStateType].Enabled = true;
                }
            }
        }

        private async void btnGetApplicationState_Click(object sender, EventArgs e)
        {
            btnGetApplicationState.Enabled = false;
            if (string.IsNullOrEmpty(txtAssetID.Text))
            {
                MessageBox.Show("Please specify the AssetId", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAssetID.Focus();
                return;
            }

            supervisor = new Supervisor(txtAssetID.Text.Trim());
            receiptResponse = await supervisor.GetApplicationState();

            if (receiptResponse != null && receiptResponse.success)
            {
                if(receiptResponse.returnValue == "")
                {
                    lblState.Text = "";
                    btnInitApplication.Enabled = true;
                }
                else
                {
                    var stateEnum = Enum.Parse(typeof(PropertyStateType),
                        receiptResponse.returnValue);
                    lblState.Text = stateEnum.ToString();
                    SetStates((PropertyStateType)stateEnum);
                }
            }
            btnGetApplicationState.Enabled = true;
            //
        }
    }
}
