using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class Transactions : Form
    {
        public Transactions()
        {
            InitializeComponent();
        }

        //Custom Functions Start

        private bool isValidInput()
        {
            return !string.IsNullOrEmpty(cbClientIdentifier.Text) &&
                   !string.IsNullOrEmpty(tbPaidAmount.Text) &&
                   !string.IsNullOrEmpty(cbToAccount.Text) &&
                   !string.IsNullOrEmpty(cbKeyword.Text) &&
                   !string.IsNullOrEmpty(tbMonthName.Text);
        }

        private void Defaults()
        {
            lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");

            //month name generation
            HelperClass helperClass = new HelperClass();
            tbMonthName.Text = helperClass.getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);

            //clearing inputs
            tbTxnId.Text = string.Empty;
            lblDue.Text = "Due :";
            lblAnalytics.Text = string.Empty;
            cbClientIdentifier.Text = string.Empty;
            tbClientName.Text = string.Empty;
            tbClientAddress.Text = string.Empty;
            tbClientPhone.Text = string.Empty;
            tbPaidAmount.Text = string.Empty;
            tbNotes.Text = string.Empty;
            cbKeyword.Items.Clear();
            cbMonthNameFilter.Text = string.Empty;
            tbSearch.Text = string.Empty;

            //Update all combo boxes
            ComboBoxUpdater();
        }

        private void LoadDefaults()
        {
            try
            {
                Defaults();
                //data grid view population with data
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * from TRANSACTION ORDER BY ID DESC", dgvTransaction);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FastLoadDefaults()
        {
            try
            {
                Defaults();
                //data grid view population with data
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * from TRANSACTION ORDER BY ID DESC LIMIT 50", dgvTransaction);
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBoxUpdater()
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = "SELECT ClientIdentifier FROM Customerinformation ORDER BY ClientIdentifier";
                var result = db.Retrieve(query);
                cbClientIdentifier.Items.Clear();
                while (result.Read())
                {
                    cbClientIdentifier.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
                db.CloseConnection();

                query = "SELECT accountname FROM ACCOUNTS GROUP BY accountname ORDER BY accountname ASC";
                result = db.Retrieve(query);
                cbToAccount.Items.Clear();
                while (result.Read())
                {
                    cbToAccount.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
                db.CloseConnection();

                query = "SELECT Month_Name FROM transaction GROUP BY Month_Name ORDER BY Month_Name ASC";
                result = db.Retrieve(query);
                cbMonthNameFilter.Items.Clear();
                while (result.Read())
                {
                    cbMonthNameFilter.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Custom Functions End

        private void Transactions_Load(object sender, EventArgs e)
        {
            FastLoadDefaults();
        }

        private void cbClientIdentifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                var result = db.Retrieve("SELECT * FROM CustomerInformation WHERE ClientIdentifier='" + cbClientIdentifier.Text.ToString() + "'");
                while (result.Read())
                {
                    tbClientName.Text = result.GetValue(1).ToString();
                    tbClientAddress.Text = result.GetValue(2).ToString();
                    tbClientPhone.Text = result.GetValue(3).ToString();
                }
                result.Close();
                db.CloseConnection();

                //getting customer due
                string due = new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'") ?? "0";
                lblDue.Text = "Due : " + due;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbToAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                var result = db.Retrieve("SELECT KEYWORD FROM ACCOUNTS WHERE ACCOUNTNAME='" + cbToAccount.Text.ToString() + "' ORDER BY KEYWORD ASC");
                cbKeyword.Items.Clear();
                while (result.Read())
                {
                    cbKeyword.Items.Add(result.GetString(0));
                }
                result.Close();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                HelperClass helperClass = new HelperClass();
                tbMonthName.Text = helperClass.getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Sure to proceed?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return; // User chose not to proceed
                }
                if (isValidInput())
                {
                    string due = new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'") ?? "0";
                    if (Convert.ToDouble(tbPaidAmount.Text) > Convert.ToDouble(due))
                    {
                        MessageBox.Show("Paid amount cannot be greater than due amount.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DatabaseConnection databaseConnection = new DatabaseConnection();
                    string query = "INSERT INTO TRANSACTION VALUES('','" + cbClientIdentifier.Text.ToString() + "','" + dtpDate.Text.ToString() + "','" +
                        tbPaidAmount.Text.ToString() + "','" + cbToAccount.Text.ToString() + "','" + cbKeyword.Text.ToString() + "','" + tbMonthName.Text.ToString() +
                        "','" + tbNotes.Text.ToString() + "','')";
                    databaseConnection.Execute(query, true);
                    databaseConnection.CloseConnection();
                    LoadDefaults();
                    CustomerDue customerDue = new CustomerDue();
                    customerDue.CalculateDues();
                }
                else
                {
                    MessageBox.Show("Input all data correctly.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbTxnId.Text))
                {
                    MessageBox.Show("Transaction ID cannot be empty.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DatabaseConnection databaseconnection = new DatabaseConnection();
                string query = "SELECT * FROM TRANSACTION WHERE ID='" + tbTxnId.Text.ToString() + "'";
                var result = databaseconnection.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        cbClientIdentifier.Text = result.GetValue(1).ToString();
                        dtpDate.Text = result.GetValue(2).ToString();
                        tbPaidAmount.Text = result.GetValue(3).ToString();
                        cbToAccount.Text = result.GetValue(4).ToString();
                        cbKeyword.Text = result.GetValue(5).ToString();
                        tbMonthName.Text = result.GetValue(6).ToString();
                        tbNotes.Text = result.GetValue(7).ToString();
                    }
                    result.Close();
                    databaseconnection.CloseConnection();
                }
                else
                {
                    MessageBox.Show("Transaction ID not found.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("ARE YOU SURE TO DELETE?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    if (tbTxnId.Text != string.Empty)
                    {
                        DatabaseConnection databaseConnection = new DatabaseConnection();
                        string query = "DELETE FROM TRANSACTION WHERE ID='" + tbTxnId.Text.ToString() + "'";
                        databaseConnection.Execute(query, true);
                        databaseConnection.CloseConnection();
                        LoadDefaults();
                        CustomerDue customerDue = new CustomerDue();
                        customerDue.CalculateDues();
                    }
                    else
                    {
                        MessageBox.Show("Transaction ID cannot be empty.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Sure to proceed?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return; // User chose not to proceed
                }
                if (isValidInput() && tbTxnId.Text != string.Empty)
                {
                    DatabaseConnection databaseConnection = new DatabaseConnection();
                    string query = "UPDATE TRANSACTION SET CLIENT_IDENTIFIER='" + cbClientIdentifier.Text.ToString() + "', DATE='" +
                        dtpDate.Text.ToString() + "', PAID_AMOUNT='" + tbPaidAmount.Text.ToString() + "', TO_ACCOUNT='" + cbToAccount.Text.ToString()
                        + "', KEYWORD='" + cbKeyword.Text.ToString() + "', MONTH_NAME='" + tbMonthName.Text.ToString() + "', NOTES='" + tbNotes.Text.ToString()
                        + "' WHERE ID='" + tbTxnId.Text.ToString() + "'";
                    databaseConnection.Execute(query, true);
                    databaseConnection.CloseConnection();
                    LoadDefaults();
                    CustomerDue customerDue = new CustomerDue();
                    customerDue.CalculateDues();
                }
                else
                {
                    MessageBox.Show("Input all data correctly.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbTxnId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFetch.PerformClick();
                e.SuppressKeyPress = true; // Prevents the beep sound on Enter key press
            }
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            string query;
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                if (string.IsNullOrEmpty(cbMonthNameFilter.Text))
                {
                    query = $"SELECT * from TRANSACTION WHERE Client_Identifier LIKE '%{tbSearch.Text}%' OR Paid_Amount LIKE '%{tbSearch.Text}%' OR To_Account LIKE '%{tbSearch.Text}%' OR Keyword LIKE '%{tbSearch.Text}%' OR Month_Name LIKE '%{tbSearch.Text}%' ORDER BY ID DESC";
                }
                else
                {
                    query = $"SELECT * from TRANSACTION WHERE (Client_Identifier LIKE '%{tbSearch.Text}%' OR Paid_Amount LIKE '%{tbSearch.Text}%' OR To_Account LIKE '%{tbSearch.Text}%' OR Keyword LIKE '%{tbSearch.Text}%') AND Month_Name='{cbMonthNameFilter.Text}' ORDER BY ID DESC";
                }
                db.DataGridViewPopulate(query, dgvTransaction);
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbMonthNameFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * from TRANSACTION where Month_Name='" + cbMonthNameFilter.Text.ToString() + "' ORDER BY ID DESC", dgvTransaction);
                db.CloseConnection();
                string amountReceived = new InformationRetriever().SingleDataGetter($"SELECT SUM(Paid_Amount) FROM TRANSACTION WHERE Month_Name='{cbMonthNameFilter.Text}'");
                lblAnalytics.Text = $"Total Amount Received in {cbMonthNameFilter.Text} : {amountReceived}/-";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void dgvTransaction_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int rowIndex = e.RowIndex;
                tbTxnId.Text = dgvTransaction.Rows[rowIndex].Cells[0].Value.ToString();
                btnFetch.PerformClick(); // Fetch the transaction details based on the ID
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}