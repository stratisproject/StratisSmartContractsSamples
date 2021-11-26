using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SaleDeedRegistry.Desktop.Constants;
using SaleDeedRegistry.Desktop.Repository;

namespace SaleDeedRegistry.Desktop.Person
{
    public partial class FrmSearchPersonInfo : Form
    {
        private readonly string dbPath = "";
        private readonly PersonRespository personRespository;
        private readonly LocationRespository propertyLocationRespository;

        public FrmSearchPersonInfo()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dbPath = string.Format($"{exePath}\\{DBConstant.SqlLiteDBFileName}");

            InitializeComponent();
            InitDataGridView();

            this.WindowState = FormWindowState.Maximized;
            personRespository = new PersonRespository(dbPath);
            propertyLocationRespository = new LocationRespository(dbPath);
            personGridView.MultiSelect = false;
            personGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void InitDataGridView()
        {
            personGridView.AutoGenerateColumns = false;

            personGridView.Columns.Add("Id", "Id");
            personGridView.Columns.Add("LocationId", "LocationId");
            personGridView.Columns.Add("FirstName", "FirstName");
            personGridView.Columns.Add("LastName", "LastName");
            personGridView.Columns.Add("PAN", "PAN");
            personGridView.Columns.Add("Aaddhar", "Aaddhar");
            personGridView.Columns.Add("WalletAddress", "WalletAddress");

            int index = 0;

            personGridView.Columns[index++].DataPropertyName = "Id";
            personGridView.Columns[index++].DataPropertyName = "LocationId";
            personGridView.Columns[index++].DataPropertyName = "FirstName";
            personGridView.Columns[index++].DataPropertyName = "LastName";
            personGridView.Columns[index++].DataPropertyName = "PAN";
            personGridView.Columns[index++].DataPropertyName = "Aaddhar";
            personGridView.Columns[index++].DataPropertyName = "WalletAddress";

            personGridView.Columns["Aaddhar"].Width = 200;
            personGridView.Columns["WalletAddress"].Width = 250;
         
            DataGridViewButtonColumn updateButton = new DataGridViewButtonColumn();
            personGridView.Columns.Add(updateButton);
            updateButton.HeaderText = "Update";
            updateButton.Text = "Update";
            updateButton.Name = "btnUpdate";
            updateButton.UseColumnTextForButtonValue = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            var persons = personRespository.Search(new Lib.Entities.PersonInfo
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                PAN = txtPAN.Text.Trim(),
                Aaddhar = txtAaddhar.Text.Trim()
            });
            personGridView.DataSource = persons;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtAaddhar.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPAN.Text = "";
            personGridView.DataSource = null;
        }

        private void personGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Update Button Click
            if (e.ColumnIndex == personGridView.Columns.Count - 1 &&
                personGridView.SelectedRows.Count > 0)
            {
                HandleUpdateButtonClick();
            }
        }

        /// <summary>
        /// Handle the Update Button Click
        /// </summary>
        private void HandleUpdateButtonClick()
        {
            var personId = int.Parse(personGridView.SelectedRows[0].Cells[0].Value.ToString());
            var locationId = int.Parse(personGridView.SelectedRows[0].Cells[1].Value.ToString());

            var person = personRespository.Query(personId);
            var location = propertyLocationRespository.Query(locationId);

            if (person != null && location != null)
            {
                FrmPersonInfo frmPersonInfo = new FrmPersonInfo(person, location);
                frmPersonInfo.FormClosing += FrmPersonInfo_FormClosing;
                frmPersonInfo.ShowDialog();
            }
        }

        private void FrmPersonInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Refresh the Gridview
            Search();
        }
    }
}
