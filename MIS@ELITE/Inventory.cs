using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class Inventory : Form
    {
        public Inventory()
        {
            InitializeComponent();
        }

        private void Inventory_Load(object sender, EventArgs e)
        {
            //access level control
            if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
            {
                btnRegisteredNames.Enabled = false;
                btnAccounts.Enabled = false;
                btnStock.Enabled = false;
                btnAdvancePayment.Enabled = false;
                btnTransactions.Enabled = false;
                btnInvestments.Enabled = false;
                btnDiscounts.Enabled = false;
                btnExpenses.Enabled = false;
                btnBalanceStatement.Enabled = false;
                btnMonthlyStatement.Enabled = false;
                btnCompanyStats.Enabled = false;
                btnChangeServer.Enabled = false;
                button8.Enabled = false;
            }

            lblConnectionInfo.Text = $"Connected to Server : {Properties.Settings.Default.Server_IP} - With Database User : {Properties.Settings.Default.Server_USER} - At Database : {Properties.Settings.Default.Database}";
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            lblVersion.Text = string.Concat($"Inventory Control Utility : Version - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}");
            InformationRetriever informationRetriever = new InformationRetriever();
            lblCompanyName.Text = informationRetriever.SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE id=1");
            lblLoggedUser.Text = string.Concat("LOGGED USER : ", Properties.Settings.Default.LoggedUserFullName).ToUpper();
        }

        private void btnRegisteredNames_Click(object sender, EventArgs e)
        {
            new CustomerManagement().Show();
        }

        private void btnAccounts_Click(object sender, EventArgs e)
        {
            new Accounts().Show();
        }

        private void btnStock_Click(object sender, EventArgs e)
        {
            new StockManagement().Show();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            new PointOfSale().Show();
        }

        private void lblCompanyName_Click(object sender, EventArgs e)
        {

        }

        private void btnSalesData_Click(object sender, EventArgs e)
        {
            new Sales().Show();
        }

        private void btnTransactions_Click(object sender, EventArgs e)
        {
            new Transactions().Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new AdvancePayments().Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new Investments().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Expenses().Show();
        }

        private void btnDiscounts_Click(object sender, EventArgs e)
        {
            new Discounts().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new CustomerDue().Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new AccountBalance().Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string serverip = Properties.Settings.Default.Server_IP;
            string database = Properties.Settings.Default.Database;
            Process.Start($"http://{serverip}/phpmyadmin/index.php?route=/database/export&db={database}");
        }

        private void btnCompanyStats_Click(object sender, EventArgs e)
        {
            new BusinessValuation().Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            new MonthlyStatement().Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new TrackSoldProducts().Show();
        }

        private void btnChangeServer_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ServerPromptForm().ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LoggedUserName = string.Empty;
            Properties.Settings.Default.LoggedUserFullName = string.Empty;
            Properties.Settings.Default.IsLoggedIn = false;
            Properties.Settings.Default.Save();
            new LoginPage().Show();
            this.Hide();
        }

        private void btnAdvancePayment_Click(object sender, EventArgs e)
        {
            new AdvancePayments().Show();
        }

        private void btnInvestments_Click(object sender, EventArgs e)
        {
            new Investments().Show();
        }

        private void btnExpenses_Click(object sender, EventArgs e)
        {
            new Expenses().Show();
        }

        private void btnCustomerDue_Click(object sender, EventArgs e)
        {
            new CustomerDue().Show();
        }

        private void btnBalanceStatement_Click(object sender, EventArgs e)
        {
            new AccountBalance().Show();
        }

        private void btnMonthlyStatement_Click(object sender, EventArgs e)
        {
            new MonthlyStatement().Show();
        }

        private void btnTrackSoldProduct_Click(object sender, EventArgs e)
        {
            new TrackSoldProducts().Show();
        }

        private void btnAddSystemUser_Click(object sender, EventArgs e)
        {
            new AddSystemUser(string.Empty).Show();
        }
    }
}
