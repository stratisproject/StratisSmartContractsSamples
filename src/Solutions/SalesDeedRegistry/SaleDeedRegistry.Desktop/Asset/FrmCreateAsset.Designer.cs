namespace SaleDeedRegistry.Desktop.Asset
{
    partial class FrmCreateAsset
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
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblAssetId = new System.Windows.Forms.Label();
            this.cmbPropertyOwner = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.locationInfoUserControl1 = new SaleDeedRegistry.Desktop.UserControls.LocationInfoUserControl();
            this.assetInfoUserControl1 = new SaleDeedRegistry.Desktop.UserControls.AssetInfoUserControl();
            this.btnCopyAssetId = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreate.Location = new System.Drawing.Point(939, 558);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(192, 61);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(1167, 558);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(192, 61);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblAssetId
            // 
            this.lblAssetId.AutoSize = true;
            this.lblAssetId.BackColor = System.Drawing.Color.Transparent;
            this.lblAssetId.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAssetId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblAssetId.Location = new System.Drawing.Point(704, 630);
            this.lblAssetId.Name = "lblAssetId";
            this.lblAssetId.Size = new System.Drawing.Size(98, 29);
            this.lblAssetId.TabIndex = 6;
            this.lblAssetId.Text = "Asset Id";
            // 
            // cmbPropertyOwner
            // 
            this.cmbPropertyOwner.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPropertyOwner.FormattingEnabled = true;
            this.cmbPropertyOwner.Location = new System.Drawing.Point(233, 13);
            this.cmbPropertyOwner.Name = "cmbPropertyOwner";
            this.cmbPropertyOwner.Size = new System.Drawing.Size(747, 33);
            this.cmbPropertyOwner.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(28, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 29);
            this.label1.TabIndex = 9;
            this.label1.Text = "Property Owner";
            // 
            // locationInfoUserControl1
            // 
            this.locationInfoUserControl1.BackColor = System.Drawing.Color.Transparent;
            this.locationInfoUserControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.locationInfoUserControl1.Location = new System.Drawing.Point(709, 55);
            this.locationInfoUserControl1.Name = "locationInfoUserControl1";
            this.locationInfoUserControl1.Size = new System.Drawing.Size(650, 486);
            this.locationInfoUserControl1.TabIndex = 10;
            // 
            // assetInfoUserControl1
            // 
            this.assetInfoUserControl1.BackColor = System.Drawing.Color.Transparent;
            this.assetInfoUserControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.assetInfoUserControl1.Location = new System.Drawing.Point(13, 55);
            this.assetInfoUserControl1.Name = "assetInfoUserControl1";
            this.assetInfoUserControl1.Size = new System.Drawing.Size(672, 604);
            this.assetInfoUserControl1.TabIndex = 7;
            // 
            // btnCopyAssetId
            // 
            this.btnCopyAssetId.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopyAssetId.Location = new System.Drawing.Point(709, 558);
            this.btnCopyAssetId.Name = "btnCopyAssetId";
            this.btnCopyAssetId.Size = new System.Drawing.Size(192, 61);
            this.btnCopyAssetId.TabIndex = 11;
            this.btnCopyAssetId.Text = "Copy Asset Id";
            this.btnCopyAssetId.UseVisualStyleBackColor = true;
            this.btnCopyAssetId.Click += new System.EventHandler(this.btnCopyAssetId_Click);
            // 
            // FrmCreateAsset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.BackgroundImage = global::SaleDeedRegistry.Desktop.Properties.Resources.light_blue_background;
            this.ClientSize = new System.Drawing.Size(1371, 674);
            this.Controls.Add(this.btnCopyAssetId);
            this.Controls.Add(this.locationInfoUserControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbPropertyOwner);
            this.Controls.Add(this.assetInfoUserControl1);
            this.Controls.Add(this.lblAssetId);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCreate);
            this.Name = "FrmCreateAsset";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Property Asset";
            this.Load += new System.EventHandler(this.FrmCreateAsset_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblAssetId;
        private UserControls.AssetInfoUserControl assetInfoUserControl1;
        private System.Windows.Forms.ComboBox cmbPropertyOwner;
        private System.Windows.Forms.Label label1;
        private UserControls.LocationInfoUserControl locationInfoUserControl1;
        private System.Windows.Forms.Button btnCopyAssetId;
    }
}