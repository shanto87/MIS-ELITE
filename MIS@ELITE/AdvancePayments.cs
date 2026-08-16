using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class AdvancePayments : Form
    {
        public AdvancePayments()
        {
            InitializeComponent();
        }

        private void LoadDefaults()
        {
            try
            {
                //Default Value
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");
                lblDue.Text = "Due :";
                tbClientName.Text = string.Empty;
                tbClientAddress.Text = string.Empty;
                tbClientPhone.Text = string.Empty;
                tbPaidAmount.Text = string.Empty;
                dtpDate.Value = DateTime.Now;
                tbNotes.Text = string.Empty;
                cbToAccount.Items.Clear();
                cbMonthNameFilter.Text = string.Empty;
                cbKeyword.Items.Clear();
                tbTxnId.Text = string.Empty;
                tbSearch.Text = string.Empty;
                cbClientIdentifier.Text = string.Empty;

                //ClientIdentifier ComboBox Populate
                DatabaseConnection db = new DatabaseConnection();
                string query = "SELECT ClientIdentifier FROM CustomerInformation";
                cbClientIdentifier.Items.Clear();
                var result = db.Retrieve(query);
                while (result.Read())
                {
                    cbClientIdentifier.Items.Add(result["ClientIdentifier"].ToString());
                }
                result.Close();
                db.CloseConnection();

                //ToAccount ComboBox Populate
                query = "SELECT accountname FROM ACCOUNTS GROUP BY accountname ORDER BY accountname ASC";
                result = db.Retrieve(query);
                while (result.Read())
                {
                    cbToAccount.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
                db.CloseConnection();

                //month name generattion
                HelperClass helperClass = new HelperClass();
                tbMonthName.Text = helperClass.getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);

                //MonthName ComboBox Populate
                query = "SELECT DISTINCT MonthName FROM AdvancePayment ORDER BY MONTHNAME ASC";
                result = db.Retrieve(query);
                cbMonthNameFilter.Items.Clear();
                while (result.Read())
                {
                    cbMonthNameFilter.Items.Add(result["MonthName"].ToString());
                }
                result.Close();
                db.CloseConnection();

                //DatagridView Populate
                query = "SELECT * FROM AdvancePayment ORDER BY ID DESC";
                db.DataGridViewPopulate(query, dgvAdvancePayment);

                //Gross Advance Payments DatagridView Populate
                query = "SELECT ClientIdentifier, SUM(AdvanceAmount) AS TOTAL_ADVANCE FROM AdvancePayment GROUP BY ClientIdentifier ORDER BY TOTAL_ADVANCE DESC";
                db.DataGridViewPopulate(query, dgvGrossAdvancePayment);

                //Customer due calculation after advance payment settlements
                new CustomerDue().CalculateDues();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdvancePayments_Load(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void cbClientIdentifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT ClientName, ClientAddress, ClientPhone FROM CustomerInformation WHERE ClientIdentifier='{cbClientIdentifier.Text}'";
                var result = db.Retrieve(query);
                while (result.Read())
                {
                    tbClientName.Text = result["ClientName"].ToString();
                    tbClientAddress.Text = result["ClientAddress"].ToString();
                    tbClientPhone.Text = result["ClientPhone"].ToString();
                }
                result.Close();
                db.CloseConnection();

                //Check if client has any due
                lblDue.Text = "Due : " + new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbToAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                var result = db.Retrieve($"SELECT KEYWORD FROM ACCOUNTS WHERE ACCOUNTNAME='{cbToAccount.Text}' ORDER BY KEYWORD ASC");
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            HelperClass helperClass = new HelperClass();
            tbMonthName.Text = helperClass.getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);
        }

        private void btnPartialClear_Click(object sender, EventArgs e)
        {
            tbPartialPaid.Text = string.Empty;
            tbPartialTxnId.Text = string.Empty;
            lblPartialDue.Text = "";
            dtpPartialPaidDate.Value = DateTime.Now;
        }

        private bool IsValid()
        {
            if (string.IsNullOrEmpty(tbClientName.Text))
            {
                MessageBox.Show("Please select a client identifier.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(tbPaidAmount.Text))
            {
                MessageBox.Show("Please enter the paid amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(cbToAccount.Text))
            {
                MessageBox.Show("Please select an account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(cbKeyword.Text))
            {
                MessageBox.Show("Please select a keyword.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(tbMonthName.Text))
            {
                MessageBox.Show("Please Input Month Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
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
                if (IsValid())
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = "INSERT INTO advancepayment VALUES('','" + cbClientIdentifier.Text + "','" + tbPaidAmount.Text + "','" +
                        cbToAccount.Text + "','" + cbKeyword.Text + "','" + dtpDate.Text + "','" + tbMonthName.Text + "','" + tbNotes.Text + "')";
                    db.Execute(query, true);
                    db.CloseConnection();
                    LoadDefaults();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (string.IsNullOrEmpty(tbTxnId.Text))
                {
                    MessageBox.Show("Please enter a transaction ID to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (IsValid())
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"UPDATE AdvancePayment SET ClientIdentifier='{cbClientIdentifier.Text}', AdvanceAmount='{tbPaidAmount.Text}', " +
                        $"ToAccount='{cbToAccount.Text}', Keyword='{cbKeyword.Text}', Date='{dtpDate.Text}', MonthName='{tbMonthName.Text}', Notes='{tbNotes.Text}' " +
                        $"WHERE ID='{tbTxnId.Text}'";
                    db.Execute(query, true);
                    db.CloseConnection();
                    LoadDefaults();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbTxnId.Text == string.Empty)
                {
                    MessageBox.Show("Please enter a transaction ID to fetch.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT * FROM AdvancePayment WHERE ID='{tbTxnId.Text}'";
                var result = db.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        cbClientIdentifier.Text = result["ClientIdentifier"].ToString();
                        tbPaidAmount.Text = result["AdvanceAmount"].ToString();
                        cbToAccount.Text = result["ToAccount"].ToString();
                        cbKeyword.Text = result["Keyword"].ToString();
                        dtpDate.Text = result["Date"].ToString();
                        tbMonthName.Text = result["MonthName"].ToString();
                        tbNotes.Text = result["Notes"].ToString();
                    }
                    result.Close();
                    db.CloseConnection();
                }
                else
                {
                    MessageBox.Show("No record found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DatabaseConnection db = new DatabaseConnection();
            string query = $"DELETE FROM AdvancePayment WHERE ID='{tbTxnId.Text}'";
            if (tbTxnId.Text == string.Empty)
            {
                MessageBox.Show("Please enter a transaction ID to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Are you sure to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                db.Execute(query, true);
                db.CloseConnection();
                LoadDefaults();
                ;
            }
        }

        private void cbMonthNameFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate($"SELECT * from AdvancePayment WHERE MonthName='{cbMonthNameFilter.Text}' ORDER BY ID DESC", dgvAdvancePayment);
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Sure to proceed?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return; // User chose not to proceed
                }
                if (string.IsNullOrEmpty(tbTxnId.Text))
                {
                    MessageBox.Show("Please enter a transaction ID to transfer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DatabaseConnection db = new DatabaseConnection();
                btnFetch.PerformClick();
                if (cbClientIdentifier.Text == string.Empty)
                {
                    //data fetched but still empty, no error shown as it is already handled in btnFetch
                    return;
                }
                else
                {
                    //data fetched and not empty
                    string txnId = tbTxnId.Text;
                    string ClientName = cbClientIdentifier.Text;
                    string amount = tbPaidAmount.Text;
                    if (Convert.ToDouble(amount) < 1)
                    {
                        MessageBox.Show("Advance Amount is already settled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string due = new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'");
                    if (Convert.ToDouble(amount) > Convert.ToDouble(due))
                    {
                        MessageBox.Show($"Advance amount is higher than Due, Use partial payment for settlement", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string toAccount = cbToAccount.Text;
                    string keyword = cbKeyword.Text;
                    string notes = tbNotes.Text;
                    string monthName = new HelperClass().getFullMonthName(DateTime.Now.Month, DateTime.Now.Year);

                    //Insert into Transaction Table from AdvancePayment Table
                    string query2 = $"INSERT INTO transaction VALUES('','{ClientName}','{DateTime.Now:dddd, MMMM d, yyyy}','{amount}','{toAccount}'," +
                        $"'{keyword}','{monthName}','Advance Payment Settled, TXN ID={txnId}','')";
                    db.ExecuteWithoutAlert(query2, true);
                    db.CloseConnection();

                    //get maximum transaction id
                    string query = "SELECT MAX(ID) FROM transaction";
                    InformationRetriever informationRetriever = new InformationRetriever();
                    string maxTransactionId = informationRetriever.SingleDataGetter(query);

                    //Update AdvancePayment Table
                    query = $"UPDATE advancepayment SET AdvanceAmount='0', Notes='{notes} [{amount}/-tk Transferred to Transaction Table ID={maxTransactionId} on Date: {DateTime.Now:dddd, MMMM d,yyyy}]' WHERE ID='{tbTxnId.Text}'";
                    db.ExecuteWithoutAlert(query, true);
                    db.CloseConnection();

                    MessageBox.Show("Transaction Completed Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDefaults();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPartialSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Sure to proceed?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return; // User chose not to proceed
                }
                if (string.IsNullOrEmpty(tbPartialTxnId.Text))
                {
                    MessageBox.Show("Please enter a transaction ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DatabaseConnection databaseConnection = new DatabaseConnection();
                string query = "SELECT AdvanceAmount, Notes FROM AdvancePayment WHERE ID='" + tbPartialTxnId.Text + "'";
                var result = databaseConnection.Retrieve(query);
                string advanceAmount = "";
                string notes = "";
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        advanceAmount = result["AdvanceAmount"].ToString();
                        notes = result["Notes"].ToString();
                    }
                    result.Close();
                    databaseConnection.CloseConnection();
                    if (advanceAmount == "0")
                    {
                        MessageBox.Show("This transaction has already been settled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        double remainingAmount = Convert.ToDouble(advanceAmount) - Convert.ToDouble(tbPartialPaid.Text);
                        if (remainingAmount < 0)
                        {
                            MessageBox.Show("Partial payment amount exceeds the remaining advance amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        tbTxnId.Text = tbPartialTxnId.Text;
                        btnFetch.PerformClick();


                        string currentDue = new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'");
                        if (Convert.ToDouble(tbPartialPaid.Text) > Convert.ToDouble(currentDue))
                        {
                            MessageBox.Show($"Partial Payment Exceed Due Amount. Customer Due= {currentDue}tk, Request parital payment= {tbPartialPaid.Text}tk", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        //Inserting to Transaction Table
                        query = $"INSERT INTO transaction VALUES('','{cbClientIdentifier.Text}','{dtpPartialPaidDate.Text}','{tbPartialPaid.Text}','{cbToAccount.Text}'," +
                            $"'{cbKeyword.Text}','{DateTime.Now:Y}','Partial Advance Payment Settled, TXN Id={tbPartialTxnId.Text}','')";
                        databaseConnection.ExecuteWithoutAlert(query, true);
                        databaseConnection.CloseConnection();

                        //Get Maximum Transaction ID
                        query = "SELECT MAX(ID) FROM transaction";
                        InformationRetriever informationRetriever = new InformationRetriever();
                        string maxTransactionId = informationRetriever.SingleDataGetter(query);


                        //Updating AdvancePayment Table
                        query = $"UPDATE AdvancePayment SET AdvanceAmount='{remainingAmount}', Notes='{notes} [Partial Payment {tbPartialPaid.Text} tk on {dtpPartialPaidDate.Value:d}, ID={maxTransactionId}]' WHERE ID='{tbPartialTxnId.Text}'";
                        databaseConnection.ExecuteWithoutAlert(query, true);
                        databaseConnection.CloseConnection();

                        MessageBox.Show("Transaction Completed Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDefaults();
                        btnPartialClear.PerformClick();
                    }
                }
                else
                {
                    MessageBox.Show("No record found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbPartialTxnId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string clientIdentifier = new InformationRetriever().SingleDataGetter($"SELECT ClientIdentifier FROM AdvancePayment WHERE ID='{tbPartialTxnId.Text}'");
                string due = new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{clientIdentifier}'");
                if (clientIdentifier == string.Empty)
                {
                    lblPartialDue.Text = "";
                    return;
                }
                lblPartialDue.Text = $"{clientIdentifier} # Due : {due}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbTxnId_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvAdvancePayment_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                int ID = Convert.ToInt32(dgvAdvancePayment.Rows[RowIndex].Cells[0].Value);
                tbTxnId.Text = ID.ToString();
                btnFetch.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                try
                {
                    string query = "";
                    DatabaseConnection db = new DatabaseConnection();
                    if (string.IsNullOrEmpty(cbMonthNameFilter.Text))
                    {
                        query = $"SELECT * from AdvancePayment WHERE ClientIdentifier LIKE '%{tbSearch.Text}%' OR AdvanceAmount LIKE '%{tbSearch.Text}%' OR ToAccount LIKE '%{tbSearch.Text}%' OR Keyword LIKE '%{tbSearch.Text}%' OR MonthName LIKE '%{tbSearch.Text}%' ORDER BY ID DESC";
                    }
                    else
                    {
                        query = $"SELECT * from AdvancePayment WHERE (ClientIdentifier LIKE '%{tbSearch.Text}%' OR AdvanceAmount LIKE '%{tbSearch.Text}%' OR ToAccount LIKE '%{tbSearch.Text}%' OR Keyword LIKE '%{tbSearch.Text}%') AND MonthName='{cbMonthNameFilter.Text}' ORDER BY ID DESC";
                    }
                    db.DataGridViewPopulate(query, dgvAdvancePayment);
                    db.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}