namespace SaleDeedRegistry.Wallet
{
    partial class FrmWalletInfo
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
            this.txtWalletName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtConnectedNodes = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtChainSynced = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtChainTip = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLastBlockSync = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtIsDecrypted = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCreationTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNetwork = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnLoadWalletInfo = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtWalletName
            // 
            this.txtWalletName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWalletName.Location = new System.Drawing.Point(300, 24);
            this.txtWalletName.Name = "txtWalletName";
            this.txtWalletName.Size = new System.Drawing.Size(318, 34);
            this.txtWalletName.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label1.Location = new System.Drawing.Point(36, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 29);
            this.label1.TabIndex = 5;
            this.label1.Text = "Wallet Name";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtConnectedNodes);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.txtChainSynced);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtChainTip);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtLastBlockSync);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtIsDecrypted);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtCreationTime);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtNetwork);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtFilePath);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(17, 83);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1029, 542);
            this.panel1.TabIndex = 8;
            // 
            // txtConnectedNodes
            // 
            this.txtConnectedNodes.BackColor = System.Drawing.Color.White;
            this.txtConnectedNodes.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConnectedNodes.Location = new System.Drawing.Point(283, 483);
            this.txtConnectedNodes.Name = "txtConnectedNodes";
            this.txtConnectedNodes.ReadOnly = true;
            this.txtConnectedNodes.Size = new System.Drawing.Size(291, 34);
            this.txtConnectedNodes.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label9.Location = new System.Drawing.Point(19, 483);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(208, 29);
            this.label9.TabIndex = 22;
            this.label9.Text = "Connected Nodes";
            // 
            // txtChainSynced
            // 
            this.txtChainSynced.BackColor = System.Drawing.Color.White;
            this.txtChainSynced.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChainSynced.Location = new System.Drawing.Point(283, 418);
            this.txtChainSynced.Name = "txtChainSynced";
            this.txtChainSynced.ReadOnly = true;
            this.txtChainSynced.Size = new System.Drawing.Size(291, 34);
            this.txtChainSynced.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label8.Location = new System.Drawing.Point(19, 418);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(161, 29);
            this.label8.TabIndex = 20;
            this.label8.Text = "Chain Synced";
            // 
            // txtChainTip
            // 
            this.txtChainTip.BackColor = System.Drawing.Color.White;
            this.txtChainTip.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChainTip.Location = new System.Drawing.Point(283, 354);
            this.txtChainTip.Name = "txtChainTip";
            this.txtChainTip.ReadOnly = true;
            this.txtChainTip.Size = new System.Drawing.Size(291, 34);
            this.txtChainTip.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label7.Location = new System.Drawing.Point(19, 354);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 29);
            this.label7.TabIndex = 18;
            this.label7.Text = "Chain Tip";
            // 
            // txtLastBlockSync
            // 
            this.txtLastBlockSync.BackColor = System.Drawing.Color.White;
            this.txtLastBlockSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastBlockSync.Location = new System.Drawing.Point(283, 287);
            this.txtLastBlockSync.Name = "txtLastBlockSync";
            this.txtLastBlockSync.ReadOnly = true;
            this.txtLastBlockSync.Size = new System.Drawing.Size(291, 34);
            this.txtLastBlockSync.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label6.Location = new System.Drawing.Point(19, 287);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(181, 29);
            this.label6.TabIndex = 16;
            this.label6.Text = "Last Block Sync";
            // 
            // txtIsDecrypted
            // 
            this.txtIsDecrypted.BackColor = System.Drawing.Color.White;
            this.txtIsDecrypted.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIsDecrypted.Location = new System.Drawing.Point(283, 224);
            this.txtIsDecrypted.Name = "txtIsDecrypted";
            this.txtIsDecrypted.ReadOnly = true;
            this.txtIsDecrypted.Size = new System.Drawing.Size(291, 34);
            this.txtIsDecrypted.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label5.Location = new System.Drawing.Point(19, 224);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 29);
            this.label5.TabIndex = 14;
            this.label5.Text = "Is Decrypted";
            // 
            // txtCreationTime
            // 
            this.txtCreationTime.BackColor = System.Drawing.Color.White;
            this.txtCreationTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCreationTime.Location = new System.Drawing.Point(283, 155);
            this.txtCreationTime.Name = "txtCreationTime";
            this.txtCreationTime.ReadOnly = true;
            this.txtCreationTime.Size = new System.Drawing.Size(291, 34);
            this.txtCreationTime.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label4.Location = new System.Drawing.Point(19, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 29);
            this.label4.TabIndex = 12;
            this.label4.Text = "Creation Time";
            // 
            // txtNetwork
            // 
            this.txtNetwork.BackColor = System.Drawing.Color.White;
            this.txtNetwork.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNetwork.Location = new System.Drawing.Point(283, 91);
            this.txtNetwork.Name = "txtNetwork";
            this.txtNetwork.ReadOnly = true;
            this.txtNetwork.Size = new System.Drawing.Size(291, 34);
            this.txtNetwork.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label3.Location = new System.Drawing.Point(19, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 29);
            this.label3.TabIndex = 10;
            this.label3.Text = "Network";
            // 
            // txtFilePath
            // 
            this.txtFilePath.BackColor = System.Drawing.Color.White;
            this.txtFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilePath.Location = new System.Drawing.Point(283, 24);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(731, 34);
            this.txtFilePath.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label2.Location = new System.Drawing.Point(19, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 29);
            this.label2.TabIndex = 0;
            this.label2.Text = "File Path";
            // 
            // btnClear
            // 
            this.btnClear.BackgroundImage = global::SaleDeedRegistry.Wallet.Properties.Resources.button_blue;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnClear.Location = new System.Drawing.Point(889, 640);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(157, 62);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnLoadWalletInfo
            // 
            this.btnLoadWalletInfo.BackgroundImage = global::SaleDeedRegistry.Wallet.Properties.Resources.button_blue;
            this.btnLoadWalletInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadWalletInfo.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnLoadWalletInfo.Location = new System.Drawing.Point(694, 640);
            this.btnLoadWalletInfo.Name = "btnLoadWalletInfo";
            this.btnLoadWalletInfo.Size = new System.Drawing.Size(157, 62);
            this.btnLoadWalletInfo.TabIndex = 7;
            this.btnLoadWalletInfo.Text = "Load";
            this.btnLoadWalletInfo.UseVisualStyleBackColor = true;
            this.btnLoadWalletInfo.Click += new System.EventHandler(this.btnLoadWalletInfo_Click);
            // 
            // FrmWalletInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(1058, 710);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnLoadWalletInfo);
            this.Controls.Add(this.txtWalletName);
            this.Controls.Add(this.label1);
            this.Name = "FrmWalletInfo";
            this.Text = "Wallet Info";
            this.Load += new System.EventHandler(this.FrmWalletInfo_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtWalletName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoadWalletInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtIsDecrypted;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCreationTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNetwork;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLastBlockSync;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtChainSynced;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtChainTip;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtConnectedNodes;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnClear;
    }
}