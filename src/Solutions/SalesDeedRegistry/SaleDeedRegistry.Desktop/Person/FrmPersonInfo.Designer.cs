namespace SaleDeedRegistry.Desktop.Person
{
    partial class FrmPersonInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.locationInfoUserControl1 = new SaleDeedRegistry.Desktop.UserControls.LocationInfoUserControl();
            this.personInfoUserControl1 = new SaleDeedRegistry.Desktop.UserControls.PersonInfoUserControl();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // locationInfoUserControl1
            // 
            this.locationInfoUserControl1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.locationInfoUserControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.locationInfoUserControl1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.locationInfoUserControl1.Location = new System.Drawing.Point(701, 12);
            this.locationInfoUserControl1.Name = "locationInfoUserControl1";
            this.locationInfoUserControl1.Size = new System.Drawing.Size(656, 533);
            this.locationInfoUserControl1.TabIndex = 2;
            // 
            // personInfoUserControl1
            // 
            this.personInfoUserControl1.BackColor = System.Drawing.Color.Transparent;
            this.personInfoUserControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.personInfoUserControl1.Location = new System.Drawing.Point(13, 13);
            this.personInfoUserControl1.Name = "personInfoUserControl1";
            this.personInfoUserControl1.Size = new System.Drawing.Size(668, 605);
            this.personInfoUserControl1.TabIndex = 1;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(1179, 569);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(178, 49);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(953, 569);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(192, 49);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmPersonInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SaleDeedRegistry.Desktop.Properties.Resources.light_blue_background;
            this.ClientSize = new System.Drawing.Size(1370, 632);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.personInfoUserControl1);
            this.Controls.Add(this.locationInfoUserControl1);
            this.MaximizeBox = false;
            this.Name = "FrmPersonInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create/Update Person Info";
            this.Load += new System.EventHandler(this.FrmPersonInfo_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private UserControls.LocationInfoUserControl locationInfoUserControl1;
        private UserControls.PersonInfoUserControl personInfoUserControl1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
    }
}