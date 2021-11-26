using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SaleDeedRegistry.Lib.Helpers;
using SaleDeedRegistry.Lib.Entities;
using SaleDeedRegistry.Desktop.Constants;
using SaleDeedRegistry.Desktop.Repository;

namespace SaleDeedRegistry.Desktop.Asset
{
    public partial class FrmCreateAsset : Form
    {
        private readonly string dbPath = "";
        private readonly AssetManagementRepository assetManagementRepository;
        private readonly PersonRespository personRespository;
        private readonly LocationRespository propertyLocationRespository;
       
        public FrmCreateAsset()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dbPath = string.Format($"{exePath}\\{DBConstant.SqlLiteDBFileName}");

            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            personRespository = new PersonRespository(dbPath);
            assetManagementRepository = new AssetManagementRepository(dbPath);
            propertyLocationRespository = new LocationRespository(dbPath);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            assetInfoUserControl1.Clear();
            locationInfoUserControl1.Clear();
            lblAssetId.Text = "";
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if(cmbPropertyOwner.Text == "" || 
                    cmbPropertyOwner.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select the property owner", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cmbPropertyOwner.Focus();
                    return;
                }

                var splittedPersonInfo = cmbPropertyOwner.Text.Split(
                    new char[1] { '-' });

                bool isAssetInfoValid = assetInfoUserControl1.Validate();
                if (!isAssetInfoValid) return;

                bool isLocationInfoValid = locationInfoUserControl1.Validate();
                if (!isLocationInfoValid) return;

                int id = 0;
                int locationId = 0;

                LocationInfo locationInfo = new LocationInfo
                {
                    Address1 = locationInfoUserControl1.AddressLine1,
                    Address2 = locationInfoUserControl1.AddressLine2,
                    City = locationInfoUserControl1.City,
                    State = locationInfoUserControl1.State,
                    ZipCode = locationInfoUserControl1.ZipCode,
                    Country = locationInfoUserControl1.Country,
                    Latitude = locationInfoUserControl1.Latitude,
                    Longitude = locationInfoUserControl1.Longitude
                };
                locationId = propertyLocationRespository.Insert(locationInfo);

                if(locationId <= 0)
                {
                    MessageBox.Show("There was a problem in saving the location info. " +
                        "Please contact Sys Admin or Support.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                AssetInfo assetInfo = new AssetInfo
                {
                    AssetId = UniqueIdHelper.GenerateId(),
                    MarketPrice = long.Parse(assetInfoUserControl1.MarketPrice),
                    PropertyNumber = assetInfoUserControl1.PropertyNumber,
                    MuncipleNumber = assetInfoUserControl1.Munciple,
                    WardNumber = assetInfoUserControl1.WardNumber,
                    PurchacePrice = long.Parse(assetInfoUserControl1.PurchasePrice),
                    PropertyTax = long.Parse(assetInfoUserControl1.PropertyTax),
                    SquareFeet = long.Parse(assetInfoUserControl1.Measurment),
                    ESignature = assetInfoUserControl1.Base64EncodedSignature,
                    PersonId = int.Parse(splittedPersonInfo[0]),
                    LocationId = locationId
                };

                // Persist the Asset Info
                assetManagementRepository.InsertAsset(assetInfo);

                btnCopyAssetId.Enabled = true;
                lblAssetId.Visible = true;
                lblAssetId.Text = string.Format("{0} - {1}", 
                    "AssetId", assetInfo.AssetId);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FrmCreateAsset_Load(object sender, EventArgs e)
        {
            lblAssetId.Visible = false;
            btnCopyAssetId.Enabled = false;
            LoadPersons();
        }

        private void LoadPersons()
        {
            var allPersons = personRespository.QueryAllPersons();
            foreach(var person in allPersons)
            {
                cmbPropertyOwner.Items.Add(string.Format("{0}-{1}-{2}",
                    person.Id, $"{person.FirstName} {person.LastName}",
                    person.Aaddhar));
            }
        }

        private void btnCopyAssetId_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblAssetId.Text.Trim())){
                Clipboard.SetText(lblAssetId.Text.Replace("AssetId -", "").Trim());
            }
        }
    }
}
