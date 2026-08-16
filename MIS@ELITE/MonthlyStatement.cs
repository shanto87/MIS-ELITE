using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class MonthlyStatement : Form
    {
        public MonthlyStatement()
        {
            InitializeComponent();
        }

        private void LoadDefaults()
        {
            try
            {
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT companyname from companyinfo where ID=1");
                lblSalesBaseData.Text = "/- tk";
                lblSalesData.Text = "/- tk";
                lblProfitData.Text = "/- tk";
                lblDiscountData.Text = "/- tk";
                lblExpenseData.Text = "/- tk";
                lblActualProfit.Text = "/- tk";
                lblGrossPercentageData.Text = " %";
                lblNetPercentage.Text = " %";
                dgvMonthlyStatement.Rows.Clear();

                //Search MonthName in diffetent table and populate the combo box with unique value
                HashSet<string> monthNames = new HashSet<string>();
                string[] Tables = { "AdvancePayment", "Discount", "Expense", "Investment", "InvestmentForeign", "Salesdata", "Transaction" };

                foreach (string table in Tables)
                {
                    string query = $"SELECT DISTINCT MonthName FROM {table}";
                    if (table == "Transaction")
                    {
                        query = $"SELECT DISTINCT Month_Name FROM {table}";
                    }
                    DatabaseConnection db = new DatabaseConnection();
                    var result = db.Retrieve(query);
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            string monthName = result.GetValue(0).ToString();
                            monthNames.Add(monthName);
                        }
                        result.Close();
                        db.CloseConnection();
                    }
                    else
                    {
                        db.CloseConnection();
                    }
                }

                // Populate the combo box with unique month names
                cbMonthName.Items.Clear();
                cbMonthName.Text = "";
                foreach (string monthName in monthNames)
                {
                    cbMonthName.Items.Add(monthName);
                    dgvMonthlyStatement.Rows.Add(dgvMonthlyStatement.Rows.Count + 1, monthName);
                }

                //Fillup other columns with specified month name
                foreach (DataGridViewRow row in dgvMonthlyStatement.Rows)
                {
                    string monthName = row.Cells[1].Value.ToString();

                    string advanceTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(AdvanceAmount) FROM AdvancePayment WHERE MonthName='{monthName}'");
                    row.Cells[2].Value = advanceTotal;

                    string salesTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(NetTotal) FROM Salesdata WHERE MonthName='{monthName}'");
                    row.Cells[3].Value = salesTotal;

                    string salesBaseTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(ProductBaseTotal) FROM Salesdata WHERE MonthName='{monthName}'");
                    row.Cells[4].Value = salesBaseTotal;

                    string profitTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(Profit) FROM Salesdata WHERE MonthName='{monthName}'");
                    row.Cells[5].Value = profitTotal;

                    //Profit percentage calculation
                    double sellPrice = NullOrNumber(row.Cells[3].Value.ToString());
                    double costPrice = NullOrNumber(row.Cells[4].Value.ToString());

                    if (costPrice == 0)
                    {
                        row.Cells[6].Value = "0.00 %"; //Avoiding divide by zero error
                    }
                    else
                    {
                        row.Cells[6].Value = (((sellPrice - costPrice) / costPrice) * 100).ToString("0.00") + " %";
                    }

                    string paymentsTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(Paid_Amount) FROM Transaction WHERE Month_Name='{monthName}'");
                    row.Cells[7].Value = paymentsTotal;

                    string investmentTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Investment WHERE MonthName='{monthName}'");
                    row.Cells[8].Value = investmentTotal;

                    string investmentForeignTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM InvestmentForeign WHERE MonthName='{monthName}'");
                    row.Cells[9].Value = investmentForeignTotal;

                    string discountTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Discount WHERE MonthName='{monthName}'");
                    row.Cells[10].Value = discountTotal;

                    string expenseTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Expense WHERE MonthName='{monthName}'");
                    row.Cells[11].Value = expenseTotal;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MonthlyStatement_Load(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private double NullOrNumber(string value)
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void cbMonthName_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvMonthlyStatement.Rows)
            {
                if (row.Cells[1].Value.ToString() == cbMonthName.Text)
                {
                    row.Visible = true;
                    row.Selected = true;
                    lblSalesData.Text = row.Cells[3].Value.ToString() + "/- tk";
                    lblSalesBaseData.Text = row.Cells[4].Value.ToString() + "/- tk";
                    lblProfitData.Text = row.Cells[5].Value.ToString() + "/- tk";
                    lblDiscountData.Text = row.Cells[10].Value.ToString() + "/- tk";
                    lblExpenseData.Text = row.Cells[11].Value.ToString() + "/- tk";
                    lblActualProfit.Text = ((NullOrNumber(row.Cells[5].Value.ToString()) - NullOrNumber(row.Cells[10].Value.ToString()) - NullOrNumber(row.Cells[11].Value.ToString()))).ToString() + "/- tk";
                    lblGrossPercentageData.Text = row.Cells[6].Value.ToString() + "";
                    double sellprice = NullOrNumber(row.Cells[3].Value.ToString()) - NullOrNumber(row.Cells[10].Value.ToString()) - NullOrNumber(row.Cells[11].Value.ToString());
                    double costprice = NullOrNumber(row.Cells[4].Value.ToString());
                    if (costprice == 0)
                    {
                        lblNetPercentage.Text = "0.00 %"; //Avoiding divide by zero error
                    }
                    else
                    {
                        lblNetPercentage.Text = (((sellprice - costprice) / costprice) * 100).ToString("0.00") + " %";
                    }
                }
                else
                {
                    row.Visible = false;
                }
            }
        }
    }
}
