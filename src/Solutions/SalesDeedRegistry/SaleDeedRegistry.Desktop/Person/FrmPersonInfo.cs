using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SaleDeedRegistry.Lib.Entities;
using SaleDeedRegistry.Desktop.Constants;
using SaleDeedRegistry.Desktop.Repository;

namespace SaleDeedRegistry.Desktop.Person
{
    public partial class FrmPersonInfo : Form
    {
        private readonly string dbPath = "";
        private readonly PersonRespository personRespository;
        private readonly LocationRespository propertyLocationRespository;

        private PersonInfo PersonToUpdate { get; set; }
        private LocationInfo LocationToUpdate { get; set; }

        public FrmPersonInfo(PersonInfo personInfo, LocationInfo locationInfo)
        {
            this.PersonToUpdate = personInfo;
            this.LocationToUpdate = locationInfo;
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dbPath = string.Format($"{exePath}\\{DBConstant.SqlLiteDBFileName}");

            InitializeComponent();
            personRespository = new PersonRespository(dbPath);
            propertyLocationRespository = new LocationRespository(dbPath);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            personInfoUserControl1.Clear();
            locationInfoUserControl1.Clear();
        }

        private void Insert()
        {
            // Validate
            bool isPersonalInfoValid = personInfoUserControl1.Validate();
            if (!isPersonalInfoValid) return;

            bool isLocationInfoValid = locationInfoUserControl1.Validate();
            if (!isLocationInfoValid) return;

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

            int id = propertyLocationRespository.Insert(locationInfo);
            if (id > 0)
            {
                locationId = propertyLocationRespository.GetLastId();
            }
            else
            {
                MessageBox.Show("Problem in saving the location info. " +
                    "Please try again or contact the admin",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PersonInfo assetPersonInfo = new PersonInfo
            {
                FirstName = personInfoUserControl1.FirstName,
                LastName = personInfoUserControl1.LastName,
                Gender = personInfoUserControl1.Gender,
                MiddleName = personInfoUserControl1.MiddleName,
                Aaddhar = personInfoUserControl1.Aaddhar,
                PAN = personInfoUserControl1.PAN,
                WalletAddress = personInfoUserControl1.WalletAddess,
                LocationId = locationId,
                Signature = personInfoUserControl1.Base64EncodedSignature
            };

            id = personRespository.Insert(assetPersonInfo);

            if (id <= 0)
            {
                MessageBox.Show("Problem in saving the person info. " +
                   "Please try again or contact the admin",
                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Saved Successfully!") == DialogResult.OK)
                this.Close();
        }

        private new void Update()
        {
            // Update the Person Info
            PersonToUpdate.FirstName = personInfoUserControl1.FirstName;
            PersonToUpdate.LastName = personInfoUserControl1.LastName;
            PersonToUpdate.Gender = personInfoUserControl1.Gender;
            PersonToUpdate.MiddleName = personInfoUserControl1.MiddleName;
            PersonToUpdate.Aaddhar = personInfoUserControl1.Aaddhar;
            PersonToUpdate.PAN = personInfoUserControl1.PAN;
            PersonToUpdate.WalletAddress = personInfoUserControl1.WalletAddess;
            PersonToUpdate.Signature = personInfoUserControl1.Base64EncodedSignature;
            personRespository.Update(PersonToUpdate);

            // Update the Location Info
            LocationToUpdate.Address1 = locationInfoUserControl1.AddressLine1;
            LocationToUpdate.Address2 = locationInfoUserControl1.AddressLine2;
            LocationToUpdate.City = locationInfoUserControl1.City;
            LocationToUpdate.State = locationInfoUserControl1.State;
            LocationToUpdate.ZipCode = locationInfoUserControl1.ZipCode;
            LocationToUpdate.Latitude = locationInfoUserControl1.Latitude;
            LocationToUpdate.Longitude = locationInfoUserControl1.Longitude;
            propertyLocationRespository.Update(LocationToUpdate);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (PersonToUpdate == null)
                    Insert();
                else
                    Update();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FrmPersonInfo_Load(object sender, EventArgs e)
        {
            if(PersonToUpdate != null)
                personInfoUserControl1.LoadPerson(PersonToUpdate);

            if(LocationToUpdate != null)
                locationInfoUserControl1.Load(LocationToUpdate);
        }
    }
}
