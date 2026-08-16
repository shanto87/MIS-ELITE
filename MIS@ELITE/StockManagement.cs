using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class StockManagement : Form
    {
        public StockManagement()
        {
            InitializeComponent();
        }

        //Custom Functions Start
        private void ComboBoxUpdater()
        {
            tbProductName.Items.Clear();
            DatabaseConnection db = new DatabaseConnection();
            var result = db.Retrieve("SELECT productname from stockinfo order by productname asc");
            while (result.Read())
            {
                tbProductName.Items.Add(result.GetValue(0).ToString());
            }
            result.Close();
            db.CloseConnection();
        }
        private void ClearAllTexts()
        {
            tbProductId.Text = string.Empty;
            tbProductName.Text = string.Empty;
            dtImportDate.Text = string.Empty;
            tbProductQuantity.Text = "0";
            tbOldQuantity.Text = "0";
            tbTotalQuantity.Text = "0";
            tbPurchaseRate.Text = string.Empty;
            tbSaleRate.Text = "0";
            tbProductType.Text = string.Empty;
            tbShipmentNo.Text = string.Empty;
            tbPastShipment.Text = string.Empty;
            tbPurchaseRate.Text = "0";
            tbOldPurchaseRate.Text = "0";
            tbUpdatedRate.Text = "0";
            tbShipmentNo.Text = "S= | Q= | P=";
        }
        private bool isValidInput()
        {
            return !string.IsNullOrEmpty(tbProductName.Text) &&
                   !string.IsNullOrEmpty(tbProductQuantity.Text) &&
                   !string.IsNullOrEmpty(tbPurchaseRate.Text) &&
                   !string.IsNullOrEmpty(tbProductType.Text) &&
                   !string.IsNullOrEmpty(tbShipmentNo.Text);
        }
        //Custom Functions End


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
                    DatabaseConnection db = new DatabaseConnection();
                    if (tbProductName.Text.Length > 65)
                    {
                        MessageBox.Show("Product name is too long. Please keep it under 65 characters.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string query = $"INSERT INTO stockinfo VALUES('','{tbProductName.Text}','{dtImportDate.Text}','{tbProductQuantity.Text}','{tbPurchaseRate.Text}','{tbSaleRate.Text}'," +
                    $"'{tbProductType.Text}','{tbShipmentNo.Text}','{tbPastShipment.Text}','')";
                    db.Execute(query, true);
                    db.DataGridViewPopulate("SELECT * FROM stockinfo ORDER BY ProductName ASC", dgvProducts);
                    db.CloseConnection();
                }
                else
                {
                    MessageBox.Show("Input all data", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ComboBoxUpdater();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StockManagement_Load(object sender, EventArgs e)
        {
            try
            {
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");
                tbOldPurchaseRate.Enabled = false;
                tbUpdatedRate.Enabled = false;
                tbOldQuantity.Enabled = false;
                tbTotalQuantity.Enabled = false;
                btnAddToStock.Visible = false;
                btnSubmit.Visible = false;
                tbProductQuantity.Text = "0";
                tbOldQuantity.Text = "0";
                tbTotalQuantity.Text = "0";
                tbPurchaseRate.Text = "0";
                tbOldPurchaseRate.Text = "0";
                tbUpdatedRate.Text = "0";
                tbShipmentNo.Text = "S= | Q= | P=";

                lblProductCounts.Text += new InformationRetriever().SingleDataGetter("SELECT SUM(Remains) FROM stockinfo") + " Pcs";
                lblProductVariants.Text += new InformationRetriever().SingleDataGetter("SELECT COUNT(Remains) FROM stockinfo") + " Types";

                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * FROM stockinfo order by productname asc", dgvProducts);
                db.CloseConnection();
                ComboBoxUpdater();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbProductId_TextChanged(object sender, EventArgs e)
        {
            if (tbProductId.Text != string.Empty)
            {
                tbProductId_KeyDown(sender, new KeyEventArgs(Keys.Enter));
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
                if (isValidInput() && tbProductId.Text != string.Empty)
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = "UPDATE stockinfo SET ProductName='" + tbProductName.Text.ToString() +
                        "', Date='" + dtImportDate.Text.ToString() +
                        "', Remains='" + tbProductQuantity.Text.ToString() +
                        "', PurchaseRate='" + tbPurchaseRate.Text.ToString() +
                        "', SaleRate='" + tbSaleRate.Text.ToString() +
                        "', ProductType='" + tbProductType.Text.ToString() +
                        "', ShipmentNoAndQty='" + tbShipmentNo.Text.ToString() +
                        "', PastShipmentNoAndQty='" + tbPastShipment.Text.ToString() +
                        "' WHERE ID='" + tbProductId.Text.ToString() + "'";
                    db.Execute(query, true);
                    db.DataGridViewPopulate("SELECT * FROM stockinfo ORDER BY ProductName ASC", dgvProducts);
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllTexts();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            DatabaseConnection db = new DatabaseConnection();
            db.DataGridViewPopulate("SELECT * FROM stockinfo WHERE CONCAT(productname, productType) LIKE '%" + tbSearch.Text.ToString() + "%' ORDER BY ProductName ASC", dgvProducts);
            db.CloseConnection();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            tbSearch.Text = string.Empty;

            lblProductCounts.Text = "Total Products Count: ";
            lblProductVariants.Text = "Product Variants: ";
            lblProductCounts.Text += new InformationRetriever().SingleDataGetter("SELECT SUM(Remains) FROM stockinfo") + " Pcs";
            lblProductVariants.Text += new InformationRetriever().SingleDataGetter("SELECT COUNT(Remains) FROM stockinfo") + " Types";

            DatabaseConnection databaseConnection = new DatabaseConnection();
            databaseConnection.DataGridViewPopulate("SELECT * from stockinfo ORDER BY productname ASC", dgvProducts);
            databaseConnection.CloseConnection();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkbxNewItemEntry.Checked = false;
                if (chkbxRestock.Checked)
                {
                    if (tbProductName.Text == "")
                    {
                        chkbxRestock.Checked = false;
                        MessageBox.Show("Select product information first.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        btnAddToStock.Visible = true;
                        btnUpdate.Visible = false;

                        if (tbProductQuantity.Text == string.Empty)
                        {
                            tbProductQuantity.Text = "0";
                        }
                        tbPastShipment.Text = tbShipmentNo.Text;
                        tbOldQuantity.Text = tbProductQuantity.Text.ToString();
                        tbShipmentNo.Text = "S= |Q= " + tbProductQuantity.Text + "| P=" + tbOldQuantity.Text;
                        tbProductQuantity.Text = "0";
                        tbOldPurchaseRate.Text = tbPurchaseRate.Text;
                        tbPurchaseRate.Text = "0";
                    }
                }
                else
                {
                    btnAddToStock.Visible = false;
                    btnUpdate.Visible = true;

                    tbProductQuantity.Text = tbOldQuantity.Text.ToString();
                    tbOldQuantity.Text = "0";
                    tbTotalQuantity.Text = "0";

                    tbPurchaseRate.Text = tbOldPurchaseRate.Text.ToString();
                    tbOldPurchaseRate.Text = "0";
                    tbUpdatedRate.Text = "0";

                    tbShipmentNo.Text = tbPastShipment.Text;
                    tbPastShipment.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbProductQuantity_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double newQty, newPrice, oldQty, oldPrice;
                if (chkbxRestock.Checked)
                {
                    newPrice = double.Parse(tbPurchaseRate.Text);
                    oldQty = double.Parse(tbOldQuantity.Text);
                    oldPrice = double.Parse(tbOldPurchaseRate.Text);
                    if (tbProductQuantity.Text == string.Empty)
                    {
                        newQty = 0;
                        tbTotalQuantity.Text = (0 + long.Parse(tbOldQuantity.Text)).ToString();
                        tbUpdatedRate.Text = Math.Ceiling((((oldQty * oldPrice) + (newQty * newPrice)) / (newQty + oldQty))).ToString();
                    }
                    else
                    {
                        newQty = double.Parse(tbProductQuantity.Text);
                        tbTotalQuantity.Text = (long.Parse(tbProductQuantity.Text) + long.Parse(tbOldQuantity.Text)).ToString();
                        tbUpdatedRate.Text = Math.Ceiling((((oldQty * oldPrice) + (newQty * newPrice)) / (newQty + oldQty))).ToString();
                    }
                    tbShipmentNo.Text = "S= | Q= " + tbProductQuantity.Text + "| P=" + tbOldQuantity.Text;
                }

                if (chkbxNewItemEntry.Checked)
                {
                    tbShipmentNo.Text = "S= | Q= " + tbProductQuantity.Text + "| P=" + tbOldQuantity.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            chkbxRestock.Checked = false;
            if (chkbxNewItemEntry.Checked)
            {
                tbProductId.Enabled = false;
                btnSubmit.Visible = true;
                btnUpdate.Visible = false;
                btnAddToStock.Visible = false;
                tbPastShipment.Enabled = false;
                tbPastShipment.Text = string.Empty;
            }
            else
            {
                ClearAllTexts();
                tbProductId.Enabled = true;
                btnSubmit.Visible = false;
                btnUpdate.Visible = true;
                tbPastShipment.Enabled = true;
            }
        }

        private void tbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tbProductName.Text != string.Empty)
            {
                try
                {
                    DatabaseConnection databaseConnection = new DatabaseConnection();
                    string query = "SELECT Id from stockinfo WHERE ProductName='" + tbProductName.Text.ToString() + "'";
                    var result = databaseConnection.Retrieve(query);
                    if (result != null)
                    {
                        while (result.Read())
                        {
                            tbProductId.Text = result.GetValue(0).ToString();
                        }
                        result.Close();
                        databaseConnection.CloseConnection();
                    }
                    else
                    {
                        ClearAllTexts();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ClearAllTexts();
            }
        }

        private void tbPurchaseRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double newQty, newPrice, oldQty, oldPrice;
                if (chkbxRestock.Checked)
                {
                    newQty = double.Parse(tbProductQuantity.Text);
                    oldQty = double.Parse(tbOldQuantity.Text);
                    oldPrice = double.Parse(tbOldPurchaseRate.Text);
                    if (tbPurchaseRate.Text == string.Empty)
                    {
                        newPrice = 0;
                        tbUpdatedRate.Text = Math.Ceiling((((oldQty * oldPrice) + (newQty * newPrice)) / (newQty + oldQty))).ToString();
                    }
                    else
                    {
                        newPrice = double.Parse(tbPurchaseRate.Text);
                        tbUpdatedRate.Text = Math.Ceiling((((oldQty * oldPrice) + (newQty * newPrice)) / (newQty + oldQty))).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddToStock_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Sure to proceed?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return; // User chose not to proceed
                }
                if (isValidInput() && tbProductId.Text != string.Empty)
                {
                    DatabaseConnection db = new DatabaseConnection();
                    string query = "UPDATE stockinfo SET remains='" + tbTotalQuantity.Text.ToString() +
                        "', Date='" + dtImportDate.Text.ToString() +
                        "', PurchaseRate='" + tbUpdatedRate.Text.ToString() +
                        "', SaleRate='" + tbSaleRate.Text.ToString() +
                        "', ShipmentNoAndQty='" + tbShipmentNo.Text.ToString() +
                        "', PastShipmentNoAndQty='" + tbPastShipment.Text.ToString() +
                        "' WHERE id='" + tbProductId.Text.ToString() + "'";
                    db.Execute(query, true);
                    db.DataGridViewPopulate("SELECT * FROM stockinfo ORDER BY ProductName ASC", dgvProducts);
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

        private void tbProductId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    DatabaseConnection databaseConnection = new DatabaseConnection();
                    string query = "SELECT * from stockinfo WHERE ID=" + tbProductId.Text.ToString();
                    var result = databaseConnection.Retrieve(query);
                    if (result != null)
                    {
                        while (result.Read())
                        {
                            tbProductName.Text = result.GetValue(1).ToString();
                            dtImportDate.Text = result.GetValue(2).ToString();
                            tbProductQuantity.Text = result.GetValue(3).ToString();
                            tbPurchaseRate.Text = result.GetValue(4).ToString();
                            tbSaleRate.Text = result.GetValue(5).ToString();
                            tbProductType.Text = result.GetValue(6).ToString();
                            if (chkbxRestock.Checked)
                            {
                                tbShipmentNo.Text = "S= | Q= " + tbProductQuantity.Text + "| P=" + tbOldQuantity.Text;
                                tbPastShipment.Text = result.GetValue(7).ToString();
                            }
                            else
                            {
                                tbShipmentNo.Text = result.GetValue(7).ToString();
                                tbPastShipment.Text = result.GetValue(8).ToString();
                            }
                        }
                        result.Close();
                        databaseConnection.CloseConnection();
                    }
                    else
                    {
                        ClearAllTexts();
                    }
                    if (chkbxRestock.Checked)
                    {
                        tbOldQuantity.Text = tbProductQuantity.Text;
                        tbProductQuantity.Text = "0";
                        tbOldPurchaseRate.Text = tbPurchaseRate.Text;
                        tbPurchaseRate.Text = "0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void dgvProducts_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                int ProductID = Convert.ToInt32(dgvProducts.Rows[RowIndex].Cells[0].Value);
                tbProductId.Text = ProductID.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
