using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class ServerPromptForm : Form
    {
        public string ServerIP
        {
            get; private set;
        }
        public string ServerUser
        {
            get; private set;
        }
        public string ServerPassword
        {
            get; private set;
        }
        public string DatabaseName
        {
            get; private set;
        }

        public ServerPromptForm()
        {
            InitializeComponent();
        }

        public static bool IsValidIp(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            IPAddress address;
            return IPAddress.TryParse(input, out address);
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            ServerIP = txtServerIP.Text;
            ServerUser = txtServerUser.Text;
            ServerPassword = txtServerPassword.Text;
            DatabaseName = txtDatabase.Text;
            if (IsValidIp(ServerIP))
            {
                Properties.Settings.Default.Server_IP = ServerIP;
                Properties.Settings.Default.Server_USER = ServerUser;
                Properties.Settings.Default.Server_PASSWORD = ServerPassword;
                Properties.Settings.Default.Database = DatabaseName;
                Properties.Settings.Default.Save();
                this.DialogResult = DialogResult.OK;
                this.Close();
                MessageBox.Show("Update Successfull.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (Properties.Settings.Default.IsLoggedIn)
                {
                    Inventory inventoryForm = new Inventory();
                    inventoryForm.Show();
                    inventoryForm.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Invalid IP address. Please enter a valid IPv4 address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChangeIP_Click(object sender, EventArgs e)
        {
            ServerIP = txtServerIP.Text;
            ServerUser = txtServerUser.Text;
            ServerPassword = txtServerPassword.Text;
            DatabaseName = txtDatabase.Text;
            if (IsValidIp(ServerIP))
            {
                Properties.Settings.Default.Server_IP = ServerIP;
                Properties.Settings.Default.Server_USER = txtServerUser.Text;
                Properties.Settings.Default.Server_PASSWORD = ServerPassword;
                Properties.Settings.Default.Database = DatabaseName;
                Properties.Settings.Default.Save();
                MessageBox.Show("Change Successfull.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (Properties.Settings.Default.IsLoggedIn)
                {
                    this.Hide();
                    Inventory inventoryForm = new Inventory();
                    inventoryForm.Show();
                }
            }
            else
            {
                MessageBox.Show("Invalid IP address. Please enter a valid IPv4 address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ServerPromptForm_Load(object sender, EventArgs e)
        {
            txtServerIP.Text = Properties.Settings.Default.Server_IP;
            txtServerUser.Text = Properties.Settings.Default.Server_USER;
            txtServerPassword.Text = "";
            txtDatabase.Text = Properties.Settings.Default.Database;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (Properties.Settings.Default.IsLoggedIn)
            {
                Inventory inventoryForm = new Inventory();
                inventoryForm.Show();
            }
        }
    }
}
