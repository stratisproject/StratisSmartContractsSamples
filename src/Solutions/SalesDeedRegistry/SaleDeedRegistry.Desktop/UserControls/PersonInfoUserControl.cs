using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SaleDeedRegistry.Lib.Entities;

namespace SaleDeedRegistry.Desktop.UserControls
{
    public partial class PersonInfoUserControl : UserControl, IUserControl
    {
        public PersonInfoUserControl()
        {
            InitializeComponent();
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
        }

        public void LoadPerson(PersonInfo personInfo)
        {
            txtAaddhar.Text = personInfo.Aaddhar;
            txtFirstName.Text = personInfo.FirstName;
            txtLastName.Text = personInfo.LastName;
            txtMiddleName.Text = personInfo.MiddleName;
            txtPAN.Text = personInfo.PAN;
            txtWalletAddress.Text = personInfo.WalletAddress;
            signatureBox.Image = GetSignature(personInfo.Signature);
        }

        /// <summary>
        /// Build the Signature from the Base64 Encoded Signature
        /// </summary>
        /// <param name="signature">Base64Encoded Signature</param>
        /// <returns>Image</returns>
        private Image GetSignature(string signature)
        {
            if (string.IsNullOrEmpty(signature))
                return null;

            Image image;
            byte[] bytes = Convert.FromBase64String(signature);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            return image;
        }

        private void PersonInfoUserControl_Load(object sender, EventArgs e)
        {
            cmbGender.SelectedIndex = 0;
            cmbGender.Text = "Male";
        }

        public string FirstName
        {
            get
            {
                return txtFirstName.Text;
            }
        }

        public string LastName
        {
            get
            {
                return txtLastName.Text;
            }
        }

        public string MiddleName
        {
            get
            {
                return txtMiddleName.Text;
            }
        }

        public string PAN
        {
            get
            {
                return txtPAN.Text;
            }
        }

        public string Aaddhar
        {
            get
            {
                return txtAaddhar.Text;
            }
        }

        public string Gender
        {
            get
            {
                return cmbGender.Text;
            }
        }

        public string WalletAddess
        {
            get
            {
                return txtWalletAddress.Text;
            }
        }

        public string Base64EncodedSignature
        {
            get
            {
                if (signatureBox.Image == null)
                    return string.Empty;

                using (Image image = signatureBox.Image)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, image.RawFormat);
                        byte[] imageBytes = memoryStream.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
        }


        public void Clear()
        {
            txtAaddhar.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPAN.Text = "";
            cmbGender.SelectedIndex = 0;
            cmbGender.Text = "Male";
            txtFirstName.Focus();
        }

        /// <summary>
        /// Custom Validate the fields and throw error message
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(txtFirstName.Text.Trim()))
            {
                ShowErrorMessage("Please specify the First Name");
                txtFirstName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtLastName.Text.Trim()))
            {
                ShowErrorMessage("Please specify the Last Name");
                txtLastName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(cmbGender.Text.Trim()))
            {
                ShowErrorMessage("Please select Gender");
                cmbGender.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtPAN.Text.Trim()))
            {
                ShowErrorMessage("Please specify the PAN Number");
                txtPAN.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtAaddhar.Text.Trim()))
            {
                ShowErrorMessage("Please specify the Aaddhar Number");
                txtAaddhar.Focus();
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
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog1.FileName;
                signatureBox.Image = Image.FromFile(selectedFile);
            }
        }
    }
}
