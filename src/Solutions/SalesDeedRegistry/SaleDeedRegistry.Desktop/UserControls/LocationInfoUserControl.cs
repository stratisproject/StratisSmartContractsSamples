using System.Windows.Forms;
using SaleDeedRegistry.Lib.Entities;

namespace SaleDeedRegistry.Desktop.UserControls
{
    public partial class LocationInfoUserControl : UserControl
    {
        public LocationInfoUserControl()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            txtAddressLine1.Text = "";
            txtAddressLine2.Text = "";
            txtCity.Text = "";
            txtCountry.Text = "";
            txtLatitude.Text = "";
            txtLongitude.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
        }

        public string AddressLine1
        {
            get
            {
                return txtAddressLine1.Text;
            }
        }

        public string AddressLine2
        {
            get
            {
                return txtAddressLine2.Text;
            }
        }

        public string City
        {
            get
            {
                return txtCity.Text;
            }
        }

        public string State
        {
            get
            {
                return txtState.Text;
            }
        }

        public string ZipCode
        {
            get
            {
                return txtZipCode.Text;
            }
        }

        public string Country
        {
            get
            {
                return txtCountry.Text;
            }
        }

        public string Latitude
        {
            get
            {
                return txtLatitude.Text;
            }
        }

        public string Longitude
        {
            get
            {
                return txtLongitude.Text;
            }
        }

        public new void Load(LocationInfo locationInfo)
        {
            txtAddressLine1.Text = locationInfo.Address1;
            txtAddressLine2.Text = locationInfo.Address2;
            txtCity.Text = locationInfo.City;
            txtCountry.Text = locationInfo.Country;
            txtLatitude.Text = locationInfo.Latitude;
            txtLongitude.Text = locationInfo.Longitude;
            txtState.Text = locationInfo.State;
            txtZipCode.Text = locationInfo.ZipCode;
        }

        /// <summary>
        /// Custom Validate the fields and throw error message
        /// </summary>
        public new bool Validate()
        {
            if (string.IsNullOrEmpty(txtAddressLine1.Text.Trim()))
            {
                ShowErrorMessage("Please specify the AddressLine1");
                txtAddressLine1.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtAddressLine2.Text.Trim()))
            {
                ShowErrorMessage("Please specify the AddressLine2");
                txtAddressLine2.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtCity.Text.Trim()))
            {
                ShowErrorMessage("Please specify the City");
                txtCity.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtState.Text.Trim()))
            {
                ShowErrorMessage("Please specify the State");
                txtState.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtZipCode.Text.Trim()))
            {
                ShowErrorMessage("Please specify the ZipCode");
                txtZipCode.Focus();
                return false;
            }
            return true;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
