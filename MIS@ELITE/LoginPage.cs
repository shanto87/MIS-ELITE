using System;
using System.Drawing;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class LoginPage : Form
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginPage_Load(object sender, EventArgs e)
        {
            try
            {
                tbUserName.Focus();
                if (string.IsNullOrEmpty(Properties.Settings.Default.Server_IP))
                {
                    using (var prompt = new ServerPromptForm())
                    {
                        if (prompt.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(prompt.ServerIP) && !string.IsNullOrEmpty(prompt.ServerUser))
                        {
                            Properties.Settings.Default.Server_IP = prompt.ServerIP;
                            Properties.Settings.Default.Server_USER = prompt.ServerUser;
                            Properties.Settings.Default.Server_PASSWORD = prompt.ServerPassword;
                            Properties.Settings.Default.Database = prompt.DatabaseName;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            MessageBox.Show("Server IP and USER is required to continue.");
                        }
                    }
                }

                lblDate.Text = DateTime.Now.ToString("dddd dd-MM-yy hh:mm tt");
                if (Properties.Settings.Default.Server_IP != string.Empty)
                {
                    CheckDatabaseConnection();
                    InformationRetriever informationRetriever = new InformationRetriever();
                    lblCompanyName.Text = informationRetriever.SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE id=1");

                    //check if already logged in
                    bool isloggedin = Properties.Settings.Default.IsLoggedIn;
                    if (isloggedin)
                    {
                        new Inventory().Show();
                        this.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the application: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CheckDatabaseConnection()
        {
            DatabaseConnection db = new DatabaseConnection();
            if (db.IsServerConnected() == "true")
            {
                //btnLogin.Enabled = true;
                lblStatus.Text = "ONLINE";
                lblStatus.ForeColor = Color.FromArgb(173, 255, 47);
                lblStatus.BackColor = Color.FromArgb(0, 0, 0);
                lblConnectionStatus.Visible = false;
                btnRetry.Visible = false;
            }
            else
            {
                //btnLogin.Enabled = false;
                lblStatus.Text = "ERROR";
                lblStatus.ForeColor = Color.FromArgb(255, 0, 0);
                lblStatus.BackColor = Color.FromArgb(255, 255, 255);
                lblConnectionStatus.Text = "DATABASE SERVER ERROR" + Environment.NewLine + db.IsServerConnected();
            }
            db.CloseConnection();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            new ServerPromptForm().ShowDialog();
            CheckDatabaseConnection();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbUserName.Text = "";
            tbPassword.Text = "";
            tbUserType.Text = "";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsValid())
                {
                    MessageBox.Show("Input all fields.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string inputUsername = tbUserName.Text.Trim();
                string inputPassword = tbPassword.Text.Trim();
                string inputUserType = tbUserType.Text.Trim();

                DatabaseConnection db = new DatabaseConnection();
                var reader = db.Retrieve($"SELECT NAME, PASSWORD, USERTYPE FROM AUTHENTICATION WHERE USERNAME='{inputUsername}'");

                if (!reader.HasRows)
                {
                    MessageBox.Show("No User found with this username", "Failed Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string fullname = "", dbHashedPassword = "", dbUserType = "";

                while (reader.Read())
                {
                    fullname = reader["NAME"].ToString();
                    dbHashedPassword = reader["PASSWORD"].ToString();
                    dbUserType = reader["USERTYPE"].ToString();
                }

                reader.Close();
                db.CloseConnection();

                // Unified check
                bool isPasswordValid = PasswordHelper.VerifyPassword(inputPassword, dbHashedPassword);
                //MessageBox.Show($"Password valid: {isPasswordValid}\nInput UserType: {inputUserType}\nDB UserType: {dbUserType}", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (isPasswordValid && inputUserType == dbUserType)
                {
                    ProceedToInventory(inputUsername, fullname, dbUserType);
                }
                else
                {
                    MessageBox.Show($"Error. Username, password, or user type incorrect.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during login: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool IsValid()
        {
            //custom function for checking if all input fields are filled. 
            if (tbUserName.Text.ToString() == "" || tbUserType.Text.ToString() == "" || tbPassword.Text.ToString() == "")
            {
                return false;
            }
            return true;
        }
        private void ProceedToInventory(string username, string Fullname, string usertype)
        {
            Properties.Settings.Default.LoggedUserName = username;
            Properties.Settings.Default.LoggedUserFullName = Fullname;
            Properties.Settings.Default.AccessLevel = usertype;
            Properties.Settings.Default.IsLoggedIn = true;
            Properties.Settings.Default.Save();
            Inventory inventory = new Inventory();
            inventory.Show();
            this.Hide();
        }

        private void tbUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbPassword.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbUserType.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void tbUserType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void linklblForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (tbUserName.Text.Trim() == "")
            {
                MessageBox.Show("Please enter your username in the username field before clicking 'Forgot Password'.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            new AddSystemUser(tbUserName.Text.Trim()).ShowDialog();
        }
    }
}
