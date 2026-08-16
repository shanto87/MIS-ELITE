using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class Discounts : Form
    {
        public Discounts()
        {
            InitializeComponent();
        }

        private void ComboBoxUpdater()
        {
            //CbclientIdentifier populate
            DatabaseConnection db = new DatabaseConnection();
            string query = "SELECT ClientIdentifier from Customerinformation ORDER BY ClientIdentifier ASC";
            var result = db.Retrieve(query);
            cbClientIdentifier.Items.Clear();// Clear previous items
            while (result.Read())
            {
                cbClientIdentifier.Items.Add(result["ClientIdentifier"].ToString());
            }
            result.Close();
            db.CloseConnection();

            //MonthNameFilter populate
            query = "SELECT DISTINCT MonthName from Discount ORDER BY ID DESC";
            result = db.Retrieve(query);
            cbMonthNameFilter.Items.Clear();// Clear previous items
            while (result.Read())
            {
                cbMonthNameFilter.Items.Add(result["MonthName"].ToString());
            }
            result.Close();
            db.CloseConnection();
        }

        private void Defaults()
        {
            // Load default values for the form controls
            lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT companyname from companyinfo where ID=1");
            tbTxnId.Text = string.Empty;
            cbClientIdentifier.Items.Clear();
            cbClientIdentifier.Text = string.Empty;
            tbAmount.Text = string.Empty;
            dtpDate.Value = DateTime.Now;
            tbMonthName.Text = new HelperClass().getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);
            tbNotes.Text = string.Empty;
            cbMonthNameFilter.Text = string.Empty;
            lblDue.Text = "Due :";
            lblAnalytics.Text = string.Empty;
        }
        private void FastLoadDefaults()
        {
            try
            {
                Defaults();
                //DatagridView populate
                new DatabaseConnection().DataGridViewPopulate("SELECT * FROM Discount ORDER BY ID DESC LIMIT 50", dgvDiscount);
                ComboBoxUpdater();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDefaults()
        {
            try
            {
                Defaults();
                //DatagridView populate
                new DatabaseConnection().DataGridViewPopulate("SELECT * FROM Discount ORDER BY ID DESC", dgvDiscount);
                ComboBoxUpdater();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Discounts_Load(object sender, EventArgs e)
        {
            FastLoadDefaults();
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                tbMonthName.Text = new HelperClass().getFullMonthName(dtpDate.Value.Month, dtpDate.Value.Year);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnClear.PerformClick();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        private bool IsValid()
        {
            if (string.IsNullOrEmpty(cbClientIdentifier.Text))
            {
                MessageBox.Show("Please select a client identifier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(tbAmount.Text))
            {
                MessageBox.Show("Please enter an amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(tbMonthName.Text))
            {
                MessageBox.Show("Please select a month name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    string due = new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'");
                    if (Convert.ToDouble(tbAmount.Text) > Convert.ToDouble(due))
                    {
                        MessageBox.Show("Discount amount cannot be greater than due amount.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"INSERT INTO Discount VALUES ('', '{cbClientIdentifier.Text}', '{tbAmount.Text}', " +
                        $"'{dtpDate.Text}', '{tbMonthName.Text}', '{tbNotes.Text}','')";
                    db.Execute(query, true);
                    db.CloseConnection();
                    LoadDefaults();
                    new CustomerDue().CalculateDues();
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
                    MessageBox.Show("Please select a transaction ID to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (IsValid())
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"UPDATE Discount SET ClientIdentifier='{cbClientIdentifier.Text}', Amount='{tbAmount.Text}', " +
                        $"Date='{dtpDate.Text}', MonthName='{tbMonthName.Text}', Notes='{tbNotes.Text}' WHERE ID={tbTxnId.Text}";
                    db.Execute(query, true);
                    db.CloseConnection();
                    LoadDefaults();
                    CustomerDue customerDue = new CustomerDue();
                    customerDue.CalculateDues();
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
                    MessageBox.Show("Please enter a transaction ID to fetch.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT * FROM Discount WHERE ID={tbTxnId.Text}";
                var result = db.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        cbClientIdentifier.Text = result["ClientIdentifier"].ToString();
                        tbAmount.Text = result["Amount"].ToString();
                        dtpDate.Value = Convert.ToDateTime(result["Date"]);
                        tbMonthName.Text = result["MonthName"].ToString();
                        tbNotes.Text = result["Notes"].ToString();
                    }
                    result.Close();
                    db.CloseConnection();
                }
                else
                {
                    MessageBox.Show("No record found with the given transaction ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                e.SuppressKeyPress = true; // Prevent the beep sound on Enter key press
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbTxnId.Text))
                {
                    MessageBox.Show("Please select a transaction ID to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = $"DELETE FROM Discount WHERE ID={tbTxnId.Text}";
                    db.Execute(query, true);
                    db.CloseConnection();
                    LoadDefaults();
                    CustomerDue customerDue = new CustomerDue();
                    customerDue.CalculateDues();
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
                string query = $"SELECT * FROM Discount WHERE MonthName='{cbMonthNameFilter.Text}' ORDER BY ID DESC";
                db.DataGridViewPopulate(query, dgvDiscount);
                db.CloseConnection();
                string discountTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(Amount) FROM Discount WHERE MonthName='{cbMonthNameFilter.Text}'");
                lblAnalytics.Text = $"Total Discount in {cbMonthNameFilter.Text}: {discountTotal} /-";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbClientIdentifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblDue.Text = "Due : " + new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'");
        }

        private void dgvDiscount_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                tbTxnId.Text = dgvDiscount.Rows[RowIndex].Cells[0].Value.ToString();
                btnFetch.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    DatabaseConnection databaseConnection = new DatabaseConnection();
                    string query = $"SELECT * FROM Discount WHERE ClientIdentifier LIKE '%{tbSearch.Text}%' OR Amount LIKE '%{tbSearch.Text}%' OR Date LIKE '%{tbSearch.Text}%' OR MonthName LIKE '%{tbSearch.Text}%' ORDER BY ID DESC";
                    databaseConnection.DataGridViewPopulate(query, dgvDiscount);
                    databaseConnection.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
