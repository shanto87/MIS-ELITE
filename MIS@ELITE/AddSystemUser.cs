using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MIS_ELITE
{
    public partial class AddSystemUser : Form
    {
        public string LoginPageUserName
        {
            get; private set;
        }

        public AddSystemUser(string UserName)
        {
            LoginPageUserName = UserName;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidInput())
                {
                    if (tbNewPwdWithoutOtp.Text.Length < 4)
                    {
                        MessageBox.Show("Password must be at least 4 characters long.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (cbAccessLevel.SelectedItem == null)
                    {
                        MessageBox.Show("Please select an access level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string username = tbUserName.Text;
                    string fullname = tbFullName.Text;
                    string accessLevel = cbAccessLevel.Text;
                    string email = tbEmail.Text;
                    string encryptedPassword = PasswordHelper.HashPassword(tbNewPwdWithoutOtp.Text);
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"INSERT INTO Authentication (USERNAME, NAME, PASSWORD, USERTYPE, EMAIL) VALUES ('{username}','{fullname}','{encryptedPassword}','{accessLevel}','{email}')";
                    db.ExecuteWithoutAlert(query, useTransaction: true);
                    MessageBox.Show("User added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding the user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSystemUser_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.IsLoggedIn)
            {
                tbOtp.Enabled = false;
                tbOldPassword.Visible = false;
                tbOldPassword.UseSystemPasswordChar = true;
                button1.Text = "🙈 "; // or set to an appropriate icon for "hide password"
                RecoveryPanel(false);
                tbRecoveryUsername.Enabled = false;
                btnChangeData.Visible = false;
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    btnOK.Visible = false;
                    btnChangeData.Visible = true;
                    cbAccessLevel.Enabled = false;
                    cbAccessLevel.Text = Properties.Settings.Default.AccessLevel;
                    tbUserName.Enabled = false;
                    tbUserName.Text = Properties.Settings.Default.LoggedUserName;
                    tbFullName.Text = Properties.Settings.Default.LoggedUserFullName;
                    tbRecoveryUsername.Text = Properties.Settings.Default.LoggedUserName;
                    lblWarning.Visible = false;
                    tbOldPassword.Visible = true;
                }
            }
            else
            {
                //hit from login page, disable all fields and show recovery panel
                tbUserName.Text = LoginPageUserName;
                tbRecoveryUsername.Text = LoginPageUserName;
                tbRecoveryMail.Text = tbEmail.Text;
                tbUserName.Enabled = false;
                tbRecoveryUsername.Enabled = false;
                tbFullName.Enabled = false;
                tbEmail.Enabled = false;
                tbOldPassword.Enabled = false;
                tbNewPwdWithoutOtp.Enabled = false;
                cbAccessLevel.Enabled = false;
                btnOK.Enabled = false;
                btnChangeData.Enabled = false;
                linkLabel1.Enabled = false;
                tbOtp.Enabled = false;
            }
        }

        private void btnChangeData_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidInput())
                {
                    if (tbNewPwdWithoutOtp.Text.Length < 4)
                    {
                        MessageBox.Show("Password must be at least 4 characters long.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (tbOldPassword.Text.Length < 4)
                    {
                        MessageBox.Show("Please enter your old password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                string query = $"SELECT Password FROM Authentication WHERE UserName='{tbUserName.Text}'";
                string storedHash = new InformationRetriever().SingleDataGetter(query);
                if (PasswordHelper.VerifyPassword(tbOldPassword.Text, storedHash))
                {
                    // Old password is correct, proceed with update
                    string changedPassword = PasswordHelper.HashPassword(tbNewPwdWithoutOtp.Text);
                    string UpdateQuery = $"UPDATE Authentication SET Name='{tbFullName.Text}', Password='{changedPassword}', Email='{tbEmail.Text}' WHERE UserName='{tbUserName.Text}'";
                    DatabaseConnection db = new DatabaseConnection();
                    db.Execute(UpdateQuery, useTransaction: true);
                    MessageBox.Show("Data changed successfully.\nNOTE: USERNAME and ACCESS LEVEL cannot be changed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Old password is incorrect.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while changing the user data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbUserName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //Modify Button Disabled
                if (tbUserName.Text == Properties.Settings.Default.LoggedUserName)
                {
                    btnChangeData.Visible = true;
                    tbOldPassword.Visible = true;
                    btnOK.Visible = false;
                    tbRecoveryUsername.Text = tbUserName.Text;
                    tbRecoveryMail.Text = tbEmail.Text;
                }
                else
                {
                    btnChangeData.Visible = false;
                    tbOldPassword.Visible = false;
                    btnOK.Visible = true;
                    tbRecoveryUsername.Text = string.Empty;
                    cbAccessLevel.Text = "";
                    tbRecoveryMail.Text = string.Empty;
                }

                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT COUNT(*) FROM Authentication WHERE UserName='{tbUserName.Text}'";
                var result = new InformationRetriever().SingleDataGetter(query);
                int count = result != null ? Convert.ToInt32(result) : 0;
                if (count > 0)
                {
                    lblWarning.Text = "Username already exists.";
                    lblWarning.ForeColor = Color.Red;
                    btnOK.Visible = false;
                    var FullNameQuery = $"SELECT Name FROM Authentication WHERE UserName='{tbUserName.Text}'";
                    var EmailQuery = $"SELECT Email FROM Authentication WHERE UserName='{tbUserName.Text}'";
                    tbFullName.Text = new InformationRetriever().SingleDataGetter(FullNameQuery);
                    tbEmail.Text = new InformationRetriever().SingleDataGetter(EmailQuery);
                    cbAccessLevel.Text = new InformationRetriever().SingleDataGetter($"SELECT Usertype FROM Authentication WHERE UserName='{tbUserName.Text}'");
                }
                else
                {
                    lblWarning.Text = "Username is available.";
                    lblWarning.ForeColor = Color.Green;
                    tbFullName.Text = string.Empty;
                    tbEmail.Text = string.Empty;
                    btnOK.Visible = true;
                    cbAccessLevel.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while checking the username: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tbOldPassword.UseSystemPasswordChar)
            {
                tbOldPassword.UseSystemPasswordChar = false; // show old password
                tbNewPwdWithoutOtp.UseSystemPasswordChar = false; // show new password in recovery panel
                button1.Text = "👁️"; // or change icon to "eye open"
            }
            else
            {
                tbOldPassword.UseSystemPasswordChar = true; // hide old password      
                tbNewPwdWithoutOtp.UseSystemPasswordChar = true; // hide new password in recovery panel
                button1.Text = "🙈"; // or change icon to "eye hide"
            }
        }

        private void btnCancelRecovery_Click(object sender, EventArgs e)
        {
            RecoveryPanel(false);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RecoveryPanel(true);
        }

        private void RecoveryPanel(bool state)
        {
            label6.Visible = state;
            label7.Visible = state;
            label8.Visible = state;
            label9.Visible = state;
            tbRecoveryMail.Visible = state;
            tbRecoveryUsername.Visible = state;
            tbNewPassword.Visible = state;
            tbOtp.Visible = state;
            btnCancelRecovery.Visible = state;
            btnRecoverPassword.Visible = state;
            btnGenerateOtp.Visible = state;
        }

        private void btnGenerateOtp_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidInputForPasswordRecovery())
                {
                    string OTP = GenerateOtp();
                    SendOtpEmail(tbRecoveryMail.Text, OTP, tbFullName.Text);
                    tbOtp.Enabled = true;
                    string expiry = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss");
                    string query = $"INSERT INTO PasswordResetTokens (Username, OTP, ExpiryTime, CreatedAt) VALUES ('{tbRecoveryUsername.Text}', '{OTP}', '{expiry}', '{DateTime.Now:yyyy-MM-dd HH:mm:ss}') ON DUPLICATE KEY UPDATE OTP = VALUES(OTP), ExpiryTime = VALUES(ExpiryTime), CreatedAt = VALUES(CreatedAt);";
                    DatabaseConnection db = new DatabaseConnection();
                    db.ExecuteWithoutAlert(query, true);
                    MessageBox.Show("OTP has been sent to your email. It will expire in 10 minutes.", "OTP Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnGenerateOtp.Enabled = false; // Disable the button to prevent multiple OTP generation
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while generating OTP: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private bool IsValidInputForPasswordRecovery()
        {
            if (tbRecoveryMail.Text == string.Empty)
            {
                MessageBox.Show("Please enter your recovery email.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (tbRecoveryUsername.Text == string.Empty)
            {
                MessageBox.Show("Username cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (tbNewPassword.Text.Length < 4)
            {
                MessageBox.Show("Password must be at least 4 characters long.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool IsValidInput()
        {
            if (tbUserName.Text == string.Empty)
            {
                MessageBox.Show("Username cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (tbFullName.Text == string.Empty)
            {
                MessageBox.Show("Full Name cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (tbEmail.Text == string.Empty)
            {
                MessageBox.Show("Email cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public static string GenerateOtp(int length = 6)
        {
            var rng = new Random();
            return string.Concat(Enumerable.Range(0, length).Select(_ => rng.Next(0, 10).ToString()));
        }

        public void SendOtpEmail(string toEmail, string otp, string FullName)
        {
            var fromAddress = new MailAddress("eliteintbd@gmail.com", "MIS@ELITE System");
            var toAddress = new MailAddress(toEmail);
            const string subject = "Password Reset OTP";
            string body = $"Dear {FullName},\n\nYour One-Time Password (OTP) is {otp}. " +
                 $"This code will expire in 10 minutes. " +
                 $"The request was generated on {DateTime.Now:dddd, dd MMM yyyy hh:mm tt}.\n\n" +
                 "Please use this code promptly to complete your verification process." +
                 "For your security, do not share this code with anyone.\n\n" +
                 "If you did not request this OTP, please contact our support team immediately.";


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("eliteintbd@gmail.com", "wsum hqac wsko qwrq")
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        public bool VerifyOtp(string username, string otp)
        {
            string query = $"SELECT ExpiryTime FROM PasswordResetTokens WHERE Username='{username}' AND OTP='{otp}'";
            var expiryObj = new InformationRetriever().SingleDataGetter(query);

            if (expiryObj == null)
                return false;

            // Safely cast to DateTime
            if (DateTime.TryParse(expiryObj.ToString(), out DateTime expiryTime))
            {
                return DateTime.Now <= expiryTime;
            }

            // If parsing fails, treat as invalid
            return false;
        }


        private void btnRecoverPassword_Click(object sender, EventArgs e)
        {
            if (tbRecoveryMail.Text == string.Empty)
            {
                MessageBox.Show("Please enter your recovery email.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (tbNewPassword.Text == string.Empty || tbNewPassword.TextLength < 4)
            {
                MessageBox.Show("Password cannot be empty or less than 4 character", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (tbOtp.Text == string.Empty)
            {
                MessageBox.Show("Please enter the OTP sent to your email.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (VerifyOtp(tbRecoveryUsername.Text, tbOtp.Text))
            {
                string changedPassword = PasswordHelper.HashPassword(tbNewPassword.Text);
                string query = $"UPDATE Authentication SET Password='{changedPassword}' WHERE UserName='{tbRecoveryUsername.Text}'";
                DatabaseConnection db = new DatabaseConnection();
                db.ExecuteWithoutAlert(query, useTransaction: true);
                MessageBox.Show("Password changed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RecoveryPanel(false);
                new DatabaseConnection().ExecuteWithoutAlert($"DELETE FROM PasswordResetTokens WHERE Username='{tbRecoveryUsername.Text}'", true);
            }
            else
            {
                MessageBox.Show("Invalid or expired OTP. Please try again.", "OTP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbFullName_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbEmail_TextChanged(object sender, EventArgs e)
        {
            tbRecoveryMail.Text = tbEmail.Text;
        }
    }
}