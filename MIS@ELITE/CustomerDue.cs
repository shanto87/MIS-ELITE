using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class CustomerDue : Form
    {
        public CustomerDue()
        {
            InitializeComponent();
        }

        public void CalculateDues()
        {
            try
            {
                cbClientIdentifier.Enabled = false;
                tbSearch.Enabled = false;
                btnSearch.Enabled = false;

                DatabaseConnection db = new DatabaseConnection();
                string truncateQuery = "TRUNCATE TABLE statementcustomerdue"; //table empty command
                db.ExecuteWithoutAlert(truncateQuery, true);

                // Step 1: Calculate dues and populate the statementcustomerdue table
                string query = $"INSERT INTO statementcustomerdue (ClientIdentifier, OrderTotal, PaymentTotal, DiscountTotal, Due) " +
                    $"SELECT c.clientIdentifier," +
                    $"COALESCE((SELECT SUM(NetTotal) FROM Salesdata WHERE Clientidentifier = c.clientIdentifier),0 ) as OrderTotal," +
                    $"COALESCE((SELECT SUM(Paid_Amount) FROM Transaction WHERE Client_identifier = c.clientIdentifier),0) as PaymentTotal," +
                    $"COALESCE((SELECT SUM(Amount) FROM Discount WHERE Clientidentifier = c.clientIdentifier),0) as DiscountTotal," +
                    $"GREATEST(COALESCE((SELECT SUM(NetTotal) FROM Salesdata WHERE Clientidentifier = c.clientIdentifier),0)" +
                    $"-COALESCE((SELECT SUM(Paid_Amount) FROM Transaction WHERE Client_identifier = c.clientIdentifier),0)" +
                    $"-COALESCE((SELECT SUM(Amount) FROM Discount WHERE Clientidentifier = c.clientIdentifier),0),0) as Due " +
                    $"FROM customerinformation c ON DUPLICATE KEY " +
                    $"UPDATE OrderTotal = VALUES(OrderTotal), PaymentTotal = VALUES(PaymentTotal), DiscountTotal = VALUES(DiscountTotal)," +
                    $"Due = VALUES(Due);";
                db.ExecuteWithoutAlert(query, true);
                db.CloseConnection();

                // Step 2: Populate the DataGridView
                DatabaseConnection databaseConnection = new DatabaseConnection();
                string query2 = "SELECT * FROM statementcustomerdue ORDER BY ClientIdentifier";
                db.DataGridViewPopulate(query2, dgvCustomerDue);
                db.CloseConnection();

                //step 3: Set controls to default values
                cbClientIdentifier.Enabled = true;
                tbSearch.Enabled = true;
                btnSearch.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDefaults()
        {
            try
            {
                // Set default values for controls
                cbClientIdentifier.Items.Clear();
                cbClientIdentifier.Text = "";
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("Select CompanyName From companyinfo where id=1");
                label1.Text = "ALL TRANSACTIONS ";
                lblCustomerName.Text = "Customer Name :";
                lblCustomerAddress.Text = "Customer Address :";
                lblCustomerPhone.Text = "Customer Phone :";
                lblLastTransaction.Text = "Last Transaction :";
                lblToAccount.Text = "To Account :";
                lblKeyword.Text = "Keyword :";
                lblDate.Text = "Date :";
                lblCustomerDue.Text = "Customer Due :";
                lblGrossCustomerDue.Text = "Gross Customer Due ";
                tbSearch.Text= "";

                //clientIdentifier combobox populate
                DatabaseConnection databaseConnection = new DatabaseConnection();
                string query = "SELECT clientIdentifier FROM customerinformation ORDER BY Clientidentifier ASC";
                var result = databaseConnection.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        cbClientIdentifier.Items.Add(result.GetValue(0).ToString());
                    }
                    result.Close();
                    databaseConnection.CloseConnection();
                }

                dgvLastTransactions.DataSource = null; // Clear the DataSource to avoid data binding issues
                dgvLastTransactions.Rows.Clear(); // Clear the DataGridView rows
                dgvLastTransactions.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomerDue_Load(object sender, EventArgs e)
        {
            LoadDefaults();
            CalculateDues();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadDefaults();
            CalculateDues();
        }


        private void cbClientIdentifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //getting customer information
                string query = $"SELECT ClientName, ClientAddress, ClientPhone FROM CustomerInformation WHERE ClientIdentifier='{cbClientIdentifier.Text}'";
                DatabaseConnection dbConnection = new DatabaseConnection();
                string CustomerName = "";
                var result = dbConnection.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        CustomerName = result.GetValue(0).ToString();
                        lblCustomerAddress.Text = "Customer Address : " + result.GetValue(1).ToString();
                        lblCustomerPhone.Text = "Customer Phone : " + result.GetValue(2).ToString();
                    }
                    lblCustomerName.Text = "Customer Name : " + CustomerName;
                    result.Close();
                }
                else
                {
                    CustomerName = "Customer Name : ";
                    lblCustomerAddress.Text = "Customer Address : ";
                    lblCustomerPhone.Text = "Customer Phone : ";
                    MessageBox.Show("No customer information found for the selected identifier.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //getting all payment information
                label1.Text = "All Transactions By - " + CustomerName.ToUpper();
                DatabaseConnection databaseConnection = new DatabaseConnection();
                query = $"SELECT Id,Paid_Amount,Date,TO_Account,KEYWORD from Transaction WHERE Client_Identifier='{cbClientIdentifier.Text}' ORDER BY ID DESC";
                databaseConnection.DataGridViewPopulate(query, dgvLastTransactions);
                databaseConnection.CloseConnection();

                //getting last transaction
                query = $"SELECT Paid_Amount,Date,TO_Account,KEYWORD from Transaction WHERE ID=(SELECT MAX(ID) FROM transaction WHERE Client_Identifier='{cbClientIdentifier.Text}')";
                var lastTransaction = databaseConnection.Retrieve(query);
                if (lastTransaction.HasRows)
                {
                    while (lastTransaction.Read())
                    {
                        lblLastTransaction.Text = "Last Transaction : " + lastTransaction.GetValue(0).ToString() + "/- tk";
                        lblDate.Text = "Date : " + lastTransaction.GetValue(1).ToString();
                        lblToAccount.Text = "To Account : " + lastTransaction.GetValue(2).ToString();
                        lblKeyword.Text = "Keyword : " + lastTransaction.GetValue(3).ToString();
                    }
                    lastTransaction.Close();
                }
                else
                {
                    lblLastTransaction.Text = "Last Transaction : ";
                    lblDate.Text = "Date : ";
                    lblToAccount.Text = "To Account : ";
                    lblKeyword.Text = "Keyword : ";
                    MessageBox.Show("No transaction information found for the selected identifier.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //getting customer due
                lblCustomerDue.Text = "Customer Due : " + new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'") + "/- tk";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomerDue_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                cbClientIdentifier.Text = dgvCustomerDue.Rows[RowIndex].Cells[1].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = tbSearch.Text.Trim();
                dgvCustomerDue.SuspendLayout();

                // Release the focused/selected row so the grid allows hiding it
                dgvCustomerDue.ClearSelection();
                dgvCustomerDue.CurrentCell = null;

                foreach (DataGridViewRow row in dgvCustomerDue.Rows)
                {
                    if (row.IsNewRow)
                        continue; // skip the "add new" row

                    string cellValue = row.Cells[1].Value?.ToString() ?? "";
                    row.Visible = cellValue.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                }

                dgvCustomerDue.ResumeLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvCustomerDue.Rows)
            {
                row.Visible = true;   // show all rows
            }
        }
    }
}