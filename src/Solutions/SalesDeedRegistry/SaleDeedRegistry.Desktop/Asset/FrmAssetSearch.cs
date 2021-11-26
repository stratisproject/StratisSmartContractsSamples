using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using SaleDeedRegistry.Desktop.Constants;
using SaleDeedRegistry.Desktop.Repository;

namespace SaleDeedRegistry.Desktop.Search
{
    public partial class FrmAssetSearch : Form
    {
        private readonly string dbPath = "";
        private List<AssetViewModel> assetCollection;
        private readonly AssetManagementRepository assetManagementRepository;
        private readonly PersonRespository personRespository;

        public FrmAssetSearch()
        {
            InitializeComponent();

            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dbPath = string.Format($"{exePath}\\{DBConstant.SqlLiteDBFileName}");
            assetManagementRepository = new AssetManagementRepository(dbPath);
            personRespository = new PersonRespository(dbPath);

            this.WindowState = FormWindowState.Maximized;
            assetCollection = new List<AssetViewModel>();
            InitDataGridView();
        }

        private void InitDataGridView()
        {
            assetsGridView.Columns.Add("Id", "Id");
            assetsGridView.Columns.Add("FirstName", "FirstName");
            assetsGridView.Columns.Add("LastName", "LastName");
            assetsGridView.Columns.Add("PAN", "PAN");
            assetsGridView.Columns.Add("Aaddhar", "Aaddhar");
            assetsGridView.Columns.Add("PropertyNumber", "PropertyNumber");
            assetsGridView.Columns.Add("MuncipleNumber", "MuncipleNumber");
            assetsGridView.Columns.Add("AssetId", "AssetId");
            assetsGridView.Columns.Add("WalletAddress", "WalletAddress");

            int index = 0;

            assetsGridView.Columns[index++].DataPropertyName = "Id";
            assetsGridView.Columns[index++].DataPropertyName = "FirstName";
            assetsGridView.Columns[index++].DataPropertyName = "LastName";
            assetsGridView.Columns[index++].DataPropertyName = "PAN";
            assetsGridView.Columns[index++].DataPropertyName = "Aaddhar";
            assetsGridView.Columns[index++].DataPropertyName = "PropertyNumber";
            assetsGridView.Columns[index++].DataPropertyName = "MuncipleNumber";
            assetsGridView.Columns[index++].DataPropertyName = "AssetId";
            assetsGridView.Columns[index++].DataPropertyName = "WalletAddress";

            assetsGridView.Columns["PAN"].Width = 200;
            assetsGridView.Columns["Aaddhar"].Width = 200;
            assetsGridView.Columns["PropertyNumber"].Width = 200;
            assetsGridView.Columns["AssetId"].Width = 200;
            assetsGridView.Columns["MuncipleNumber"].Width = 200;
            assetsGridView.Columns["WalletAddress"].Width = 200;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var filteredAssets = assetManagementRepository.Search(txtAssetID.Text.Trim(),
                txtPropertyNumber.Text.Trim());
            assetsGridView.DataSource = filteredAssets;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtAssetID.Text = "";
            txtPropertyNumber.Text = "";
            txtAssetID.Focus();
            assetsGridView.DataSource = null;
        }

        private void LoadAll()
        {
            var allRegistredAssets = assetManagementRepository.QueryAllAssets();
            foreach(var asset in allRegistredAssets)
            {
                var assetViewModel = new AssetViewModel
                {
                    Id = asset.Id,
                    AssetId = asset.AssetId,
                    PropertyNumber = asset.PropertyNumber,
                    MuncipleNumber = asset.MuncipleNumber
                };
                if(asset.PersonId > 0)
                {
                    var personInfo = personRespository.Query(asset.PersonId);
                    if (personInfo != null)
                    {
                        assetViewModel.FirstName = personInfo.FirstName;
                        assetViewModel.LastName = personInfo.LastName;
                        assetViewModel.PAN = personInfo.PAN;
                        assetViewModel.Aaddhar = personInfo.Aaddhar;
                        assetViewModel.WalletAddress = personInfo.WalletAddress;
                    }
                }
                assetCollection.Add(assetViewModel);
            }
            assetsGridView.DataSource = assetCollection;
        }

        private void FrmAssetSearch_Load(object sender, EventArgs e)
        {
            LoadAll();
        }
    }
}
