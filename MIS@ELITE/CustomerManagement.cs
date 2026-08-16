using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class CustomerManagement : Form
    {
        public CustomerManagement()
        {
            InitializeComponent();
        }

        // Custom Function Start
        private void ClearAllTexts()
        {
            tbName.Text = string.Empty;
            tbAddress.Text = string.Empty;
            tbPhone.Text = string.Empty;
            tbClientIdentifier.Text = string.Empty;
            tbCustomerId.Text = string.Empty;
        }
        private bool isValidInput()
        {
            return !string.IsNullOrEmpty(tbName.Text) &&
                   !string.IsNullOrEmpty(tbAddress.Text) &&
                   !string.IsNullOrEmpty(tbPhone.Text) &&
                   !string.IsNullOrEmpty(tbClientIdentifier.Text);
        }

        // Custom Function End


        private void CustomerManagement_Load(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * FROM customerinformation ORDER BY clientname ASC", dgvRegisteredNames);
                db.CloseConnection();
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * from customerinformation where clientname LIKE '%" +
                    tbSearch.Text.ToString() + "%' OR clientphone LIKE '%" +
                    tbSearch.Text.ToString() + "%' OR clientaddress LIKE '%" +
                    tbSearch.Text.ToString() + "%' ORDER BY ClientName", dgvRegisteredNames);
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                tbSearch.Text = string.Empty;
                DatabaseConnection databaseConnection = new DatabaseConnection();
                databaseConnection.DataGridViewPopulate("SELECT * from Customerinformation ORDER BY CLIENTNAME ASC", dgvRegisteredNames);
                databaseConnection.CloseConnection();
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

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Sure to proceed?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return; // User chose not to proceed
                }
                string query = "INSERT INTO customerinformation VALUES('','" + tbName.Text.ToString() + "','" + tbAddress.Text.ToString()
                                + "','" + tbPhone.Text.ToString() + "', '" + tbClientIdentifier.Text.ToString() + "')";

                if (isValidInput())
                {
                    DatabaseConnection db = new DatabaseConnection();
                    db.Execute(query, true);
                    db.DataGridViewPopulate("SELECT * FROM customerinformation ORDER BY CLIENTNAME ASC", dgvRegisteredNames);
                    db.CloseConnection();
                    CustomerDue due = new CustomerDue();
                    due.CalculateDues();
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Sure to proceed?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return; // User chose not to proceed
                }
                if (isValidInput() && tbCustomerId.Text != string.Empty)
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = "UPDATE customerinformation SET CLIENTNAME='" + tbName.Text.ToString() +
                        "', CLIENTADDRESS='" + tbAddress.Text.ToString() +
                        "', CLIENTPHONE='" + tbPhone.Text.ToString() +
                        "', CLIENTIDENTIFIER='" + tbClientIdentifier.Text.ToString() +
                        "' WHERE ID='" + tbCustomerId.Text.ToString() + "'";
                    db.Execute(query, true);
                    db.DataGridViewPopulate("SELECT * FROM customerinformation ORDER BY CLIENTNAME ASC", dgvRegisteredNames);
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

        private void tbCustomerId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    DatabaseConnection databaseConnection = new DatabaseConnection();
                    string query = "SELECT * from customerinformation WHERE ID=" + tbCustomerId.Text.ToString();
                    var result = databaseConnection.Retrieve(query);
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            tbName.Text = result.GetValue(1).ToString();
                            tbAddress.Text = result.GetValue(2).ToString();
                            tbPhone.Text = result.GetValue(3).ToString();
                            tbClientIdentifier.Text = result.GetValue(4).ToString();
                        }
                        result.Close();
                        databaseConnection.CloseConnection();
                    }
                    else
                    {
                        MessageBox.Show("No record found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    e.SuppressKeyPress = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvRegisteredNames_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                tbCustomerId.Text = dgvRegisteredNames.Rows[RowIndex].Cells[0].Value.ToString();
                tbCustomerId_KeyDown(sender, new KeyEventArgs(Keys.Enter));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
