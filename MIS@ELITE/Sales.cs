using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class Sales : Form
    {
        // public static Sales Instance { get; private set; }
        public static string UserName
        {
            get; set;
        }
        public static string Password
        {
            get; set;
        }
        public static string UserType
        {
            get; set;
        }

        public Sales()
        {
            InitializeComponent();
        }

        //Custom Functions Start
        private void DefaultValues()
        {
            try
            {
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");
                tbInvoice.Text = string.Empty;
                lblDue.Text = "Due :";
                cbClientIdentifier.Text = string.Empty;
                cbMonthName.Text = string.Empty;
                tbItemCodes.Text = string.Empty;
                dtpOrderDate.Text = string.Empty;
                tbProductValue.Text = "0";
                tbNetTotal.Text = "0";
                tbProfit.Text = string.Empty;
                tbMonthName.Text = string.Empty;
                tbClientName.Text = string.Empty;
                tbClientAddress.Text = string.Empty;
                tbClientPhone.Text = string.Empty;
                lblSummary.Text = string.Empty;
                tbNotes.Text = string.Empty;
                dgvReceipt.Rows.Clear();
                dgvReceipt.Refresh();
                tbSearch.Text = string.Empty;

                DatabaseConnection dbClientInfo = new DatabaseConnection();
                string query = "SELECT ClientIdentifier FROM customerinformation";
                var result = dbClientInfo.Retrieve(query);
                cbClientIdentifier.Items.Clear();
                while (result.Read())
                {
                    cbClientIdentifier.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
                dbClientInfo.CloseConnection();

                DatabaseConnection dbMonth = new DatabaseConnection();
                query = "SELECT MonthName FROM SalesData group by MonthName";
                result = dbMonth.Retrieve(query);
                cbMonthName.Items.Clear();
                while (result.Read())
                {
                    cbMonthName.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();
                dbMonth.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDefault()
        {
            try
            {
                DefaultValues();
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * from SALESDATA Order by Invoice DESC", dgvSalesData);
                db.CloseConnection();
                // Hide sensitive columns for EMPLOYEE access level
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    dgvSalesData.Columns[6].Visible = false;
                    dgvSalesData.Columns[7].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FastLoadDefault()
        {
            try
            {
                DefaultValues();
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * from SALESDATA Order by Invoice DESC LIMIT 100", dgvSalesData);
                db.CloseConnection();
                // Hide sensitive columns for EMPLOYEE access level
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    dgvSalesData.Columns[6].Visible = false;
                    dgvSalesData.Columns[7].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidInput()
        {
            return !(string.IsNullOrWhiteSpace(tbInvoice.Text) ||
                     string.IsNullOrWhiteSpace(cbClientIdentifier.Text) ||
                     string.IsNullOrWhiteSpace(tbItemCodes.Text) ||
                     string.IsNullOrWhiteSpace(tbNetTotal.Text) ||
                     string.IsNullOrWhiteSpace(tbProductValue.Text) ||
                     string.IsNullOrWhiteSpace(tbProfit.Text) ||
                     string.IsNullOrWhiteSpace(tbMonthName.Text));
        }

        //Custom Functions End

        private void Sales_Load(object sender, EventArgs e)
        {
            FastLoadDefault();
        }

        private void DgvReceiptFillup(string ItemCodes)
        {
            try
            {
                // Hide sensitive columns for EMPLOYEE access level
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    dgvReceipt.Columns[4].Visible = false;
                    dgvReceipt.Columns[5].Visible = false;
                }


                List<string> itemList;
                itemList = ItemCodes.Split('|').ToList();
                itemList.Remove("");

                string itemID, itemQuantity, itemPrice, itemBaseValue;
                List<string> tempArray;
                var Retriever = new InformationRetriever();
                foreach (string item in itemList)
                {
                    tempArray = item.Split(',').ToList();
                    itemID = tempArray[0];
                    itemQuantity = tempArray[1];
                    itemPrice = tempArray[2];
                    itemBaseValue = tempArray[3];

                    string itemName = Retriever.SingleDataGetter("SELECT productname FROM stockinfo WHERE ID='" + itemID + "'");
                    //string itemBasePrice = informationRetriever.SingleDataGetter("SELECT PurchaseRate FROM stockinfo where ID='" + itemID + "'");
                    dgvReceipt.Rows.Add(dgvReceipt.Rows.Count + 1, itemID, itemName, itemQuantity, itemBaseValue, (double.Parse(itemQuantity) * double.Parse(itemBaseValue)).ToString(), itemPrice, (double.Parse(itemQuantity) * double.Parse(itemPrice)).ToString());
                }

                double BaseTotal = 0;
                double SellTotal = 0;
                foreach (DataGridViewRow Row in dgvReceipt.Rows)
                {
                    BaseTotal += double.Parse(Row.Cells[5].Value.ToString());
                    SellTotal += double.Parse(Row.Cells[7].Value.ToString());
                }
                tbProductValue.Text = BaseTotal.ToString();
                tbNetTotal.Text = SellTotal.ToString();
                tbProfit.Text = (SellTotal - BaseTotal).ToString();
                double percentage = ((SellTotal - BaseTotal) / BaseTotal) * 100;

                if (dgvReceipt.Rows.Count > 0)
                {
                    lblSummary.Text = $"SUMMARY : BASE= {tbProductValue.Text}/-, SELL= {tbNetTotal.Text}/-, PROFIT= {tbProfit.Text}/-, PERCENTAGE= {percentage:0.00}%";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbInvoice.Text))
                {
                    MessageBox.Show("Enter Invoice number to fetch data", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Hide summary for EMPLOYEE access level
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    lblSummary.Visible = false;
                    tbProfit.Visible = false;
                    tbProductValue.Visible = false;
                }

                dgvReceipt.Rows.Clear();
                dgvReceipt.Refresh();
                string ItemCodes = "";
                DatabaseConnection db = new DatabaseConnection();
                InformationRetriever informationRetriever = new InformationRetriever();
                var result = db.Retrieve("SELECT * FROM salesdata WHERE Invoice='" + tbInvoice.Text + "'");
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        tbInvoice.Text = result.GetValue(0).ToString();
                        cbClientIdentifier.Text = result.GetValue(1).ToString();
                        tbItemCodes.Text = result.GetValue(2).ToString();
                        ItemCodes = result.GetValue(2).ToString();
                        dtpOrderDate.Text = result.GetValue(3).ToString();
                        //tbNetTotal.Text = result.GetValue(4).ToString();
                        //tbProductValue.Text = result.GetValue(6).ToString();                        
                        tbProfit.Text = result.GetValue(7).ToString();
                        tbMonthName.Text = result.GetValue(8).ToString();
                        tbNotes.Text = result.GetValue(9).ToString();
                    }
                    result.Close();
                    db.CloseConnection();
                }
                else
                {
                    MessageBox.Show("No data found for this invoice.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //dgvreceipt fillup
                DgvReceiptFillup(ItemCodes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private bool IsAuthorizedUser(string UserName, string PassWord, string UserType)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                string dbpassword = "";
                string dbusertype = "";

                var result = db.Retrieve("SELECT * FROM AUTHENTICATION WHERE USERNAME='" + UserName + "'");
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        dbpassword = result.GetValue(2).ToString();
                        dbusertype = result.GetValue(3).ToString();
                    }
                    result.Close();
                    db.CloseConnection();

                    // Check if the provided credentials match.
                    bool isPasswordMatched = PasswordHelper.VerifyPassword(PassWord, dbpassword);

                    if (isPasswordMatched && UserType == dbusertype)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Credentials Doesn't Match.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("No data found for this user.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidInput())
                {
                    VerificationForm verificationForm = new VerificationForm();
                    verificationForm.ShowDialog();

                    if (IsAuthorizedUser(UserName, Password, UserType))
                    {
                        DatabaseConnection db = new DatabaseConnection();
                        string query = $"UPDATE salesdata SET ClientIdentifier='{cbClientIdentifier.Text}',ItemDetails='{tbItemCodes.Text}'," +
                            $"OrderDate='{dtpOrderDate.Text}', NetTotal='{tbNetTotal.Text}',ProductBaseTotal='{tbProductValue.Text}'," +
                            $"Profit='{tbProfit.Text}',MonthName='{tbMonthName.Text}',Notes='{tbNotes.Text}' WHERE Invoice='{tbInvoice.Text}'";
                        db.Execute(query, true);
                        db.CloseConnection();
                        LoadDefault();
                        CustomerDue customerDue = new CustomerDue();
                        customerDue.CalculateDues();
                    }
                }
                else
                {
                    MessageBox.Show("Input all data properly.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbClientIdentifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = "SELECT * from customerinformation where ClientIdentifier='" + cbClientIdentifier.Text.ToString() + "'";
                var result = db.Retrieve(query);
                while (result.Read())
                {
                    tbClientName.Text = result.GetValue(1).ToString();
                    tbClientAddress.Text = result.GetValue(2).ToString();
                    tbClientPhone.Text = result.GetValue(3).ToString();
                }
                result.Close();
                db.CloseConnection();

                //getting previous due info from statementcustomerdue table
                lblDue.Text = "Due : " + new InformationRetriever().SingleDataGetter($"SELECT Due FROM statementcustomerdue WHERE ClientIdentifier='{cbClientIdentifier.Text}'");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadDefault();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbMonthName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate($"SELECT * from SalesData where MonthName='{cbMonthName.Text}' ORDER BY Invoice DESC", dgvSalesData);
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    dgvSalesData.Columns[6].Visible = false;
                    dgvSalesData.Columns[7].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFetch.PerformClick();
                e.SuppressKeyPress = true;
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
                        e.Graphics.DrawString($" : {tbInvoice.Text} [REPRINTED]", defaultFont, Brushes.Black, leftMargin + 150, currentY);//invoice number

                        currentY += lineHeight;
                        e.Graphics.DrawString($"DATE", defaultFont, Brushes.Black, leftMargin, currentY);//date //y=182
                        e.Graphics.DrawString($" : {dtpOrderDate.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);//date

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER NAME", defaultFont, Brushes.Black, leftMargin, currentY);//customer name //y=199
                        e.Graphics.DrawString($" : {tbClientName.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER ADDRESS", defaultFont, Brushes.Black, leftMargin, currentY);//customer address //y=216
                        e.Graphics.DrawString($" : {tbClientAddress.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER PHONE", defaultFont, Brushes.Black, leftMargin, currentY);//customer phone //y=233
                        e.Graphics.DrawString($" : {tbClientPhone.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

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
                        e.Graphics.DrawString($" : {tbInvoice.Text} [REPRINTED]", defaultFont, Brushes.Black, leftMargin + 150, currentY);//invoice number

                        currentY += lineHeight;
                        e.Graphics.DrawString($"DATE", defaultFont, Brushes.Black, leftMargin, currentY);//date //y=182
                        e.Graphics.DrawString($" : {dtpOrderDate.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);//date

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER NAME", defaultFont, Brushes.Black, leftMargin, currentY);//customer name //y=199
                        e.Graphics.DrawString($" : {tbClientName.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER ADDRESS", defaultFont, Brushes.Black, leftMargin, currentY);//customer address //y=216
                        e.Graphics.DrawString($" : {tbClientAddress.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

                        currentY += lineHeight;
                        e.Graphics.DrawString("CUSTOMER PHONE", defaultFont, Brushes.Black, leftMargin, currentY);//customer phone //y=233
                        e.Graphics.DrawString($" : {tbClientPhone.Text}", defaultFont, Brushes.Black, leftMargin + 150, currentY);

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
                        e.Graphics.DrawString(row.Cells[6]?.Value?.ToString() ?? string.Empty, defaultFont, Brushes.Black, 635, currentY);
                        e.Graphics.DrawString(row.Cells[7]?.Value?.ToString() ?? string.Empty, defaultFont, Brushes.Black, 695, currentY);

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

                    //getting previous due info
                    string previousDue = new InformationRetriever().SingleDataGetter("SELECT PreviousDue FROM SalesData WHERE Invoice='" + tbInvoice.Text + "'");
                    string paid = new InformationRetriever().SingleDataGetter("SELECT Paid_Amount FROM Transaction WHERE OrderId='" + tbInvoice.Text + "'");
                    if (previousDue == string.Empty)
                    {
                        previousDue = "0";
                    }
                    if (paid == string.Empty)
                    {
                        paid = "0";
                    }

                    currentY += lineHeight;
                    e.Graphics.DrawString($"PREVIOUS DUE", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {previousDue}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);

                    double grandTotal = Convert.ToDouble(tbNetTotal.Text) + Convert.ToDouble(previousDue);
                    currentY += lineHeight;
                    e.Graphics.DrawString($"GRAND TOTAL", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {grandTotal}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);

                    //Payment infotmation
                    currentY += lineHeight;
                    string paidDate = new InformationRetriever().SingleDataGetter("SELECT Date FROM Transaction WHERE OrderId='" + tbInvoice.Text + "'");
                    e.Graphics.DrawString($"PAID", defaultFont, Brushes.Black, leftMargin, currentY);
                    if (double.Parse(paid) > 0)
                    {
                        e.Graphics.DrawString($": {paid}/- tk on : {paidDate}", defaultFont, Brushes.Black, leftMargin + 100, currentY);
                    }
                    else
                    {
                        e.Graphics.DrawString($": {paid}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);
                    }

                    //getting discount info on the invoice number from Discount table
                    string discount = new InformationRetriever().SingleDataGetter("SELECT Amount FROM Discount WHERE OrderId='" + tbInvoice.Text + "'");
                    if (discount == string.Empty)
                    {
                        discount = "0";
                    }
                    currentY += lineHeight;
                    e.Graphics.DrawString($"DISCOUNT", defaultFont, Brushes.Black, leftMargin, currentY);
                    e.Graphics.DrawString($": {discount}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);

                    double Due = grandTotal - double.Parse(paid) - double.Parse(discount);
                    if (Due >= 0)
                    {
                        //due is positive
                        currentY += lineHeight;
                        e.Graphics.DrawString($"DUE", defaultFont, Brushes.Black, leftMargin, currentY);
                        e.Graphics.DrawString($": {Due}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);
                    }
                    else
                    {
                        //No due
                        currentY += lineHeight;
                        e.Graphics.DrawString($"DUE", defaultFont, Brushes.Black, leftMargin, currentY);
                        e.Graphics.DrawString($": 0/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);
                    }

                    if (Due < 0)
                    {
                        //change is positive
                        currentY += lineHeight;
                        e.Graphics.DrawString($"CHANGE", defaultFont, Brushes.Black, leftMargin, currentY);
                        e.Graphics.DrawString($": {Math.Abs(Due)}/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);
                    }
                    else
                    {
                        //no change
                        currentY += lineHeight;
                        e.Graphics.DrawString($"CHANGE", defaultFont, Brushes.Black, leftMargin, currentY);
                        e.Graphics.DrawString($": 0/-", defaultFont, Brushes.Black, leftMargin + 100, currentY);
                    }

                    if (tbNotes.Text.Length > 0)
                    {
                        currentY += lineHeight + 5;
                        e.Graphics.DrawString($"NOTES : {tbNotes.Text}", smallFontItalic, Brushes.Black, leftMargin, currentY);
                    }
                    // Draw footer and page number
                    DrawFooter(e, smallFont, leftMargin, pagenumber);
                    // When everything is printed, ensure we reset for future printing.
                    currentRowIndex = 0;
                    pagenumber = 1;
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
            e.Graphics.DrawString("PRINTED BY - " + Properties.Settings.Default.LoggedUserFullName, font, Brushes.Black, leftMargin, e.PageBounds.Bottom - 90);
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
                Image logo = Image.FromFile(imagePath);
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


        private void print_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbInvoice.Text != string.Empty)
                {
                    btnFetch.PerformClick();
                    printDialog1.Document = printDocument1;
                    printDocument1.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169); // A4 size
                    printDocument1.Print();
                }
                else
                {
                    MessageBox.Show("Enter Invoice number to print receipt", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpOrderDate_ValueChanged(object sender, EventArgs e)
        {
            tbMonthName.Text = new HelperClass().getFullMonthName(dtpOrderDate.Value.Month, dtpOrderDate.Value.Year);
        }

        private void tbNetTotal_TextChanged(object sender, EventArgs e)
        {
            //tbProfit.Text = (Convert.ToDouble(tbNetTotal.Text) - Convert.ToDouble(tbProductValue.Text)).ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            dgvReceipt.Rows.Clear();
            DgvReceiptFillup(tbItemCodes.Text);
        }

        private void tbInvoice_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvSalesData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                tbInvoice.Text = dgvSalesData.Rows[RowIndex].Cells[0].Value.ToString();
                btnFetch.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();
                db.DataGridViewPopulate("SELECT * from SalesData where clientIdentifier LIKE '%" + tbSearch.Text.ToString() + "%' ORDER BY Invoice DESC", dgvSalesData);
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    dgvSalesData.Columns[6].Visible = false;
                    dgvSalesData.Columns[7].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
