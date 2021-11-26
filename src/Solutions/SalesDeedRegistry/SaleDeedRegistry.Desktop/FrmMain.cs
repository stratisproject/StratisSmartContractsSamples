using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ContactManagement;

using SaleDeedRegistry.Desktop.Asset;
using SaleDeedRegistry.Desktop.Search;
using SaleDeedRegistry.Desktop.SaleDeed;
using SaleDeedRegistry.Desktop.Constants;
using SaleDeedRegistry.Desktop.Signature;
using SaleDeedRegistry.Desktop.Person;

namespace SaleDeedRegistry.Desktop
{
    public partial class FrmMain : Form
    {
        string dbPath = "";

        public FrmMain()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dbPath = string.Format($"{exePath}\\{DBConstant.SqlLiteDBFileName}");

            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.IsMdiContainer = true;
        }

        private void createNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCreateAsset frmCreateAsset = new FrmCreateAsset
            {
                MdiParent = this
            };
            frmCreateAsset.Show();
        }

        private void signaturePadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSignaturePad signaturePad = new FrmSignaturePad();
            signaturePad.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAboutBox frmAboutBox = new FrmAboutBox();
            frmAboutBox.ShowDialog();
        }

        private void saleDeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSaleDeedRegistry frmSaleDeedRegistry = new FrmSaleDeedRegistry();
            frmSaleDeedRegistry.ShowDialog();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAssetSearch frmAssetSearch = new FrmAssetSearch
            {
                MdiParent = this
            };
            frmAssetSearch.Show();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPersonInfo frmPerson = new FrmPersonInfo(null, null);
            frmPerson.ShowDialog();
        }

        private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmSearchPersonInfo frmSearchPersonInfo = new FrmSearchPersonInfo
            {
                MdiParent = this
            };
            frmSearchPersonInfo.Show();
        }
    }
}
