using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class AccountBalance : Form
    {
        public AccountBalance()
        {
            InitializeComponent();
        }

        public void LoadDefaults()
        {
            try
            {
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE id=1"); //getting company name

                //Bangladesh Branch Balance Calculation
                string query = "SELECT DISTINCT AccountName FROM accounts"; //getting account names from account table
                DatabaseConnection db = new DatabaseConnection();
                var AccountNames = db.Retrieve(query);
                if (AccountNames.HasRows)
                {
                    while (AccountNames.Read())
                    {
                        dgvBalance.Rows.Add(dgvBalance.Rows.Count + 1, AccountNames.GetValue(0).ToString());
                    }
                    AccountNames.Close();
                    //db.CloseConnection();
                }

                for (int i = 0; i < dgvBalance.Rows.Count; i++)
                {
                    string accountName = dgvBalance.Rows[i].Cells[1].Value.ToString();
                    dgvBalance.Rows[i].Cells[2].Value = new InformationRetriever().SingleDataGetter($"SELECT SUM(AdvanceAmount) FROM AdvancePayment WHERE ToAccount='{accountName}'");//advance totals on accountname
                    dgvBalance.Rows[i].Cells[3].Value = new InformationRetriever().SingleDataGetter($"SELECT SUM(Paid_Amount) FROM Transaction WHERE To_Account='{accountName}'");//payment total to accountname
                    dgvBalance.Rows[i].Cells[4].Value = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Investment WHERE FromAccount='{accountName}'");//investment total from accountname
                    dgvBalance.Rows[i].Cells[5].Value = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Expense WHERE FromAccount='{accountName}'");//expense total to accountname
                    double Balance = IsNullOrNumber(dgvBalance.Rows[i].Cells[2].Value.ToString()) + IsNullOrNumber(dgvBalance.Rows[i].Cells[3].Value.ToString()) - IsNullOrNumber(dgvBalance.Rows[i].Cells[4].Value.ToString()) - IsNullOrNumber(dgvBalance.Rows[i].Cells[5].Value.ToString());//balance calculation
                    //Balance Formula = Advance + Payment - Investment - Expense
                    dgvBalance.Rows[i].Cells[6].Value = Balance.ToString("0.00"); //balance amount
                }

                //Foreign Branch Balance Calculation
                string query2 = "SELECT DISTINCT AccountName FROM AccountsForeign"; //getting account names from account table
                DatabaseConnection db2 = new DatabaseConnection();
                var AccountNames2 = db2.Retrieve(query2);
                if (AccountNames2.HasRows)
                {
                    while (AccountNames2.Read())
                    {
                        dgvBalanceForeign.Rows.Add(dgvBalanceForeign.Rows.Count + 1, AccountNames2.GetValue(0).ToString());
                    }
                    AccountNames2.Close();
                    //db2.CloseConnection();
                }

                for (int i = 0; i < dgvBalanceForeign.Rows.Count; i++)
                {
                    string accountName = dgvBalanceForeign.Rows[i].Cells[1].Value.ToString();
                    dgvBalanceForeign.Rows[i].Cells[2].Value = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Investment WHERE ToForeignAccount='{accountName}'");//investment Received totals on accountname
                    dgvBalanceForeign.Rows[i].Cells[3].Value = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM InvestmentForeign WHERE FromAccount='{accountName}'");//invested total to accountname
                    dgvBalanceForeign.Rows[i].Cells[4].Value = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Expense WHERE FromAccount='{accountName}'");//expense total to accountname
                    double ReceivedTotal = IsNullOrNumber(dgvBalanceForeign.Rows[i].Cells[2].Value.ToString());
                    double InvestedTotal = IsNullOrNumber(dgvBalanceForeign.Rows[i].Cells[3].Value.ToString());
                    double ExpenseTotal = IsNullOrNumber(dgvBalanceForeign.Rows[i].Cells[4].Value.ToString());
                    double Balance = ReceivedTotal - InvestedTotal - ExpenseTotal;      //balance calculation
                    //Balance Formula = investmentReceivedTotal-InvestedTotal-ExpenseTotal
                    dgvBalanceForeign.Rows[i].Cells[5].Value = Balance.ToString("0.00"); //balance amount
                }


                //Local+Foreign Balance Calculation
                double localBalance = 0;
                double foreignBalance = 0;
                foreach (DataGridViewRow row in dgvBalance.Rows)
                {
                    localBalance += IsNullOrNumber(row.Cells[6].Value.ToString());
                }
                foreach (DataGridViewRow row in dgvBalanceForeign.Rows)
                {
                    foreignBalance += IsNullOrNumber(row.Cells[5].Value.ToString());
                }
                double totalBalance = localBalance + foreignBalance;
                label1.Text += localBalance.ToString("0.00") + " || " + new ConvertNumberToWords().ConvertNumberToWord(localBalance) + " tk Only";
                label3.Text += foreignBalance.ToString("0.00") + " || " + new ConvertNumberToWords().ConvertNumberToWord(foreignBalance) + " tk Only";
                ;
                lblTotalBalance.Text = "Total Balance : " + totalBalance.ToString("0.00"); //total balance amount
                new DatabaseConnection().ExecuteWithoutAlert($"UPDATE AccountBalance SET LocalBalance='{localBalance}', ForeignBalance='{foreignBalance}', TotalBalance='{totalBalance}' WHERE id=1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double IsNullOrNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(value);
            }
        }


        private void AccountBalance_Load(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void btnRecalculate_Click(object sender, EventArgs e)
        {
            dgvBalance.Rows.Clear();
            dgvBalanceForeign.Rows.Clear();
            label1.Text = "BALANCE AT BANGLADESH BRANCH : ";
            label3.Text = "BALANCE AT FOREIGN BRANCH : ";
            Array loading = new string[] { "R", "E", "C", "A", "L", "C", "U", "L", "A", "T", "I", "N", "G", ".", ".", "." };
            lblRefresing.Text = string.Empty;
            foreach (string item in loading)
            {
                lblRefresing.Text += item;
                Application.DoEvents();
                Task.Delay(65).Wait();
            }
            LoadDefaults();
            Task.Delay(1000).Wait();
            lblRefresing.Text = string.Empty;
        }
    }
}
