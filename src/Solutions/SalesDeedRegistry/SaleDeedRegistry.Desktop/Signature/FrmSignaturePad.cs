using System.IO;
using System.Windows.Forms;

namespace SaleDeedRegistry.Desktop.Signature
{
    public partial class FrmSignaturePad : Form
    {
        public FrmSignaturePad()
        {
            InitializeComponent();
            this.MaximizeBox = false;
        }

        private void FrmSignaturePad_Load(object sender, System.EventArgs e)
        {
            string html = File.ReadAllText("SignaturePad.html");
            webBrowser1.DocumentText = html;
        }
    }
}
