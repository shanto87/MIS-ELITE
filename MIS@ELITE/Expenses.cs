using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class Expenses : Form
    {
        public Expenses()
        {
            InitializeComponent();
        }

        private void ComboBoxUpdater()
        {
            //MonthNameFilter populate
            string query = "SELECT DISTINCT MonthName from Expense ORDER BY MonthName ASC";
            var result = new DatabaseConnection().Retrieve(query);
            cbMonthNameFilter.Items.Clear(); // Clear previous items
            if (result.HasRows)
            {
                while (result.Read())
                {
                    cbMonthNameFilter.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
            }

            //FromAccount populate
            string tablename = "Accounts";
            if (RbForeign.Checked)
            {
                tablename = "AccountsForeign";
            }
            query = $"SELECT DISTINCT AccountName from {tablename} ORDER BY AccountName ASC";
            cbFromAccount.Items.Clear(); // Clear previous items
            result = new DatabaseConnection().Retrieve(query);
            if (result.HasRows)
            {
                while (result.Read())
                {
                    cbFromAccount.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
            }
        }

        private void Defaults()
        {
            //Default values
            lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT companyname from CompanyInfo where ID=1");
            tbTxnId.Text = string.Empty;
            tbDetails.Text = string.Empty;
            tbAmount.Text = string.Empty;
            dtpDate.Text = DateTime.Now.ToString();
            cbFromAccount.Text = string.Empty;
            cbKeyword.Items.Clear();
            tbMonthName.Text = new HelperClass().getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);
            tbNotes.Text = string.Empty;
            cbMonthNameFilter.Text = string.Empty;
            tbSearch.Text = string.Empty;
            lblAnalytics.Text = string.Empty;
        }

        private void LoadDefaults()
        {
            try
            {
                Defaults();
                //Datagridview populate
                new DatabaseConnection().DataGridViewPopulate("SELECT * from Expense ORDER BY ID DESC", dgvExpense);

                ComboBoxUpdater();
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
                //Datagridview populate
                new DatabaseConnection().DataGridViewPopulate("SELECT * from Expense ORDER BY ID DESC LIMIT 50", dgvExpense);

                ComboBoxUpdater();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Expenses_Load(object sender, EventArgs e)
        {
            RbBangladesh.PerformClick();
            FastLoadDefaults();
        }

        private void cbFromAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            // This method is called when the selected index of the cbFromAccount ComboBox changes.
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                string tablename = "Accounts";
                if (RbForeign.Checked)
                {
                    tablename = "AccountsForeign";
                }
                string query = $"SELECT Keyword from {tablename} where AccountName='" + cbFromAccount.Text + "' ORDER BY KEYWORD ASC";
                var result = db.Retrieve(query);
                cbKeyword.Items.Clear(); // Clear previous items
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        cbKeyword.Items.Add(result.GetValue(0).ToString());
                    }
                    result.Close();
                    db.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadDefaults();
            RbBangladesh.PerformClick();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnClear.PerformClick();
        }

        private bool IsValid()
        {
            if (string.IsNullOrEmpty(tbDetails.Text))
            {
                MessageBox.Show("Please enter details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(cbFromAccount.Text))
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
                MessageBox.Show("Please enter Month Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            tbMonthName.Text = new HelperClass().getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);
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
                    //radio button check
                    string foreignExpense = "NO";
                    if (RbForeign.Checked)
                    {
                        foreignExpense = "YES";
                    }
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"INSERT INTO Expense VALUES('','{tbDetails.Text}','{tbAmount.Text}','{dtpDate.Text}','{cbFromAccount.Text}'," +
                        $"'{cbKeyword.Text}','{tbMonthName.Text}','{tbNotes.Text}','{foreignExpense}')";
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
                if (string.IsNullOrWhiteSpace(tbTxnId.Text))
                {
                    MessageBox.Show("Please enter Transaction ID to fetch.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT * from Expense where ID='{tbTxnId.Text}'";
                var result = db.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        tbDetails.Text = result.GetValue(1).ToString();
                        tbAmount.Text = result.GetValue(2).ToString();
                        dtpDate.Text = result.GetValue(3).ToString();
                        cbFromAccount.Text = result.GetValue(4).ToString();
                        cbKeyword.Text = result.GetValue(5).ToString();
                        tbMonthName.Text = result.GetValue(6).ToString();
                        tbNotes.Text = result.GetValue(7).ToString();
                        if (result.GetValue(8).ToString() == "YES")
                        {
                            RbForeign.PerformClick();
                        }
                        else
                        {
                            RbBangladesh.PerformClick();
                        }
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

        private void btnUpdate_Click(object sender, EventArgs e)
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
                    if (string.IsNullOrEmpty(tbTxnId.Text))
                    {
                        MessageBox.Show("Please enter Transaction ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DatabaseConnection db = new DatabaseConnection();
                    //radio button check
                    string foreignExpense = "NO";
                    if (RbForeign.Checked)
                    {
                        foreignExpense = "YES";
                    }
                    string query = $"UPDATE Expense SET Details='{tbDetails.Text}', Amount='{tbAmount.Text}', Date='{dtpDate.Text}', " +
                        $"FromAccount='{cbFromAccount.Text}', Keyword='{cbKeyword.Text}', MonthName='{tbMonthName.Text}', Notes='{tbNotes.Text}', ForeignExpense='{foreignExpense}' " +
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbTxnId.Text))
                {
                    MessageBox.Show("Please enter Transaction ID to Delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"DELETE FROM Expense WHERE ID='{tbTxnId.Text}'";
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
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT * from Expense where MonthName='{cbMonthNameFilter.Text}' ORDER BY ID DESC";
                db.DataGridViewPopulate(query, dgvExpense);
                db.CloseConnection();
                string expenseTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Expense WHERE MonthName='{cbMonthNameFilter.Text}'");
                lblAnalytics.Text = $"Total Expense for {cbMonthNameFilter.Text}: {expenseTotal}/-";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT * from Expense where Details like '%{tbSearch.Text}%' OR Amount like '%{tbSearch.Text}%' OR Date like '%{tbSearch.Text}%' OR FromAccount like '%{tbSearch.Text}%' or Keyword like '%{tbSearch.Text}%' OR MonthName like '%{tbSearch.Text}%'  ORDER BY ID DESC";
                db.DataGridViewPopulate(query, dgvExpense);
                db.CloseConnection();
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
                e.SuppressKeyPress = true; // Suppress the beep sound
            }
        }

        private void RbForeign_CheckedChanged(object sender, EventArgs e)
        {
            FromAccountPopulateOnRadioButtonChange();
        }

        private void RbBangladesh_CheckedChanged(object sender, EventArgs e)
        {
            FromAccountPopulateOnRadioButtonChange();
        }

        void FromAccountPopulateOnRadioButtonChange()
        {
            try
            {
                //FromAccount populate
                string tablename = "Accounts";
                if (RbForeign.Checked)
                {
                    tablename = "AccountsForeign";
                }
                string query = $"SELECT DISTINCT AccountName from {tablename} ORDER BY AccountName ASC";
                cbFromAccount.Items.Clear(); // Clear previous items
                var result = new DatabaseConnection().Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        cbFromAccount.Items.Add(result.GetValue(0).ToString());
                    }
                    result.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvExpense_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                tbTxnId.Text = dgvExpense.Rows[RowIndex].Cells[0].Value.ToString();
                btnFetch.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
