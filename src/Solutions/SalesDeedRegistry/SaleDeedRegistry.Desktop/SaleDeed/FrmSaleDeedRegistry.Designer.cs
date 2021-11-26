namespace SaleDeedRegistry.Desktop.SaleDeed
{
    partial class FrmSaleDeedRegistry
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
            this.components = new System.ComponentModel.Container();
            this.btnInitApplication = new System.Windows.Forms.Button();
            this.btnStartReviewProcess = new System.Windows.Forms.Button();
            this.btnCompleteReviewProcess = new System.Windows.Forms.Button();
            this.btnPayApplicationTransferFee = new System.Windows.Forms.Button();
            this.btnTransferOwnership = new System.Windows.Forms.Button();
            this.txtAssetID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBuyerAddress = new System.Windows.Forms.TextBox();
            this.txtSellerAddress = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnRejectApplication = new System.Windows.Forms.Button();
            this.btnReApply = new System.Windows.Forms.Button();
            this.lblState = new System.Windows.Forms.Label();
            this.btnGetApplicationState = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btnInitApplication
            // 
            this.btnInitApplication.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInitApplication.Location = new System.Drawing.Point(229, 209);
            this.btnInitApplication.Name = "btnInitApplication";
            this.btnInitApplication.Size = new System.Drawing.Size(411, 99);
            this.btnInitApplication.TabIndex = 0;
            this.btnInitApplication.Text = "Init Application";
            this.btnInitApplication.UseVisualStyleBackColor = true;
            this.btnInitApplication.Click += new System.EventHandler(this.btnInitApplication_Click);
            // 
            // btnStartReviewProcess
            // 
            this.btnStartReviewProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartReviewProcess.Location = new System.Drawing.Point(229, 341);
            this.btnStartReviewProcess.Name = "btnStartReviewProcess";
            this.btnStartReviewProcess.Size = new System.Drawing.Size(411, 99);
            this.btnStartReviewProcess.TabIndex = 1;
            this.btnStartReviewProcess.Text = "Start Review Process";
            this.btnStartReviewProcess.UseVisualStyleBackColor = true;
            this.btnStartReviewProcess.Click += new System.EventHandler(this.btnStartReviewProcess_Click);
            // 
            // btnCompleteReviewProcess
            // 
            this.btnCompleteReviewProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompleteReviewProcess.Location = new System.Drawing.Point(229, 474);
            this.btnCompleteReviewProcess.Name = "btnCompleteReviewProcess";
            this.btnCompleteReviewProcess.Size = new System.Drawing.Size(411, 99);
            this.btnCompleteReviewProcess.TabIndex = 2;
            this.btnCompleteReviewProcess.Text = "Complete Review Process";
            this.btnCompleteReviewProcess.UseVisualStyleBackColor = true;
            this.btnCompleteReviewProcess.Click += new System.EventHandler(this.btnCompleteReviewProcess_Click);
            // 
            // btnPayApplicationTransferFee
            // 
            this.btnPayApplicationTransferFee.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPayApplicationTransferFee.Location = new System.Drawing.Point(229, 604);
            this.btnPayApplicationTransferFee.Name = "btnPayApplicationTransferFee";
            this.btnPayApplicationTransferFee.Size = new System.Drawing.Size(411, 99);
            this.btnPayApplicationTransferFee.TabIndex = 4;
            this.btnPayApplicationTransferFee.Text = "Pay Application Transfer Fee";
            this.btnPayApplicationTransferFee.UseVisualStyleBackColor = true;
            this.btnPayApplicationTransferFee.Click += new System.EventHandler(this.btnPayApplicationTransferFee_Click);
            // 
            // btnTransferOwnership
            // 
            this.btnTransferOwnership.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTransferOwnership.Location = new System.Drawing.Point(229, 734);
            this.btnTransferOwnership.Name = "btnTransferOwnership";
            this.btnTransferOwnership.Size = new System.Drawing.Size(411, 99);
            this.btnTransferOwnership.TabIndex = 5;
            this.btnTransferOwnership.Text = "Transfer Ownership";
            this.btnTransferOwnership.UseVisualStyleBackColor = true;
            this.btnTransferOwnership.Click += new System.EventHandler(this.btnTransferOwnership_Click);
            // 
            // txtAssetID
            // 
            this.txtAssetID.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAssetID.Location = new System.Drawing.Point(231, 23);
            this.txtAssetID.Name = "txtAssetID";
            this.txtAssetID.Size = new System.Drawing.Size(411, 38);
            this.txtAssetID.TabIndex = 6;
            this.toolTip1.SetToolTip(this.txtAssetID, "The Unique Property Asset Id");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 32);
            this.label2.TabIndex = 7;
            this.label2.Text = "Asset Id : ";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(671, 85);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(732, 618);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Teal;
            this.label3.Location = new System.Drawing.Point(86, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 91);
            this.label3.TabIndex = 10;
            this.label3.Text = "1.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Teal;
            this.label4.Location = new System.Drawing.Point(86, 341);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 91);
            this.label4.TabIndex = 11;
            this.label4.Text = "2.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Teal;
            this.label5.Location = new System.Drawing.Point(86, 474);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 91);
            this.label5.TabIndex = 12;
            this.label5.Text = "3.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Teal;
            this.label6.Location = new System.Drawing.Point(86, 604);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 91);
            this.label6.TabIndex = 13;
            this.label6.Text = "4.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Teal;
            this.label7.Location = new System.Drawing.Point(86, 734);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 91);
            this.label7.TabIndex = 14;
            this.label7.Text = "5.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 91);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(208, 32);
            this.label8.TabIndex = 15;
            this.label8.Text = "Buyer Address:";
            // 
            // txtBuyerAddress
            // 
            this.txtBuyerAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuyerAddress.Location = new System.Drawing.Point(231, 85);
            this.txtBuyerAddress.Name = "txtBuyerAddress";
            this.txtBuyerAddress.Size = new System.Drawing.Size(411, 38);
            this.txtBuyerAddress.TabIndex = 16;
            this.toolTip1.SetToolTip(this.txtBuyerAddress, "Please key in the Buyer Wallet Address");
            // 
            // txtSellerAddress
            // 
            this.txtSellerAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSellerAddress.Location = new System.Drawing.Point(231, 148);
            this.txtSellerAddress.Name = "txtSellerAddress";
            this.txtSellerAddress.Size = new System.Drawing.Size(411, 38);
            this.txtSellerAddress.TabIndex = 18;
            this.toolTip1.SetToolTip(this.txtSellerAddress, "Please key in the Owner or Seller Wallet Address");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(12, 154);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(208, 32);
            this.label9.TabIndex = 17;
            this.label9.Text = "Seller Address:";
            // 
            // btnRejectApplication
            // 
            this.btnRejectApplication.BackColor = System.Drawing.Color.Red;
            this.btnRejectApplication.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRejectApplication.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnRejectApplication.Location = new System.Drawing.Point(671, 734);
            this.btnRejectApplication.Name = "btnRejectApplication";
            this.btnRejectApplication.Size = new System.Drawing.Size(351, 99);
            this.btnRejectApplication.TabIndex = 19;
            this.btnRejectApplication.Text = "Reject";
            this.btnRejectApplication.UseVisualStyleBackColor = false;
            this.btnRejectApplication.Click += new System.EventHandler(this.btnRejectApplication_Click);
            // 
            // btnReApply
            // 
            this.btnReApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnReApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReApply.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnReApply.Location = new System.Drawing.Point(1052, 734);
            this.btnReApply.Name = "btnReApply";
            this.btnReApply.Size = new System.Drawing.Size(351, 99);
            this.btnReApply.TabIndex = 20;
            this.btnReApply.Text = "Re-Apply";
            this.btnReApply.UseVisualStyleBackColor = false;
            this.btnReApply.Click += new System.EventHandler(this.btnReApply_Click);
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.BackColor = System.Drawing.Color.Transparent;
            this.lblState.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblState.ForeColor = System.Drawing.Color.Red;
            this.lblState.Location = new System.Drawing.Point(908, 36);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(104, 32);
            this.lblState.TabIndex = 21;
            this.lblState.Text = "State : ";
            // 
            // btnGetApplicationState
            // 
            this.btnGetApplicationState.BackColor = System.Drawing.Color.LightGreen;
            this.btnGetApplicationState.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetApplicationState.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnGetApplicationState.Location = new System.Drawing.Point(671, 23);
            this.btnGetApplicationState.Name = "btnGetApplicationState";
            this.btnGetApplicationState.Size = new System.Drawing.Size(219, 56);
            this.btnGetApplicationState.TabIndex = 22;
            this.btnGetApplicationState.Text = "Get State";
            this.toolTip1.SetToolTip(this.btnGetApplicationState, "Gets the Application State");
            this.btnGetApplicationState.UseVisualStyleBackColor = false;
            this.btnGetApplicationState.Click += new System.EventHandler(this.btnGetApplicationState_Click);
            // 
            // FrmSaleDeedRegistry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SaleDeedRegistry.Desktop.Properties.Resources.light_blue_background;
            this.ClientSize = new System.Drawing.Size(1415, 844);
            this.Controls.Add(this.btnGetApplicationState);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.btnReApply);
            this.Controls.Add(this.btnRejectApplication);
            this.Controls.Add(this.txtSellerAddress);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtBuyerAddress);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAssetID);
            this.Controls.Add(this.btnTransferOwnership);
            this.Controls.Add(this.btnPayApplicationTransferFee);
            this.Controls.Add(this.btnCompleteReviewProcess);
            this.Controls.Add(this.btnStartReviewProcess);
            this.Controls.Add(this.btnInitApplication);
            this.MaximizeBox = false;
            this.Name = "FrmSaleDeedRegistry";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Deed Registry";
            this.Load += new System.EventHandler(this.FrmSaleDeedRegistry_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInitApplication;
        private System.Windows.Forms.Button btnStartReviewProcess;
        private System.Windows.Forms.Button btnCompleteReviewProcess;
        private System.Windows.Forms.Button btnPayApplicationTransferFee;
        private System.Windows.Forms.Button btnTransferOwnership;
        private System.Windows.Forms.TextBox txtAssetID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBuyerAddress;
        private System.Windows.Forms.TextBox txtSellerAddress;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnRejectApplication;
        private System.Windows.Forms.Button btnReApply;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Button btnGetApplicationState;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}