using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class BusinessValuation : Form
    {
        public BusinessValuation()
        {
            InitializeComponent();
        }

        private double NullOrEmpty(string value)
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

        private void LoadDefaults()
        {
            lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE id=1");
            lblDate.Text = DateTime.Now.ToString("F");

            //Stock Valuation
            DatabaseConnection db = new DatabaseConnection();
            string query = "SELECT Remains, PurchaseRate FROM StockInfo";
            var result = db.Retrieve(query);
            double totalStockValue = 0;
            if (result.HasRows)
            {
                while (result.Read())
                {
                    double remains = NullOrEmpty(result["Remains"].ToString());
                    double purchaseRate = NullOrEmpty(result["PurchaseRate"].ToString());
                    totalStockValue += remains * purchaseRate;
                }
                result.Close();
                db.CloseConnection();
                lblStockValueation.Text += totalStockValue.ToString() + "/-";
            }
            else
            {
                MessageBox.Show("No data found in StockInfo table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Customer Due
            query = "SELECT Sum(Due) FROM statementcustomerdue";
            double customerDue = NullOrEmpty(new InformationRetriever().SingleDataGetter(query));
            lblCustomerDue.Text += customerDue.ToString() + "/-";

            //Local Balance
            query = "SELECT LocalBalance FROM accountbalance WHERE ID=1";
            double localBalance = NullOrEmpty(new InformationRetriever().SingleDataGetter(query));
            lblLocalBalance.Text += localBalance.ToString() + "/-";

            //Foreign Balance
            query = "SELECT ForeignBalance FROM accountbalance WHERE ID=1";
            double foreignBalance = NullOrEmpty(new InformationRetriever().SingleDataGetter(query));
            lblForeignBalance.Text += foreignBalance.ToString() + "/-";

            //Total Balance
            double totalBalance = localBalance + foreignBalance;
            lblTotalBalance.Text += totalBalance.ToString() + "/-";

            //Final Valuation
            double finalValuation = totalStockValue + customerDue + totalBalance;
            lblBusinessValuation.Text += finalValuation.ToString() + "/-";

            //Convert to Words
            string finalValuationInWords = new ConvertNumberToWords().ConvertNumberToWord(finalValuation);
            lblInWords.Text += finalValuationInWords + " Tk Only";
        }

        private void BusinessValuation_Load(object sender, EventArgs e)
        {
            new AccountBalance().LoadDefaults();
            LoadDefaults();
        }
    }
}
