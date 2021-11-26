namespace SaleDeedRegistry.Wallet
{
    partial class FrmWalletBalance
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
            this.txtSpendableAmount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtAccountUnConfirmed = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAccountConfirmed = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCoinType = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtAccountHDPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAccountName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGetBalance = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtWalletName
            // 
            this.txtWalletName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWalletName.Location = new System.Drawing.Point(306, 20);
            this.txtWalletName.Name = "txtWalletName";
            this.txtWalletName.Size = new System.Drawing.Size(291, 34);
            this.txtWalletName.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label1.Location = new System.Drawing.Point(42, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 29);
            this.label1.TabIndex = 8;
            this.label1.Text = "Wallet Name";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtSpendableAmount);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.txtAccountUnConfirmed);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtAccountConfirmed);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtCoinType);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtAccountHDPath);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtAccountName);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(28, 80);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(595, 412);
            this.panel1.TabIndex = 11;
            // 
            // txtSpendableAmount
            // 
            this.txtSpendableAmount.BackColor = System.Drawing.Color.White;
            this.txtSpendableAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSpendableAmount.Location = new System.Drawing.Point(278, 344);
            this.txtSpendableAmount.Name = "txtSpendableAmount";
            this.txtSpendableAmount.ReadOnly = true;
            this.txtSpendableAmount.Size = new System.Drawing.Size(291, 34);
            this.txtSpendableAmount.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label9.Location = new System.Drawing.Point(14, 344);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(218, 29);
            this.label9.TabIndex = 22;
            this.label9.Text = "Spendable Amount";
            // 
            // txtAccountUnConfirmed
            // 
            this.txtAccountUnConfirmed.BackColor = System.Drawing.Color.White;
            this.txtAccountUnConfirmed.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountUnConfirmed.Location = new System.Drawing.Point(278, 279);
            this.txtAccountUnConfirmed.Name = "txtAccountUnConfirmed";
            this.txtAccountUnConfirmed.ReadOnly = true;
            this.txtAccountUnConfirmed.Size = new System.Drawing.Size(291, 34);
            this.txtAccountUnConfirmed.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label8.Location = new System.Drawing.Point(14, 279);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(241, 29);
            this.label8.TabIndex = 20;
            this.label8.Text = "Account Unconfirmed";
            // 
            // txtAccountConfirmed
            // 
            this.txtAccountConfirmed.BackColor = System.Drawing.Color.White;
            this.txtAccountConfirmed.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountConfirmed.Location = new System.Drawing.Point(278, 215);
            this.txtAccountConfirmed.Name = "txtAccountConfirmed";
            this.txtAccountConfirmed.ReadOnly = true;
            this.txtAccountConfirmed.Size = new System.Drawing.Size(291, 34);
            this.txtAccountConfirmed.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label7.Location = new System.Drawing.Point(14, 215);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(216, 29);
            this.label7.TabIndex = 18;
            this.label7.Text = "Account Confirmed";
            // 
            // txtCoinType
            // 
            this.txtCoinType.BackColor = System.Drawing.Color.White;
            this.txtCoinType.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCoinType.Location = new System.Drawing.Point(278, 148);
            this.txtCoinType.Name = "txtCoinType";
            this.txtCoinType.ReadOnly = true;
            this.txtCoinType.Size = new System.Drawing.Size(291, 34);
            this.txtCoinType.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label6.Location = new System.Drawing.Point(14, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 29);
            this.label6.TabIndex = 16;
            this.label6.Text = "Coin Type";
            // 
            // txtAccountHDPath
            // 
            this.txtAccountHDPath.BackColor = System.Drawing.Color.White;
            this.txtAccountHDPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountHDPath.Location = new System.Drawing.Point(278, 85);
            this.txtAccountHDPath.Name = "txtAccountHDPath";
            this.txtAccountHDPath.ReadOnly = true;
            this.txtAccountHDPath.Size = new System.Drawing.Size(291, 34);
            this.txtAccountHDPath.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label5.Location = new System.Drawing.Point(14, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(192, 29);
            this.label5.TabIndex = 14;
            this.label5.Text = "Account HD Path";
            // 
            // txtAccountName
            // 
            this.txtAccountName.BackColor = System.Drawing.Color.White;
            this.txtAccountName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountName.Location = new System.Drawing.Point(278, 16);
            this.txtAccountName.Name = "txtAccountName";
            this.txtAccountName.ReadOnly = true;
            this.txtAccountName.Size = new System.Drawing.Size(291, 34);
            this.txtAccountName.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label4.Location = new System.Drawing.Point(14, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 29);
            this.label4.TabIndex = 12;
            this.label4.Text = "Account Name";
            // 
            // btnClear
            // 
            this.btnClear.BackgroundImage = global::SaleDeedRegistry.Wallet.Properties.Resources.button_blue;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnClear.Location = new System.Drawing.Point(895, 439);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(177, 53);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGetBalance
            // 
            this.btnGetBalance.BackgroundImage = global::SaleDeedRegistry.Wallet.Properties.Resources.button_blue;
            this.btnGetBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetBalance.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnGetBalance.Location = new System.Drawing.Point(652, 439);
            this.btnGetBalance.Name = "btnGetBalance";
            this.btnGetBalance.Size = new System.Drawing.Size(205, 53);
            this.btnGetBalance.TabIndex = 10;
            this.btnGetBalance.Text = "Get Balance";
            this.btnGetBalance.UseVisualStyleBackColor = true;
            this.btnGetBalance.Click += new System.EventHandler(this.btnGetBalance_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(28, 512);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1044, 300);
            this.dataGridView1.TabIndex = 13;
            // 
            // FrmWalletBalance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(1091, 828);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnGetBalance);
            this.Controls.Add(this.txtWalletName);
            this.Controls.Add(this.label1);
            this.Name = "FrmWalletBalance";
            this.Text = "Wallet Balance";
            this.Load += new System.EventHandler(this.FrmWalletBalance_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetBalance;
        private System.Windows.Forms.TextBox txtWalletName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSpendableAmount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtAccountUnConfirmed;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAccountConfirmed;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCoinType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAccountHDPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtAccountName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}