using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace SaleDeedRegistry.Desktop.UserControls
{
    public partial class AssetInfoUserControl : UserControl
    {
        public AssetInfoUserControl()
        {
            InitializeComponent();
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
        }

        public void Clear()
        {
            txtMarketPrice.Text = "";
            txtPropertyNumber.Text = "";
            txtPurchacePrice.Text = "";
            txtPropertyTax.Text = "";
            txtWardNumber.Text = "";
            txtMunciple.Text = "";
            signatureBox.Image = null;
            txtMeasurment.Text = "";
        }

        public string MarketPrice
        {
            get
            {
                return txtMarketPrice.Text.Trim();
            }
        }

        public string PropertyNumber
        {
            get
            {
                return txtPropertyNumber.Text.Trim();
            }
        }

        public string WardNumber
        {
            get
            {
                return txtWardNumber.Text.Trim();
            }
        }

        public string Munciple
        {
            get
            {
                return txtMunciple.Text.Trim();
            }
        }

        public string PropertyTax
        {
            get
            {
                return txtPropertyTax.Text.Trim();
            }
        }

        public string PurchasePrice
        {
            get
            {
                return txtPurchacePrice.Text.Trim();
            }
        }

        public string Measurment
        {
            get
            {
                return txtMeasurment.Text.Trim();
            }
        }

        public string Base64EncodedSignature
        {
            get
            {
                if (signatureBox.Image == null)
                    return string.Empty;

                Image image = signatureBox.Image;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, image.RawFormat);
                    byte[] imageBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
        }


        /// <summary>
        /// Custom Validate the fields and throw error message
        /// </summary>
        public new bool Validate()
        {
            if (string.IsNullOrEmpty(txtPropertyNumber.Text.Trim()))
            {
                ShowErrorMessage("Please specify the property number");
                txtPropertyNumber.Focus();
                return false;
            }


            if (string.IsNullOrEmpty(txtWardNumber.Text.Trim()))
            {
                ShowErrorMessage("Please specify the ward number");
                txtWardNumber.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtMunciple.Text.Trim()))
            {
                ShowErrorMessage("Please specify the munciple");
                txtMunciple.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtMarketPrice.Text.Trim()))
            {
                ShowErrorMessage("Please specify the market price");
                txtMarketPrice.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtPurchacePrice.Text.Trim()))
            {
                ShowErrorMessage("Please specify the purchace price");
                txtPurchacePrice.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtMeasurment.Text.Trim()))
            {
                ShowErrorMessage("Please specify the measurment");
                txtMeasurment.Focus();
                return false;
            }

            return true;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void btnBrowseSignature_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog1.FileName;
                signatureBox.Image = Image.FromFile(selectedFile);
            }
        }
    }
}
