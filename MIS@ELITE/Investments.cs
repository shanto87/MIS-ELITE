using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class Investments : Form
    {
        public Investments()
        {
            InitializeComponent();
        }

        private void ComboBoxUpdater()
        {
            DatabaseConnection db = new DatabaseConnection();
            string query = "";
            //CbfromAccount Combo Box Updater
            cbFromAccount.Items.Clear();
            query = "SELECT accountname FROM ACCOUNTS GROUP BY accountname ORDER BY accountname ASC";
            var result = db.Retrieve(query);
            while (result.Read())
            {
                cbFromAccount.Items.Add(result.GetValue(0).ToString());
            }
            result.Close();
            db.CloseConnection();

            //To Foreign Account Combo Box Updater
            cbToForeignAccount.Items.Clear();
            query = "SELECT accountname FROM ACCOUNTSFOREIGN GROUP BY accountname ORDER BY accountname ASC";
            var result1 = db.Retrieve(query);
            while (result1.Read())
            {
                cbToForeignAccount.Items.Add(result1.GetValue(0).ToString());
            }
            result1.Close();
            db.CloseConnection();

            //MonthName Filter Combo box updater
            cbMonthNameFilter.Items.Clear();
            cbMonthNameForeign.Items.Clear();
            query = "SELECT DISTINCT MONTHNAME FROM Investment ORDER BY MonthName ASC";
            result = db.Retrieve(query);
            while (result.Read())
            {
                cbMonthNameFilter.Items.Add(result.GetValue(0).ToString());
                cbMonthNameForeign.Items.Add(result.GetValue(0).ToString());
            }
            result.Close();
            db.CloseConnection();
        }

        private void Defaults()
        {
            // Load default values for the form controls
            lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");
            tbTxnId.Text = string.Empty;
            tbClientName.Text = string.Empty;
            tbClientAddress.Text = string.Empty;
            tbClientPhone.Text = string.Empty;
            tbBankName.Text = string.Empty;
            tbBankAccountNumber.Text = string.Empty;
            tbBranchName.Text = string.Empty;
            tbAmount.Text = string.Empty;
            dtpDate.Value = DateTime.Now;
            cbFromAccount.Text = "";
            cbKeyword.Items.Clear();
            cbKeyword.Text = "";
            cbToForeignAccount.Items.Clear();
            cbToForeignAccount.Text = string.Empty;
            tbMonthName.Text = new HelperClass().getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);
            tbNotes.Text = string.Empty;
            tbSearch.Text = string.Empty;
            cbMonthNameFilter.Text = "";
            cbMonthNameForeign.Text = "";
            lblAnalytics.Text = string.Empty;
            lblForeignAnalytics.Text = string.Empty;
        }

        private void LoadDefaults()
        {
            try
            {
                Defaults();
                //Default values for DataGridView
                DatabaseConnection db = new DatabaseConnection();
                string query = "SELECT * FROM Investment ORDER BY ID DESC";
                db.DataGridViewPopulate(query, dgvInvestment);
                db.CloseConnection();

                //Foreign Investment DataGridView
                db.DataGridViewPopulate("SELECT * FROM InvestmentForeign ORDER BY ID DESC", dgvInvestmentForeign);

                //Update Combo Boxes
                ComboBoxUpdater();

                //Calculation after all default value load
                InvestmentBalanceCalculate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FastLoadDefaults()
        {
            try
            {
                Defaults();
                //Default values for DataGridView
                DatabaseConnection db = new DatabaseConnection();
                string query = "SELECT * FROM Investment ORDER BY ID DESC LIMIT 50";
                db.DataGridViewPopulate(query, dgvInvestment);
                db.CloseConnection();

                //Foreign Investment DataGridView
                db.DataGridViewPopulate("SELECT * FROM InvestmentForeign ORDER BY ID DESC LIMIT 50", dgvInvestmentForeign);

                //Update Combo Boxes
                ComboBoxUpdater();

                //Calculation after all default value load
                InvestmentBalanceCalculate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValid()
        {
            // Validate the input fields
            if (string.IsNullOrEmpty(tbClientName.Text))
            {
                MessageBox.Show("Please enter Client Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(tbBankName.Text))
            {
                MessageBox.Show("Please enter Bank Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(tbBankAccountNumber.Text))
            {
                MessageBox.Show("Please enter Bank Account Number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(tbAmount.Text))
            {
                MessageBox.Show("Please enter Amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(cbFromAccount.Text))
            {
                MessageBox.Show("Please select From Account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(cbKeyword.Text))
            {
                MessageBox.Show("Please select Keyword.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(cbToForeignAccount.Text))
            {
                MessageBox.Show("Please select Receiver Foreign Account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }


        private void Investments_Load(object sender, EventArgs e)
        {
            FastLoadDefaults();
        }

        public void InvestmentBalanceCalculate()
        {
            try
            {
                dgvForeignCurrencyTotal.DataSource = null;
                dgvForeignCurrencyTotal.Rows.Clear();
                dgvForeignCurrencyTotal.Refresh();

                //Function to check if value is empty
                double DigitValidation(string value)
                {
                    return string.IsNullOrEmpty(value) ? 0 : Convert.ToDouble(value);
                }

                // Calculate the sum of the investment      
                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT DISTINCT AccountName From AccountsForeign ORDER BY AccountName ASC";
                var result = db.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        dgvForeignCurrencyTotal.Rows.Add(dgvForeignCurrencyTotal.Rows.Count + 1, result.GetValue(0).ToString(), "", "", "");
                    }
                    result.Close();
                    db.CloseConnection();
                }
                else
                {
                    MessageBox.Show("No records found in the foreign investment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                foreach (DataGridViewRow row in dgvForeignCurrencyTotal.Rows)
                {
                    string accountName = row.Cells[1].Value.ToString();
                    string query1 = $"SELECT SUM(Amount) FROM InvestmentForeign WHERE FromAccount='{accountName}'";
                    double InvestmentReceivedTotal = DigitValidation(new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Investment WHERE ToForeignAccount='{accountName}'"));
                    double InvestmentInvestedTotal = DigitValidation(new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM InvestmentForeign WHERE FromAccount='{accountName}'"));
                    double InvestmentExpenseTotal = DigitValidation(new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Expense WHERE FromAccount='{accountName}'"));
                    double Balance = InvestmentReceivedTotal - InvestmentInvestedTotal - InvestmentExpenseTotal;
                    row.Cells[2].Value = InvestmentReceivedTotal;
                    row.Cells[3].Value = InvestmentInvestedTotal;
                    row.Cells[4].Value = InvestmentExpenseTotal;
                    row.Cells[5].Value = Balance;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbFromAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                var result = db.Retrieve($"SELECT KEYWORD FROM ACCOUNTS WHERE ACCOUNTNAME='{cbFromAccount.Text}' ORDER BY KEYWORD ASC");
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
            if (CheckBoxForeignInvest.Checked)
            {
                tbClientName.Text = string.Empty;
                tbAmount.Text = string.Empty;
                dtpDate.Value = DateTime.Now;
                cbFromAccount.Items.Clear();
                cbFromAccount.Text = "";

                //from account combobox populate
                cbFromAccount.Items.Clear();
                string tablename = CheckBoxForeignInvest.Checked ? "ACCOUNTSForeign" : "ACCOUNTS";
                string query = $"SELECT accountname FROM {tablename} GROUP BY accountname ORDER BY accountname ASC";
                var result = new DatabaseConnection().Retrieve(query);
                while (result.Read())
                {
                    cbFromAccount.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
            }
            else
            {
                LoadDefaults();
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
                if (CheckBoxForeignInvest.Checked)
                {
                    if (tbClientName.Text != string.Empty && tbAmount.Text != string.Empty && cbFromAccount.Text != string.Empty)
                    {
                        DatabaseConnection db = new DatabaseConnection();
                        string query = $"INSERT INTO InvestmentForeign VALUES ('', '{tbClientName.Text}','{tbAmount.Text}', '{dtpDate.Text}','{cbFromAccount.Text}'," +
                            $"'{tbMonthName.Text}', '{tbNotes.Text}')";
                        db.Execute(query, true);
                        db.DataGridViewPopulate($"SELECT * FROM InvestmentForeign ORDER BY ID DESC", dgvInvestmentForeign);
                        InvestmentBalanceCalculate();
                        db.CloseConnection();
                    }
                    else
                    {
                        MessageBox.Show("Please fill all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (IsValid())
                    {
                        DatabaseConnection databaseConnection = new DatabaseConnection();
                        string query = $"INSERT INTO Investment VALUES ('', '{tbClientName.Text}', '{tbClientAddress.Text}', '{tbClientPhone.Text}'," +
                            $"'{tbBankName.Text}','{tbBankAccountNumber.Text}', '{tbBranchName.Text}', '{tbAmount.Text}', '{dtpDate.Text}', " +
                            $"'{cbFromAccount.Text}', '{cbKeyword.Text}','{cbToForeignAccount.Text}', '{tbMonthName.Text}', '{tbNotes.Text}')";
                        databaseConnection.Execute(query, true);
                        databaseConnection.CloseConnection();
                        LoadDefaults();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            tbMonthName.Text = new HelperClass().getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);
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
                if (CheckBoxForeignInvest.Checked)
                {
                    if (tbTxnId.Text != string.Empty && tbClientName.Text != string.Empty && tbAmount.Text != string.Empty && cbFromAccount.Text != string.Empty)
                    {
                        DatabaseConnection db = new DatabaseConnection();
                        string query = $"UPDATE InvestmentForeign SET Details='{tbClientName.Text}', Amount='{tbAmount.Text}', " +
                            $"Date='{dtpDate.Text}', FromAccount='{cbFromAccount.Text}', MonthName='{tbMonthName.Text}', Notes='{tbNotes.Text}' WHERE ID='{tbTxnId.Text}'";
                        db.Execute(query, true);
                        db.DataGridViewPopulate($"SELECT * FROM InvestmentForeign ORDER BY ID DESC", dgvInvestmentForeign);
                        InvestmentBalanceCalculate();
                        db.CloseConnection();
                    }
                    else
                    {
                        MessageBox.Show("Please fill all required fields to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(tbTxnId.Text))
                    {
                        MessageBox.Show("Please enter a Transaction ID to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (IsValid())
                    {
                        DatabaseConnection databaseConnection = new DatabaseConnection();
                        string query = $"UPDATE Investment SET ClientName='{tbClientName.Text}', ClientAddress='{tbClientAddress.Text}', " +
                            $"ClientPhone='{tbClientPhone.Text}', BankName='{tbBankName.Text}', BankAccountNumber='{tbBankAccountNumber.Text}', " +
                            $"BranchDetails='{tbBranchName.Text}', Amount='{tbAmount.Text}', Date='{dtpDate.Text}', FromAccount='{cbFromAccount.Text}', " +
                            $"Keyword='{cbKeyword.Text}',ToForeignAccount='{cbToForeignAccount.Text}', MonthName='{tbMonthName.Text}', Notes='{tbNotes.Text}' WHERE ID='{tbTxnId.Text}'";
                        databaseConnection.Execute(query, true);
                        databaseConnection.CloseConnection();
                        LoadDefaults();
                    }
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
                if (string.IsNullOrWhiteSpace(tbTxnId.Text))
                {
                    MessageBox.Show("Please enter a Transaction ID to fetch the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (CheckBoxForeignInvest.Checked)
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"SELECT * FROM InvestmentForeign WHERE ID='{tbTxnId.Text}'";
                    var result = db.Retrieve(query);
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            tbClientName.Text = result.GetString(1);
                            tbAmount.Text = result.GetString(2);
                            dtpDate.Text = result.GetString(3);
                            cbFromAccount.Text = result.GetString(4);
                            tbMonthName.Text = result.GetValue(5).ToString();
                            tbNotes.Text = result.GetString(6);
                        }
                        result.Close();
                        db.CloseConnection();
                    }
                    else
                    {
                        MessageBox.Show("No record found with the given ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"SELECT * FROM Investment WHERE ID='{tbTxnId.Text}'";
                    var result = db.Retrieve(query);
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            tbClientName.Text = result.GetString(1);
                            tbClientAddress.Text = result.GetString(2);
                            tbClientPhone.Text = result.GetString(3);
                            tbBankName.Text = result.GetString(4);
                            tbBankAccountNumber.Text = result.GetString(5);
                            tbBranchName.Text = result.GetString(6);
                            tbAmount.Text = result.GetString(7);
                            dtpDate.Text = result.GetString(8);
                            cbFromAccount.Text = result.GetString(9);
                            cbKeyword.Text = result.GetString(10);
                            cbToForeignAccount.Text = result.GetString(11);
                            tbMonthName.Text = result.GetString(12);
                            tbNotes.Text = result.GetString(13);
                        }
                        result.Close();
                        db.CloseConnection();
                    }
                    else
                    {
                        MessageBox.Show("No record found with the given ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (string.IsNullOrEmpty(tbTxnId.Text))
                    {
                        MessageBox.Show("Please enter a Transaction ID to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DatabaseConnection db = new DatabaseConnection();
                    string tablename = CheckBoxForeignInvest.Checked ? "InvestmentForeign" : "Investment";
                    string query = $"DELETE FROM {tablename} WHERE ID='{tbTxnId.Text}'";
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

        private void cbMonthNameFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            new DatabaseConnection().DataGridViewPopulate($"SELECT * FROM Investment WHERE MonthName='{cbMonthNameFilter.Text}' ORDER BY ID DESC", dgvInvestment);
            string amountInvested = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Investment WHERE MonthName='{cbMonthNameFilter.Text}'");
            lblAnalytics.Text = $"Total Amount Invested in {cbMonthNameFilter.Text} : {amountInvested}/-";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnClear.PerformClick();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (CheckBoxForeignInvest.Checked)
            {
                new DatabaseConnection().DataGridViewPopulate($"SELECT * FROM InvestmentForeign WHERE Details LIKE '%{tbSearch.Text}%' OR FromAccount LIKE '%{tbSearch.Text}%'" +
                $"OR MonthName LIKE '%{tbSearch.Text}%' OR Amount LIKE '%{tbSearch.Text}%' ORDER BY ID DESC", dgvInvestmentForeign);
            }
            else
            {
                new DatabaseConnection().DataGridViewPopulate($"SELECT * FROM Investment WHERE ClientName LIKE '%{tbSearch.Text}%' " +
                $"OR ClientAddress LIKE '%{tbSearch.Text}%' OR ClientPhone LIKE '%{tbSearch.Text}%' " +
                $"OR BankName LIKE '%{tbSearch.Text}%' OR BankAccountNumber LIKE '%{tbSearch.Text}%'" +
                $"OR BranchDetails LIKE '%{tbSearch.Text}%' OR FromAccount LIKE '%{tbSearch.Text}%'" +
                $"OR Keyword LIKE '%{tbSearch.Text}%' OR MonthName LIKE '%{tbSearch.Text}%' OR Amount LIKE '%{tbSearch.Text}%' ORDER BY ID DESC", dgvInvestment);
            }
        }

        private void CheckBoxForeignInvest_CheckedChanged(object sender, EventArgs e)
        {
            label4.Text = CheckBoxForeignInvest.Checked ? "DETAILS :" : "CLIENT NAME :";
            tbClientAddress.Visible = !CheckBoxForeignInvest.Checked;
            tbClientPhone.Visible = !CheckBoxForeignInvest.Checked;
            tbBankName.Visible = !CheckBoxForeignInvest.Checked;
            tbBankAccountNumber.Visible = !CheckBoxForeignInvest.Checked;
            tbBranchName.Visible = !CheckBoxForeignInvest.Checked;
            cbKeyword.Visible = !CheckBoxForeignInvest.Checked;
            cbToForeignAccount.Visible = !CheckBoxForeignInvest.Checked;

            //from account combobox populate
            cbFromAccount.Items.Clear();
            string tablename = CheckBoxForeignInvest.Checked ? "ACCOUNTSForeign" : "ACCOUNTS";
            string query = $"SELECT accountname FROM {tablename} GROUP BY accountname ORDER BY accountname ASC";
            var result = new DatabaseConnection().Retrieve(query);
            while (result.Read())
            {
                cbFromAccount.Items.Add(result.GetValue(0).ToString());
            }
            result.Close();
        }

        private void tbTxnId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFetch.PerformClick();
                e.SuppressKeyPress = true; // Suppress the beep sound
            }
        }

        private void dgvInvestment_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                tbTxnId.Text = dgvInvestment.Rows[RowIndex].Cells[0].Value.ToString();
                CheckBoxForeignInvest.Checked = false;
                btnFetch.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvInvestmentForeign_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                tbTxnId.Text = dgvInvestmentForeign.Rows[RowIndex].Cells[0].Value.ToString();
                CheckBoxForeignInvest.Checked = true;
                btnFetch.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbMonthNameForeign_SelectedIndexChanged(object sender, EventArgs e)
        {
            new DatabaseConnection().DataGridViewPopulate($"SELECT * FROM InvestmentForeign WHERE MonthName='{cbMonthNameForeign.Text}' ORDER BY ID DESC", dgvInvestmentForeign);
            string amountInvested = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM InvestmentForeign WHERE MonthName='{cbMonthNameForeign.Text}'");
            lblForeignAnalytics.Text = $"Total Foreign Amount Invested in {cbMonthNameForeign.Text} : {amountInvested}/-";
        }
    }
}