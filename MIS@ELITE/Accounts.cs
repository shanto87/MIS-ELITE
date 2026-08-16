using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class Accounts : Form
    {
        public Accounts()
        {
            InitializeComponent();
        }

        private void Accounts_Load(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * FROM accounts ", dgvAccounts);
                db.DataGridViewPopulate("SELECT * FROM accountsForeign ", dgvAccountsForeign);
                db.CloseConnection();
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");
                RbBangladesh.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearAllTexts()
        {
            tbAccountID.Text = string.Empty;
            tbAccountName.Text = string.Empty;
            tbPhone.Text = string.Empty;
            tbBankName.Text = string.Empty;
            tbBankAccountNumber.Text = string.Empty;
            tbKeyword.Text = string.Empty;
            RbBangladesh.PerformClick();

            DatabaseConnection db = new DatabaseConnection();
            db.DataGridViewPopulate("SELECT * FROM accounts ", dgvAccounts);
            db.DataGridViewPopulate("SELECT * FROM accountsForeign ", dgvAccountsForeign);
            db.CloseConnection();
        }
        private bool isValidInput()
        {
            if (tbAccountName.Text != string.Empty && tbPhone.Text != string.Empty && tbPhone.Text != string.Empty && tbBankName.Text != string.Empty &&
                tbBankAccountNumber.Text != string.Empty && tbKeyword.Text != string.Empty)
                return true;
            return false;
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
                    DatabaseConnection databaseConnection = new DatabaseConnection();
                    //checking radio button
                    string tablename = string.Empty;
                    if (RbBangladesh.Checked)
                    {
                        tablename = "accounts";
                    }
                    else
                    {
                        tablename = "accountsForeign";
                    }
                    var query = "INSERT INTO " + tablename + " VALUES('','" + tbAccountName.Text.ToString() + "','" + tbPhone.Text.ToString() +
                        "','" + tbBankName.Text.ToString() + "','" + tbBankAccountNumber.Text.ToString() + "','"
                        + tbKeyword.Text.ToString() + "')";
                    databaseConnection.Execute(query, true);
                    query = "SELECT * from accounts ORDER by ACCOUNTNAME ASC";
                    databaseConnection.DataGridViewPopulate(query, dgvAccounts);
                    databaseConnection.DataGridViewPopulate("SELECT * from accountsForeign ORDER by ACCOUNTNAME ASC", dgvAccountsForeign);
                    databaseConnection.CloseConnection();
                }
                else
                {
                    MessageBox.Show("Input all data", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllTexts();
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
                if (isValidInput() && tbAccountID.Text != string.Empty)
                {
                    DatabaseConnection db = new DatabaseConnection();
                    //checking radio button
                    string tablename = string.Empty;
                    if (RbBangladesh.Checked)
                    {
                        tablename = "accounts";
                    }
                    else
                    {
                        tablename = "accountsForeign";
                    }
                    string query = "UPDATE " + tablename + " SET AccountName='" + tbAccountName.Text.ToString() +
                        "', AccountPhone='" + tbPhone.Text.ToString() +
                        "', BankName='" + tbBankName.Text.ToString() +
                        "', BankAccountNo='" + tbBankAccountNumber.Text.ToString() +
                        "', Keyword='" + tbKeyword.Text.ToString() +
                        "' WHERE ID='" + tbAccountID.Text.ToString() + "'";
                    db.Execute(query, true);
                    db.DataGridViewPopulate("SELECT * FROM accounts ORDER BY AccountName ASC", dgvAccounts);
                    db.DataGridViewPopulate("SELECT * FROM accountsForeign ORDER BY AccountName ASC", dgvAccountsForeign);
                    db.CloseConnection();
                }
                else
                {
                    MessageBox.Show("Input all data", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbAccountID_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbAccountID.Text != string.Empty)
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure want to delete the data?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        //checking radio button
                        string query = string.Empty;
                        if (RbBangladesh.Checked)
                        {
                            query = "DELETE FROM ACCOUNTS WHERE ID='" + tbAccountID.Text.ToString() + "'";
                        }
                        else
                        {
                            query = "DELETE FROM ACCOUNTSForeign WHERE ID='" + tbAccountID.Text.ToString() + "'";
                        }
                        DatabaseConnection db = new DatabaseConnection();
                        db.Execute(query, true);
                        query = "SELECT * from ACCOUNTS order by AccountName ASC";
                        db.DataGridViewPopulate(query, dgvAccounts);
                        db.DataGridViewPopulate("SELECT * from ACCOUNTSForeign order by AccountName ASC", dgvAccountsForeign);
                        db.CloseConnection();
                    }
                }
                else
                {
                    MessageBox.Show("ID Cannot be empty", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbAccountID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    DatabaseConnection databaseConnection = new DatabaseConnection();
                    string query = string.Empty;
                    //checking radio button
                    if (RbBangladesh.Checked)
                    {
                        query = "SELECT * from accounts WHERE ID=" + tbAccountID.Text.ToString();
                    }
                    else
                    {
                        query = "SELECT * from accountsForeign WHERE ID=" + tbAccountID.Text.ToString();
                    }
                    var result = databaseConnection.Retrieve(query);
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            tbAccountName.Text = result.GetValue(1).ToString();
                            tbPhone.Text = result.GetValue(2).ToString();
                            tbBankName.Text = result.GetValue(3).ToString();
                            tbBankAccountNumber.Text = result.GetValue(4).ToString();
                            tbKeyword.Text = result.GetValue(5).ToString();
                        }
                        result.Close();
                        databaseConnection.CloseConnection();
                    }
                    else
                    {
                        MessageBox.Show("No data found", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    e.SuppressKeyPress = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvAccounts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                int ID = Convert.ToInt32(dgvAccounts.Rows[RowIndex].Cells[0].Value);
                tbAccountID.Text = ID.ToString();
                tbAccountID_KeyDown(sender, new KeyEventArgs(Keys.Enter));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}