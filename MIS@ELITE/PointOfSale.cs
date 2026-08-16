using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class PointOfSale : Form
    {
        public PointOfSale()
        {
            InitializeComponent();
        }

        // Custom Variables for internal calculation
        private double total = 0.0, grandtotal = 0.0, productBaseValue = 0.0, productBaseTotal = 0.0, paid = 0;
        int currentStockQty = 0;
        string CustomerAddress, CustomerPhone, CustomerName;
        private static readonly Random random = new Random();

        //Custom Functions Start
        private string InvoiceNumber()
        {
            // Use time for consistency across machines
            string datePart = DateTime.Now.ToString("yyMMdd");
            string timePart = DateTime.Now.ToString("HHmmss");
            int randomDigit = random.Next(0, 10);
            // single digit 0–9
            return $"{datePart}{timePart}{randomDigit}";
        }

        private void LoadDefaults()
        {
            lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");
            tbDiscount.Text = "0";
            lblCurrentStock.Text = "";
            lblCustomerName.Text = "Customer Name : ";
            lblCustomerPhone.Text = "Customer Phone : ";
            tbProductId.Text = string.Empty;
            tbProductName.Text = string.Empty;
            tbProductQuantity.Text = string.Empty;
            tbSaleRate.Text = "0";
            tbClientPhone.Text = string.Empty;
            cbClientName.Text = string.Empty;
            tbNetTotal.Text = "0";
            tbPreviousDue.Text = "0";
            tbFinalBill.Text = "0";
            tbPaid.Text = "0";
            tbProductQuantity.Text = "0";
            cbToWhom.Text = "";
            cbToAccount.Items.Clear();
            cbToAccount.Text = "";
            tbBankName.Text = string.Empty;
            tbBankAccountNo.Text = string.Empty;
            tbChange.Text = "0";
            tbDue.Text = "0";
            tbNotes.Text = string.Empty;
            dtpSaleDate.Text = string.Empty;
            dgvReceipt.Rows.Clear();
            dgvReceipt.Refresh();
            grandtotal = 0;
            total = 0;
            productBaseTotal = 0;
            productBaseValue = 0;
            lblAdvanceTotal.Text = "";
            ComboBoxUpdater();
        }

        private void ComboBoxUpdater()
        {
            try
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

                cbToWhom.Items.Clear();
                cbToWhom.Text = "";
                result = db.Retrieve("SELECT ACCOUNTNAME FROM ACCOUNTS GROUP BY ACCOUNTNAME");
                while (result.Read())
                {
                    cbToWhom.Items.Add(result.GetString(0).ToString());
                }
                result.Close();
                db.CloseConnection();

                cbClientName.Items.Clear();
                result = db.Retrieve("SELECT ClientIdentifier FROM CustomerInformation ORDER BY ClientIdentifier");
                while (result.Read())
                {
                    cbClientName.Items.Add(result.GetString(0).ToString());
                }
                result.Close();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool isValidInput()
        {
            if (string.IsNullOrEmpty(tbProductName.Text))
            {
                MessageBox.Show("Product name is mandatory.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(tbProductId.Text))
            {
                MessageBox.Show("Product ID is mandatory.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(tbSaleRate.Text))
            {
                MessageBox.Show("Sale rate is mandatory.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrEmpty(tbProductQuantity.Text))
            {
                MessageBox.Show("Product quantity cannot be zero.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (int.Parse(tbProductQuantity.Text) <= 0)
            {
                MessageBox.Show("Negative quantity not allowed.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (Convert.ToInt32(tbProductQuantity.Text) > currentStockQty)
            {
                MessageBox.Show("Quantity is higher than available stock!", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool TransactionDataValid()
        {
            if (string.IsNullOrEmpty(cbToWhom.Text) || string.IsNullOrEmpty(cbToAccount.Text))
            {
                return false;
            }
            return true;
        }

        private string getFullMonthName(int month, int year)
        {
            HelperClass helperClass = new HelperClass();
            return helperClass.getFullMonthName(month, year);
        }

        private bool StockUpdate(DataGridView dataGridView)
        {
            try
            {
                int rows = dataGridView.Rows.Count;
                for (int i = 0; i < rows; i++)
                {
                    string productID = dataGridView.Rows[i].Cells[1].Value.ToString();
                    string quantity = dataGridView.Rows[i].Cells[3].Value.ToString();

                    //Save updated values to stock table
                    var db = new DatabaseConnection();
                    db.UpdateStockWithRowLock(int.Parse(productID), int.Parse(quantity));
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //Custom Functions End

        private void PointOfSale_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDefaults();
                lblInvoice.Text = InvoiceNumber();
                DatabaseConnection db = new DatabaseConnection();
                var result = db.Retrieve("SELECT * from companyinfo WHERE id=1");
                while (result.Read())
                {
                    shopnametxtbx.Text = result.GetValue(1).ToString() + System.Environment.NewLine + result.GetValue(2).ToString() +
                        System.Environment.NewLine + result.GetValue(3).ToString() + System.Environment.NewLine
                        + "--------------------------------------------------------------------------------------------------------------------";
                }
                result.Close();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection databaseConnection = new DatabaseConnection();
                string query = $"SELECT * from stockinfo WHERE ProductName='{tbProductName.Text}'";
                var result = databaseConnection.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        tbProductId.Text = result.GetValue(0).ToString();
                        //tbProductName.Text = result.GetValue(1).ToString();
                        tbSaleRate.Text = result.GetValue(5).ToString();
                        productBaseValue = double.Parse(result.GetValue(4).ToString());
                        currentStockQty = Convert.ToInt32(result.GetValue(3).ToString());
                        lblCurrentStock.Text = "Available " + result.GetValue(3).ToString() + " Pcs";
                    }
                    result.Close();
                }
                else
                {
                    MessageBox.Show("Query returned an empty result.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                tbProductQuantity.Text = "1";
                tbProductQuantity.Focus();
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
                if (productBaseValue > double.Parse(tbSaleRate.Text))
                {
                    DialogResult dialogResult = MessageBox.Show("Selling price is below base price. Continue?", "Price Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                }
                if (isValidInput())
                {
                    total = double.Parse(tbSaleRate.Text) * double.Parse(tbProductQuantity.Text);
                    int rows = dgvReceipt.Rows.Count;
                    int DgvQuantity = 0, newQuantity = 0;
                    double DgvPrice = 0;
                    bool added = false;
                    if (rows > 0)
                    {
                        //Checking if the product already exists in the receipt, if exist then increase quantity and recalculate
                        for (int i = 0; i < rows; i++)
                        {
                            string currentRowProductName = dgvReceipt.Rows[i].Cells[2].Value.ToString(); //dgvReceipt product name
                            if (currentRowProductName == tbProductName.Text)
                            {
                                DialogResult dialogResult = MessageBox.Show("Product already added. Do you want to increase the quantity?", "Duplicate Product", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (dialogResult == DialogResult.No)
                                {
                                    return;
                                }
                                DgvQuantity = int.Parse(dgvReceipt.Rows[i].Cells[3].Value.ToString());
                                newQuantity = DgvQuantity + int.Parse(tbProductQuantity.Text);
                                DgvPrice = double.Parse(dgvReceipt.Rows[i].Cells[5].Value.ToString());
                                dgvReceipt.Rows[i].SetValues(dgvReceipt.Rows[i].Cells[0].Value, tbProductId.Text, tbProductName.Text, newQuantity, tbSaleRate.Text, double.Parse(tbSaleRate.Text) * newQuantity);
                                added = true;
                                grandtotal = grandtotal - DgvPrice + (double.Parse(tbSaleRate.Text) * newQuantity);
                                productBaseTotal = productBaseTotal - (productBaseValue * DgvQuantity) + (productBaseValue * newQuantity);
                                break;
                            }
                        }
                        if (!added)
                        {
                            //Adding new row with new product
                            dgvReceipt.Rows.Add("", tbProductId.Text, tbProductName.Text, tbProductQuantity.Text, tbSaleRate.Text, total, productBaseValue);
                            grandtotal += total;
                            productBaseTotal += productBaseValue * double.Parse(tbProductQuantity.Text);
                        }
                    }
                    else
                    {
                        //Adding first row
                        dgvReceipt.Rows.Add("", tbProductId.Text, tbProductName.Text, tbProductQuantity.Text, tbSaleRate.Text, total, productBaseValue);
                        grandtotal += total;
                        productBaseTotal += productBaseValue * double.Parse(tbProductQuantity.Text);
                    }
                }
                tbNetTotal.Text = grandtotal.ToString();
                dgvReceipt.Sort(dgvReceipt.Columns[2], System.ComponentModel.ListSortDirection.Ascending);
                foreach (DataGridViewRow row in dgvReceipt.Rows)
                {
                    row.Cells[0].Value = row.Index + 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = "SELECT * from customerinformation where clientphone LIKE '%" + tbClientPhone.Text.ToString() + "%'";
                var result = db.Retrieve(query);
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        cbClientName.Text = result.GetValue(4).ToString();
                        lblCustomerName.Text = "CUSTOMER NAME  : " + result.GetValue(1).ToString();
                        CustomerName = result.GetValue(1).ToString();
                        CustomerAddress = result.GetValue(2).ToString();
                        lblCustomerPhone.Text = "CUSTOMER PHONE : " + result.GetValue(3).ToString();
                        CustomerPhone = result.GetValue(3).ToString();
                    }
                    result.Close();
                    db.CloseConnection();
                }
                else
                {
                    MessageBox.Show("No customer found with this phone number.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void tbNetTotal_TextChanged(object sender, EventArgs e)
        {
            tbChange.Text = "0";
            if (tbPreviousDue.Text == string.Empty)
            {
                tbFinalBill.Text = (double.Parse(tbNetTotal.Text) + 0).ToString();
            }
            else
            {
                tbFinalBill.Text = (double.Parse(tbNetTotal.Text) + double.Parse(tbPreviousDue.Text)).ToString();
            }
        }

        private void tbFinalBill_TextChanged(object sender, EventArgs e)
        {
            if (tbPaid.Text == "")
            {
                paid = 0;
            }
            else
            {
                paid = double.Parse(tbPaid.Text);
            }
            if (paid > double.Parse(tbFinalBill.Text))
            {
                tbChange.Text = (paid - double.Parse(tbFinalBill.Text)).ToString();
                tbDue.Text = "0";
            }
            else
            {
                tbChange.Text = "0";
                tbDue.Text = (double.Parse(tbFinalBill.Text) - paid).ToString();
            }
        }

        private void tbPaid_TextChanged(object sender, EventArgs e)
        {
            try
            {
                tbDiscount.Text = "0";
                if (tbPaid.Text == "")
                {
                    paid = 0;
                }
                else
                {
                    paid = double.Parse(tbPaid.Text);
                }

                if (paid > double.Parse(tbFinalBill.Text))
                {
                    tbChange.Text = (paid - double.Parse(tbFinalBill.Text)).ToString();
                    tbDue.Text = "0";
                }
                else
                {
                    tbChange.Text = "0";
                    tbDue.Text = (double.Parse(tbFinalBill.Text) - paid).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cbToWhom_SelectedIndexChanged(object sender, EventArgs e)
        {
            DatabaseConnection db = new DatabaseConnection();
            var result = db.Retrieve("SELECT KEYWORD FROM ACCOUNTS WHERE ACCOUNTNAME='" + cbToWhom.Text.ToString() + "'");
            cbToAccount.Items.Clear();
            cbToAccount.Text = "";
            tbBankName.Text = "";
            tbBankAccountNo.Text = "";
            while (result.Read())
            {
                cbToAccount.Items.Add(result.GetString(0));
            }
            result.Close();
            db.CloseConnection();
        }

        private void cbToAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            DatabaseConnection db = new DatabaseConnection();
            var result = db.Retrieve("SELECT BANKNAME,BankAccountNO FROM ACCOUNTS WHERE KEYWORD='" + cbToAccount.Text.ToString() + "'");
            while (result.Read())
            {
                tbBankName.Text = result.GetString(0).ToString();
                tbBankAccountNo.Text = result.GetString(1).ToString();
            }
            result.Close();
            db.CloseConnection();
        }

        private void cbClientName_SelectedIndexChanged(object sender, EventArgs e)
        {
            DatabaseConnection db = new DatabaseConnection();
            string query = "SELECT * from customerinformation where ClientIdentifier='" + cbClientName.Text.ToString() + "'";
            var result = db.Retrieve(query);
            if (result.HasRows)
            {
                while (result.Read())
                {
                    lblCustomerName.Text = "CUSTOMER NAME : " + result.GetValue(1).ToString();
                    CustomerName = result.GetValue(1).ToString();
                    CustomerAddress = result.GetValue(2).ToString();
                    tbClientPhone.Text = result.GetValue(3).ToString();
                    CustomerPhone = result.GetValue(3).ToString();
                    lblCustomerPhone.Text = "CUSTOMER PHONE : " + tbClientPhone.Text;
                }
                result.Close();
                db.CloseConnection();

                //Customer due getting
                tbPreviousDue.Text = new InformationRetriever().SingleDataGetter($"SELECT Due from statementcustomerdue Where ClientIdentifier='{cbClientName.Text}'");
                if (tbPreviousDue.Text == string.Empty)
                {
                    tbPreviousDue.Text = "0";
                }
                string AdvanceTotal = new InformationRetriever().SingleDataGetter($"SELECT SUM(AdvanceAmount) FROM AdvancePayment WHERE ClientIdentifier='{cbClientName.Text}'");
                double ParsedValue;
                if (double.TryParse(AdvanceTotal, out ParsedValue))
                {
                    lblAdvanceTotal.Text = $"Advance Total: {AdvanceTotal} tk";
                }
                else
                {
                    lblAdvanceTotal.Text = "";
                }
            }
            else
            {
                MessageBox.Show("No customer found with this identifier.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbProductQuantity_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int productQuantity;
                if (tbProductQuantity.Text == string.Empty)
                {
                    productQuantity = 0;
                }
                else
                {
                    productQuantity = int.Parse(tbProductQuantity.Text);
                }

                if (productQuantity > currentStockQty)
                {
                    MessageBox.Show("Quantity is higher than available stock!", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteSL_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("ARE YOU SURE TO DELETE?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    int index = int.Parse(tbDeleteSL.Text);
                    if (int.Parse(tbDeleteSL.Text) > dgvReceipt.Rows.Count)
                    {
                        MessageBox.Show("Invalid row number.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //Reducing grand total, base total
                    string RemovableProductID = dgvReceipt.Rows[index - 1].Cells[1].Value.ToString();
                    string RemovableProductQty = dgvReceipt.Rows[index - 1].Cells[3].Value.ToString();
                    string RemovableProductSellPrice = dgvReceipt.Rows[index - 1].Cells[4].Value.ToString();
                    string RemovableProductBasePrice = dgvReceipt.Rows[index - 1].Cells[6].Value.ToString();

                    productBaseTotal -= (double.Parse(RemovableProductBasePrice) * double.Parse(RemovableProductQty));
                    grandtotal -= (double.Parse(RemovableProductQty) * Double.Parse(RemovableProductSellPrice));

                    tbNetTotal.Text = grandtotal.ToString();
                    tbDiscount.Text = "0";

                    dgvReceipt.Rows.RemoveAt(index - 1);
                    tbDeleteSL.Text = string.Empty;

                    //Rearranging the row numbers
                    foreach (DataGridViewRow row in dgvReceipt.Rows)
                    {
                        row.Cells[0].Value = row.Index + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbPreviousDue_Leave(object sender, EventArgs e)
        {
            if (tbPreviousDue.Text == string.Empty)
                tbPreviousDue.Text = "0";
        }

        private void tbPaid_Leave(object sender, EventArgs e)
        {
            if (tbPaid.Text == string.Empty)
                tbPaid.Text = "0";
        }

        private void tbDue_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbDiscount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double Due = Convert.ToDouble(tbFinalBill.Text) - Convert.ToDouble(tbPaid.Text);
                double Discount = 0;

                if (tbDiscount.Text == string.Empty)
                {
                    Discount = 0;
                }
                else
                {
                    Discount = Convert.ToDouble(tbDiscount.Text);
                }

                if (Discount <= Due)
                {
                    Due -= Discount;
                    tbDue.Text = Due.ToString();
                }
                else
                {
                    tbDiscount.Text = "0";
                    MessageBox.Show("Discount cannot be more than due amount.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbChange_TextChanged(object sender, EventArgs e)
        {
            if (double.Parse(tbChange.Text) > 0)
            {
                tbDiscount.Text = "0";
                tbDiscount.Enabled = false;
            }
            else
            {
                tbDiscount.Text = "0";
                tbDiscount.Enabled = true;
            }
        }

        private void tbDiscount_Leave(object sender, EventArgs e)
        {
            if (tbDiscount.Text == string.Empty)
            {
                tbDiscount.Text = "0";
            }
        }

        private void tbClientPhone_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbDeleteSL_KeyDown(object sender, KeyEventArgs e)
        {
            //on enter press
            if (e.KeyCode == Keys.Enter)
            {
                btnDeleteSL.PerformClick();
            }
        }

        private void tbDeleteSL_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblCurrentStock_Click(object sender, EventArgs e)
        {

        }

        private void tbPreviousDue_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double PreviousDue, NetTotal;
                tbDiscount.Text = "0";
                if (tbPreviousDue.Text == string.Empty)
                {
                    PreviousDue = 0;
                }
                else
                {
                    PreviousDue = Convert.ToDouble(tbPreviousDue.Text);
                }
                NetTotal = Convert.ToDouble(tbNetTotal.Text);
                tbFinalBill.Text = (NetTotal + PreviousDue).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsSaleUpdated(Dictionary<string, object> parameters)
        {
            try
            {
                var MaxInvoice = parameters["@Invoice"].ToString().Replace("'", "''");
                var items = parameters["@Items"].ToString().Replace("'", "''");
                var clientName = parameters["@ClientName"].ToString().Replace("'", "''");
                var saleDate = parameters["@SaleDate"].ToString().Replace("'", "''");
                var baseTotal = parameters["@ProductBaseTotal"].ToString().Replace("'", "''");
                var previousDue = parameters["@PreviousDue"].ToString().Replace("'", "''");
                var netTotal = parameters["@NetTotal"].ToString().Replace("'", "''");
                var profit = parameters["@Profit"].ToString().Replace("'", "''");
                var monthname = parameters["@MonthName"].ToString().Replace("'", "''");
                var notes = parameters["@Notes"].ToString().Replace("'", "''");

                var db = new DatabaseConnection();
                // 1) Check for existing invoice -> avoid duplicate insert
                var checkReader = db.Retrieve($"SELECT COUNT(1) FROM SalesData WHERE Invoice = '{MaxInvoice}'");
                if (checkReader == null)
                    return false; // retrieval failed
                int existing = 0;
                if (checkReader.Read())
                    existing = checkReader.GetInt32(0);
                checkReader.Close();
                db.CloseConnection();

                if (existing > 0)
                {
                    MessageBox.Show($"Invoice '{MaxInvoice}' already exists. Aborting insert.", "Duplicate Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // 2) Safe insert (escaped values) - still prefer parameterized method but reusing existing helper:
                string query = $"INSERT INTO SalesData VALUES('{MaxInvoice}','{clientName}','{items}','{saleDate}','{netTotal}','{previousDue}','{baseTotal}','{profit}','{monthname}','{notes}')";
                bool result = db.ExecuteWithoutAlert(query, true);
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void btnFinalSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string MaxInvoice = lblInvoice.Text;
                DialogResult dialogResult = MessageBox.Show("ARE YOU SURE TO SAVE?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    //Customer name is mandatory for record keeping
                    if (cbClientName.Text == string.Empty)
                    {
                        MessageBox.Show("Customer name is mandatory.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // If paid amount is greater than zero then payment information must be selected
                    if (double.Parse(tbPaid.Text) > 0)
                    {
                        if (!TransactionDataValid())
                        {
                            MessageBox.Show("Transaction data is incomplete. Please select proper payment information.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    string items = "";
                    string query;
                    double profit = double.Parse(tbNetTotal.Text) - productBaseTotal;
                    string monthname = getFullMonthName(dtpSaleDate.Value.Month, dtpSaleDate.Value.Year);
                    int rows = dgvReceipt.Rows.Count;
                    bool isStockUpdated = false;
                    bool isDataSaved = false;
                    for (int i = 0; i < rows; i++)
                    {
                        string productID = dgvReceipt.Rows[i].Cells[1].Value.ToString();
                        string quantity = dgvReceipt.Rows[i].Cells[3].Value.ToString();
                        string unitprice = dgvReceipt.Rows[i].Cells[4].Value.ToString();
                        string productBasePrice = dgvReceipt.Rows[i].Cells[6].Value.ToString();
                        items += "|" + productID + "," + quantity + "," + unitprice + "," + productBasePrice;
                    }
                    var parameters = new Dictionary<string, object> { { "@Invoice", MaxInvoice }, { "@ClientName", cbClientName.Text }, { "@Items", items }, { "@SaleDate", dtpSaleDate.Text }, { "@NetTotal", tbNetTotal.Text }, { "@PreviousDue", tbPreviousDue.Text }, { "@ProductBaseTotal", productBaseTotal }, { "@Profit", profit }, { "@MonthName", monthname }, { "@Notes", tbNotes.Text } };
                    isDataSaved = IsSaleUpdated(parameters);
                    if (isDataSaved)
                    {
                        //Updating stock only when sales data is successfully updated
                        isStockUpdated = StockUpdate(dgvReceipt);
                    }
                    if (isStockUpdated)
                    {
                        //Notifying user of successful transaction
                        MessageBox.Show("Record successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //Updating Transaction Table in database if Payment is made
                        if (double.Parse(tbPaid.Text) > 0)
                        {
                            DatabaseConnection dbTransactionTable = new DatabaseConnection();
                            double actualPaid = Convert.ToDouble(tbPaid.Text) - Convert.ToDouble(tbChange.Text);
                            query = $"INSERT into Transaction Values('','{cbClientName.Text}','{dtpSaleDate.Text}','{actualPaid}','{cbToWhom.Text}'," +
                                $"'{cbToAccount.Text}','{monthname}','Auto POS Entry - {MaxInvoice}','{MaxInvoice}')";
                            dbTransactionTable.ExecuteWithoutAlert(query, true);
                        }
                        //Updating Discount Table in database
                        if (double.Parse(tbDiscount.Text) > 0)
                        {
                            DatabaseConnection dbDiscountTable = new DatabaseConnection();
                            string discountQuery = $"INSERT into Discount Values('','{cbClientName.Text}','{tbDiscount.Text}','{dtpSaleDate.Text}','{monthname}','Auto POS Entry - {MaxInvoice}','{MaxInvoice}')";
                            dbDiscountTable.ExecuteWithoutAlert(discountQuery, true);
                        }
                        //Auto Copy Invoice and Client Name for Easy Pasting
                        string ClientNameRaw = lblCustomerName.Text;
                        ClientNameRaw = ClientNameRaw.Remove(0, 16);
                        Clipboard.SetText($"E-{MaxInvoice} {ClientNameRaw}");

                        // After successful update show print preview and load defaults for next data entry
                        print.PerformClick();
                        LoadDefaults();
                        lblInvoice.Text = InvoiceNumber();
                        CustomerDue customerDue = new CustomerDue();
                        customerDue.CalculateDues();
                    }
                    else
                    {
                        // If stock update fails then we need to rollback the sales data to avoid data inconsistency
                        DatabaseConnection dbSalesTable = new DatabaseConnection();
                        string queryRollback = $"DELETE FROM SalesData WHERE Invoice='{MaxInvoice}'";
                        dbSalesTable.ExecuteWithoutAlert(queryRollback, true);
                        isStockUpdated = false;
                        MessageBox.Show("Stock update failed. Transaction rolled back.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void print_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbClientName.Text != string.Empty)
                {
                    printDialog1.Document = printDocument1;
                    printDocument1.Print();
                }
                else
                {
                    MessageBox.Show("Customer name is mandatory.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void tbProductQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbSaleRate.Focus();
                e.SuppressKeyPress = true; // prevent ding sound
            }
        }


        private void tbSaleRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSubmit.Focus();
                e.SuppressKeyPress = true;
            }
        }
        private void tbProductId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (tbProductId.Text != string.Empty)
                    {
                        DatabaseConnection databaseConnection = new DatabaseConnection();
                        string query = $"SELECT * from stockinfo WHERE ID={tbProductId.Text}";
                        var result = databaseConnection.Retrieve(query);
                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                tbProductName.Text = result.GetValue(1).ToString();
                                tbSaleRate.Text = result.GetValue(5).ToString();
                                productBaseValue = double.Parse(result.GetValue(4).ToString());
                                currentStockQty = Convert.ToInt32(result.GetValue(3).ToString());
                                lblCurrentStock.Text = "Available " + result.GetValue(3).ToString() + " Pcs";
                            }
                            result.Close();
                        }
                        else
                        {
                            MessageBox.Show("Query returned an empty result.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Product ID cannot be empty.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    tbProductQuantity.Text = "1";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                e.SuppressKeyPress = true; // prevent ding sound
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("ARE YOU SURE TO CLEAR ALL?", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                LoadDefaults();
            }
        }

        private int currentRowIndex = 0;
        private int pagenumber = 1;
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                int borderWidth = e.PageBounds.Width - 45 * 2;  // Adjust based on margins
                int borderHeight = e.PageBounds.Height - 45 * 2; // Adjust based on margins
                int footerHeight = 100; // Reserve space for footer
                int bottomLimit = e.MarginBounds.Bottom - footerHeight;

                // Get company information
                DatabaseConnection db = new DatabaseConnection();
                var result = db.Retrieve("SELECT * from companyinfo WHERE id=1");
                string CompanyName = "", Address = "", Contact = "";
                while (result.Read())
                {
                    CompanyName = result.GetValue(1).ToString();
                    Address = result.GetValue(2).ToString();
                    Contact = result.GetValue(3).ToString();
                }
                result.Close();
                db.CloseConnection();

                const string fontName = "Bahnschrift";
                const int leftMargin = 50;
                const int lineHeight = 17; // Space between lines.   
                int currentY = 50; // Start at the top margin.

                using (Font defaultFont = new Font(fontName, 10, FontStyle.Regular))
                using (Font HeaderFont = new Font(fontName, 12, FontStyle.Bold))
                using (Font underlinedFont = new Font(fontName, 10, FontStyle.Underline))
                using (Font smallFont = new Font(fontName, 9, FontStyle.Regular))
                using (Font smallFontItalic = new Font("Calibri", 10, FontStyle.Italic))
                using (Font boldFont = new Font(fontName, 9, FontStyle.Bold))
                {
                    //Current Row Index keeps track of which row to print next.
                    if (currentRowIndex == 0)
                    {
                        //Page Headers Goes Here
                        DrawHeader(e, defaultFont, HeaderFont, boldFont, leftMargin, currentY, lineHeight, Address, Contact);

                        currentY = 165; // y=165
                        e.Graphics.DrawString($"INVOICE #", defaultFont, Brushes.Black, leftMargin, currentY);//invoice number
                        e.Graphics.DrawString($" : {lblInvoice.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);//invoice number

                        currentY += lineHeight;
                        e.Graphics.DrawString($"DATE", defaultFont, Brushes.Black, leftMargin, currentY);//date //y=182
                        e.Graphics.DrawString($" : {dtpSaleDate.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);//date

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER NAME", defaultFont, Brushes.Black, leftMargin, currentY);//customer name //y=199
                        e.Graphics.DrawString($" : {CustomerName}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER ADDRESS", defaultFont, Brushes.Black, leftMargin, currentY);//customer address //y=216
                        e.Graphics.DrawString($" : {CustomerAddress}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER PHONE", defaultFont, Brushes.Black, leftMargin, currentY);//customer phone //y=233
                        e.Graphics.DrawString($" : {CustomerPhone}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("ORDER DETAILS", underlinedFont, Brushes.Black, 350, currentY); //y=250

                        currentY += lineHeight; //y=282
                        // Table Headers that you want at the top of every page:
                        e.Graphics.DrawString("SL", defaultFont, Brushes.Black, leftMargin, currentY);
                        e.Graphics.DrawString("PID", defaultFont, Brushes.Black, leftMargin + 30, currentY);
                        e.Graphics.DrawString("ITEMS", defaultFont, Brushes.Black, leftMargin + 150, currentY);
                        e.Graphics.DrawString("QUANTITY", defaultFont, Brushes.Black, 560, currentY);
                        e.Graphics.DrawString("PRICE", defaultFont, Brushes.Black, 635, currentY);
                        e.Graphics.DrawString("SUBTOTAL", defaultFont, Brushes.Black, 685, currentY);

                        currentY += lineHeight; //y=300
                        e.Graphics.DrawString("-----------------------------------------------------------------------------------------------------------", defaultFont, Brushes.Black, leftMargin, currentY);
                    }
                    else
                    {
                        // For subsequent pages, just draw the header
                        DrawHeader(e, defaultFont, HeaderFont, boldFont, leftMargin, currentY, lineHeight, Address, Contact);

                        currentY = 165; // y=165
                        e.Graphics.DrawString($"INVOICE #", defaultFont, Brushes.Black, leftMargin, currentY);//invoice number
                        e.Graphics.DrawString($" : {lblInvoice.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);//invoice number

                        currentY += lineHeight;
                        e.Graphics.DrawString($"DATE", defaultFont, Brushes.Black, leftMargin, currentY);//date //y=182
                        e.Graphics.DrawString($" : {dtpSaleDate.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);//date

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER NAME", defaultFont, Brushes.Black, leftMargin, currentY);//customer name //y=199
                        e.Graphics.DrawString($" : {CustomerName}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER ADDRESS", defaultFont, Brushes.Black, leftMargin, currentY);//customer address //y=216
                        e.Graphics.DrawString($" : {CustomerAddress}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER PHONE", defaultFont, Brushes.Black, leftMargin, currentY);//customer phone //y=233
                        e.Graphics.DrawString($" : {CustomerPhone}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        //Print rest information for subsequent pages
                        currentY = 250;
                        e.Graphics.DrawString("ORDER DETAILS - CONTINUES", underlinedFont, Brushes.Black, 330, currentY);
                        currentY += lineHeight;
                        e.Graphics.DrawString("SL", defaultFont, Brushes.Black, leftMargin, currentY);
                        e.Graphics.DrawString("PID", defaultFont, Brushes.Black, leftMargin + 30, currentY);
                        e.Graphics.DrawString("ITEMS", defaultFont, Brushes.Black, leftMargin + 150, currentY);
                        e.Graphics.DrawString("QUANTITY", defaultFont, Brushes.Black, 560, currentY);
                        e.Graphics.DrawString("PRICE", defaultFont, Brushes.Black, 635, currentY);
                        e.Graphics.DrawString("SUBTOTAL", defaultFont, Brushes.Black, 685, currentY);
                        currentY += lineHeight;
                        e.Graphics.DrawString("-----------------------------------------------------------------------------------------------------------", defaultFont, Brushes.Black, leftMargin, currentY);
                    }

                    // Print items (continue from previous page if applicable)
                    while (currentRowIndex < dgvReceipt.Rows.Count)
                    {
                        if (currentY + lineHeight > bottomLimit)
                        {
                            // Draw footer and page number
                            DrawFooter(e, smallFont, leftMargin, pagenumber);
                            e.HasMorePages = true;
                            pagenumber++;
                            currentY += lineHeight;
                            e.Graphics.DrawString("-----------------------------------------------------------------------------------------------------------", defaultFont, Brushes.Black, leftMargin, currentY);

                            return;
                        }

                        DataGridViewRow row = dgvReceipt.Rows[currentRowIndex];
                        currentY += lineHeight;
                        e.Graphics.DrawString(row.Cells[0]?.Value?.ToString() ?? string.Empty, defaultFont, Brushes.Black, leftMargin, currentY);
                        e.Graphics.DrawString(row.Cells[1]?.Value?.ToString() ?? string.Empty, defaultFont, Brushes.Black, leftMargin + 30, currentY);
                        e.Graphics.DrawString(row.Cells[2]?.Value?.ToString() ?? string.Empty, defaultFont, Brushes.Black, leftMargin + 60, currentY);
                        e.Graphics.DrawString(row.Cells[3]?.Value?.ToString() ?? string.Empty, defaultFont, Brushes.Black, 590, currentY);
                        e.Graphics.DrawString(row.Cells[4]?.Value?.ToString() ?? string.Empty, defaultFont, Brushes.Black, 635, currentY);
                        e.Graphics.DrawString(row.Cells[5]?.Value?.ToString() ?? string.Empty, defaultFont, Brushes.Black, 695, currentY);

                        currentRowIndex++; // Move to the next row.
                    }

                    // After all items have been printed, summary information goes here               
                    currentY += lineHeight;
                    e.Graphics.DrawString("-----------------------------------------------------------------------------------------------------------", defaultFont, Brushes.Black, leftMargin, currentY);

                    //200 is the approx height required for the summary section
                    if (currentY + lineHeight + 200 > bottomLimit)
                    {
                        // Draw footer and page number
                        DrawFooter(e, smallFont, leftMargin, pagenumber);
                        e.HasMorePages = true;
                        pagenumber++;
                        return;
                    }

                    currentY += lineHeight;
                    e.Graphics.DrawString($"IN WORDS", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {new ConvertNumberToWords().ConvertNumberToWord(Convert.ToDouble(tbNetTotal.Text))} Tk Only.", smallFont, Brushes.Black, leftMargin + 100, currentY);

                    currentY += lineHeight;
                    e.Graphics.DrawString($"TOTAL PRICE", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {tbNetTotal.Text}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);


                    currentY += lineHeight;
                    e.Graphics.DrawString($"PREVIOUS DUE", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {tbPreviousDue.Text}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);


                    currentY += lineHeight;
                    e.Graphics.DrawString($"GRAND TOTAL", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {tbFinalBill.Text}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);

                    currentY += lineHeight;
                    e.Graphics.DrawString($"PAID", defaultFont, Brushes.Black, leftMargin, currentY);
                    if (double.Parse(tbPaid.Text) > 0)
                    {
                        e.Graphics.DrawString($": {tbPaid.Text}/- tk on : {dtpSaleDate.Text}", defaultFont, Brushes.Black, leftMargin + 100, currentY);
                    }
                    else
                    {
                        e.Graphics.DrawString($": {tbPaid.Text}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);
                    }

                    currentY += lineHeight;
                    e.Graphics.DrawString($"DISCOUNT", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {tbDiscount.Text}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);

                    currentY += lineHeight;
                    e.Graphics.DrawString($"DUE", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {tbDue.Text}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);

                    currentY += lineHeight;
                    e.Graphics.DrawString($"CHANGE", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {tbChange.Text}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);

                    if (tbNotes.Text.Length > 0)
                    {
                        currentY += lineHeight + 10;
                        e.Graphics.DrawString($"NOTES : {tbNotes.Text}", smallFontItalic, Brushes.Black, leftMargin, currentY);
                    }
                    DrawFooter(e, smallFont, leftMargin, pagenumber);

                    // When everything is printed, ensure we reset for future printing.
                    currentRowIndex = 0;
                    e.HasMorePages = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DrawFooter(PrintPageEventArgs e, Font font, int leftMargin, int pageNumber)
        {
            e.Graphics.DrawString($"PAGE {pageNumber}", font, Brushes.Black, 700, e.PageBounds.Bottom - 90);
            e.Graphics.DrawString($"PRINTED BY - {Properties.Settings.Default.LoggedUserFullName}", font, Brushes.Black, leftMargin, e.PageBounds.Bottom - 90);
            e.Graphics.DrawString("Software Powered by: MIS@ELITE_VENTURES, Contact: +8801776935787, E-Mail: mashiur.cse34@gmail.com", font, Brushes.Black, leftMargin, e.PageBounds.Bottom - 75);
            e.Graphics.DrawString("Entry Time: " + DateTime.Now.ToString("F") + " | This is a computer generated slip, no signature required.", font, Brushes.Black, leftMargin, e.PageBounds.Bottom - 60);
        }

        private void DrawHeader(PrintPageEventArgs e, Font defaultFont, Font HeaderFont, Font boldFont, int leftMargin, int currentY, int lineHeight, string Address, string Contact)
        {
            e.Graphics.DrawString(CompanyName, HeaderFont, Brushes.Black, leftMargin, currentY);//CompanyName name

            // Load the logo image
            string imagePath = new InformationRetriever().SingleDataGetter("SELECT directory FROM media WHERE id=1");
            if (File.Exists(imagePath))
            {
                Image logo = Image.FromFile(imagePath, true);
                // Define the position and size for the logo
                // int logoX = 500; // Align to the upper-right corner
                int logoX = 500; // Adjust this value as needed
                int logoY = 50; // Adjust this value as needed
                e.Graphics.DrawImage(logo, logoX, logoY, 252, 72);
            }
            else
            {
                MessageBox.Show("Logo file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            currentY += lineHeight + 5;
            e.Graphics.DrawString(Address, defaultFont, Brushes.Black, leftMargin, currentY);//address

            currentY += lineHeight + 15;
            e.Graphics.DrawString(Contact, defaultFont, Brushes.Black, leftMargin, currentY);//contact

            currentY += lineHeight + 15;
            e.Graphics.DrawString("----------------------------------------------------------------------------------------------------------------------",
                boldFont, Brushes.Black, leftMargin, currentY);//line
        }
    }
}

