namespace MIS_ELITE
{
    partial class AddSystemUser
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.cbAccessLevel = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbOldPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbFullName = new System.Windows.Forms.TextBox();
            this.btnChangeData = new System.Windows.Forms.Button();
            this.lblWarning = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbRecoveryUsername = new System.Windows.Forms.TextBox();
            this.tbRecoveryMail = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbNewPassword = new System.Windows.Forms.TextBox();
            this.btnGenerateOtp = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.tbOtp = new System.Windows.Forms.TextBox();
            this.btnRecoverPassword = new System.Windows.Forms.Button();
            this.btnCancelRecovery = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.tbNewPwdWithoutOtp = new System.Windows.Forms.TextBox();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(108, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "USERNAME :";
            // 
            // tbUserName
            // 
            this.tbUserName.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbUserName.Location = new System.Drawing.Point(207, 118);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(301, 26);
            this.tbUserName.TabIndex = 1;
            this.tbUserName.TextChanged += new System.EventHandler(this.tbUserName_TextChanged);
            // 
            // cbAccessLevel
            // 
            this.cbAccessLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAccessLevel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbAccessLevel.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAccessLevel.FormattingEnabled = true;
            this.cbAccessLevel.Items.AddRange(new object[] {
            "GENERAL",
            "EMPLOYEE"});
            this.cbAccessLevel.Location = new System.Drawing.Point(207, 278);
            this.cbAccessLevel.Name = "cbAccessLevel";
            this.cbAccessLevel.Size = new System.Drawing.Size(301, 26);
            this.cbAccessLevel.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Red;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(208, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.YellowGreen;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(413, 310);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 30);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "ADD USER";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(75, 218);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "OLD PASSWORD :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(83, 282);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "ACCESS LEVEL :";
            // 
            // tbOldPassword
            // 
            this.tbOldPassword.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbOldPassword.Location = new System.Drawing.Point(207, 214);
            this.tbOldPassword.Name = "tbOldPassword";
            this.tbOldPassword.Size = new System.Drawing.Size(256, 26);
            this.tbOldPassword.TabIndex = 4;
            this.tbOldPassword.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bahnschrift", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(156, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(312, 33);
            this.label4.TabIndex = 0;
            this.label4.Text = "SYSTEM USER CONTROL";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(104, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 18);
            this.label5.TabIndex = 0;
            this.label5.Text = "FULL NAME :";
            // 
            // tbFullName
            // 
            this.tbFullName.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFullName.Location = new System.Drawing.Point(207, 150);
            this.tbFullName.Name = "tbFullName";
            this.tbFullName.Size = new System.Drawing.Size(301, 26);
            this.tbFullName.TabIndex = 2;
            this.tbFullName.TextChanged += new System.EventHandler(this.tbFullName_TextChanged);
            // 
            // btnChangeData
            // 
            this.btnChangeData.BackColor = System.Drawing.Color.Yellow;
            this.btnChangeData.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnChangeData.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChangeData.ForeColor = System.Drawing.Color.DimGray;
            this.btnChangeData.Location = new System.Drawing.Point(308, 310);
            this.btnChangeData.Name = "btnChangeData";
            this.btnChangeData.Size = new System.Drawing.Size(94, 30);
            this.btnChangeData.TabIndex = 9;
            this.btnChangeData.Text = "UPDATE";
            this.btnChangeData.UseVisualStyleBackColor = false;
            this.btnChangeData.Click += new System.EventHandler(this.btnChangeData_Click);
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.Location = new System.Drawing.Point(204, 97);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(0, 18);
            this.lblWarning.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(469, 214);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(39, 58);
            this.button1.TabIndex = 6;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.LinkColor = System.Drawing.Color.Red;
            this.linkLabel1.Location = new System.Drawing.Point(204, 343);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(128, 18);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Forgot Password?";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(105, 408);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 18);
            this.label6.TabIndex = 0;
            this.label6.Text = "USERNAME :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(120, 440);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 18);
            this.label8.TabIndex = 0;
            this.label8.Text = "OTP MAIL :";
            // 
            // tbRecoveryUsername
            // 
            this.tbRecoveryUsername.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRecoveryUsername.Location = new System.Drawing.Point(206, 404);
            this.tbRecoveryUsername.Name = "tbRecoveryUsername";
            this.tbRecoveryUsername.Size = new System.Drawing.Size(301, 26);
            this.tbRecoveryUsername.TabIndex = 12;
            // 
            // tbRecoveryMail
            // 
            this.tbRecoveryMail.Enabled = false;
            this.tbRecoveryMail.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRecoveryMail.Location = new System.Drawing.Point(206, 436);
            this.tbRecoveryMail.Name = "tbRecoveryMail";
            this.tbRecoveryMail.Size = new System.Drawing.Size(300, 26);
            this.tbRecoveryMail.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(70, 473);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(130, 18);
            this.label7.TabIndex = 0;
            this.label7.Text = "NEW PASSWORD :";
            // 
            // tbNewPassword
            // 
            this.tbNewPassword.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNewPassword.Location = new System.Drawing.Point(206, 469);
            this.tbNewPassword.Name = "tbNewPassword";
            this.tbNewPassword.Size = new System.Drawing.Size(300, 26);
            this.tbNewPassword.TabIndex = 14;
            this.tbNewPassword.UseSystemPasswordChar = true;
            // 
            // btnGenerateOtp
            // 
            this.btnGenerateOtp.BackColor = System.Drawing.Color.SteelBlue;
            this.btnGenerateOtp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGenerateOtp.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerateOtp.ForeColor = System.Drawing.Color.White;
            this.btnGenerateOtp.Location = new System.Drawing.Point(413, 502);
            this.btnGenerateOtp.Name = "btnGenerateOtp";
            this.btnGenerateOtp.Size = new System.Drawing.Size(93, 26);
            this.btnGenerateOtp.TabIndex = 15;
            this.btnGenerateOtp.Text = "GENERATE OTP";
            this.btnGenerateOtp.UseVisualStyleBackColor = false;
            this.btnGenerateOtp.Click += new System.EventHandler(this.btnGenerateOtp_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(159, 505);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 18);
            this.label9.TabIndex = 0;
            this.label9.Text = "OTP :";
            // 
            // tbOtp
            // 
            this.tbOtp.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbOtp.Location = new System.Drawing.Point(207, 501);
            this.tbOtp.Name = "tbOtp";
            this.tbOtp.Size = new System.Drawing.Size(200, 26);
            this.tbOtp.TabIndex = 16;
            this.tbOtp.UseSystemPasswordChar = true;
            // 
            // btnRecoverPassword
            // 
            this.btnRecoverPassword.BackColor = System.Drawing.Color.DarkMagenta;
            this.btnRecoverPassword.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRecoverPassword.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecoverPassword.ForeColor = System.Drawing.Color.White;
            this.btnRecoverPassword.Location = new System.Drawing.Point(297, 533);
            this.btnRecoverPassword.Name = "btnRecoverPassword";
            this.btnRecoverPassword.Size = new System.Drawing.Size(110, 26);
            this.btnRecoverPassword.TabIndex = 17;
            this.btnRecoverPassword.Text = "Recover Password";
            this.btnRecoverPassword.UseVisualStyleBackColor = false;
            this.btnRecoverPassword.Click += new System.EventHandler(this.btnRecoverPassword_Click);
            // 
            // btnCancelRecovery
            // 
            this.btnCancelRecovery.BackColor = System.Drawing.Color.Crimson;
            this.btnCancelRecovery.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelRecovery.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelRecovery.ForeColor = System.Drawing.Color.White;
            this.btnCancelRecovery.Location = new System.Drawing.Point(217, 533);
            this.btnCancelRecovery.Name = "btnCancelRecovery";
            this.btnCancelRecovery.Size = new System.Drawing.Size(74, 26);
            this.btnCancelRecovery.TabIndex = 18;
            this.btnCancelRecovery.Text = "Cancel";
            this.btnCancelRecovery.UseVisualStyleBackColor = false;
            this.btnCancelRecovery.Click += new System.EventHandler(this.btnCancelRecovery_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(71, 250);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(130, 18);
            this.label10.TabIndex = 0;
            this.label10.Text = "NEW PASSWORD :";
            // 
            // tbNewPwdWithoutOtp
            // 
            this.tbNewPwdWithoutOtp.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNewPwdWithoutOtp.Location = new System.Drawing.Point(208, 246);
            this.tbNewPwdWithoutOtp.Name = "tbNewPwdWithoutOtp";
            this.tbNewPwdWithoutOtp.Size = new System.Drawing.Size(255, 26);
            this.tbNewPwdWithoutOtp.TabIndex = 5;
            this.tbNewPwdWithoutOtp.UseSystemPasswordChar = true;
            // 
            // tbEmail
            // 
            this.tbEmail.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEmail.Location = new System.Drawing.Point(205, 182);
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(301, 26);
            this.tbEmail.TabIndex = 3;
            this.tbEmail.TextChanged += new System.EventHandler(this.tbEmail_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Bahnschrift", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(142, 186);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 18);
            this.label11.TabIndex = 0;
            this.label11.Text = "E-Mail :";
            // 
            // AddSystemUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 611);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnChangeData);
            this.Controls.Add(this.btnCancelRecovery);
            this.Controls.Add(this.btnRecoverPassword);
            this.Controls.Add(this.btnGenerateOtp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cbAccessLevel);
            this.Controls.Add(this.tbOtp);
            this.Controls.Add(this.tbNewPassword);
            this.Controls.Add(this.tbNewPwdWithoutOtp);
            this.Controls.Add(this.tbOldPassword);
            this.Controls.Add(this.tbRecoveryMail);
            this.Controls.Add(this.tbRecoveryUsername);
            this.Controls.Add(this.tbEmail);
            this.Controls.Add(this.tbFullName);
            this.Controls.Add(this.tbUserName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(650, 650);
            this.Name = "AddSystemUser";
            this.Text = "System Users";
            this.Load += new System.EventHandler(this.AddSystemUser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.ComboBox cbAccessLevel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbOldPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbFullName;
        private System.Windows.Forms.Button btnChangeData;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbRecoveryUsername;
        private System.Windows.Forms.TextBox tbRecoveryMail;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbNewPassword;
        private System.Windows.Forms.Button btnGenerateOtp;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbOtp;
        private System.Windows.Forms.Button btnRecoverPassword;
        private System.Windows.Forms.Button btnCancelRecovery;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbNewPwdWithoutOtp;
        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.Label label11;
    }
}