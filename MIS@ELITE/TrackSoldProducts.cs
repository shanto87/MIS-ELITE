using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class TrackSoldProducts : Form
    {
        private DataTable trackingTable;
        private BindingSource trackingSource;

        public TrackSoldProducts()
        {
            InitializeComponent();
            InitializeTrackingTable();
        }

        private void TrackSoldProducts_Load(object sender, EventArgs e)
        {
            LoadDefaultData();
        }

        private void InitializeTrackingTable()
        {
            trackingTable = new DataTable();
            trackingTable.Columns.Add("Invoice", typeof(string));
            trackingTable.Columns.Add("ClientIdentifier", typeof(string));
            trackingTable.Columns.Add("Quantity", typeof(int));
            trackingTable.Columns.Add("BaseValue", typeof(decimal));
            trackingTable.Columns.Add("Price", typeof(decimal));
            trackingTable.Columns.Add("Date", typeof(DateTime));
            trackingTable.Columns.Add("Notes", typeof(string));

            trackingSource = new BindingSource { DataSource = trackingTable };
            dgvTrackingResult.DataSource = trackingSource;
        }

        private void LoadDefaultData()
        {
            // This method can be used to load default data into the form controls
            // For example, loading a list of products or sales records
            try
            {
                lblCompanyName.Text = new InformationRetriever().SingleDataGetter("SELECT CompanyName FROM CompanyInfo WHERE ID=1");
                tbItemCode.Text = string.Empty;
                cbProductsList.Items.Clear();
                cbProductsList.Text = "";
                trackingTable.Clear();
                tbTrackingResult.Text = string.Empty;
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    chkbxConfidential.Visible = false; // Disable the checkbox for employees
                }
                else
                {
                    chkbxConfidential.Checked = true;
                    chkbxConfidential.Enabled = true; // Enable the checkbox for non-employees
                    chkbxConfidential.Visible = true;
                }
                var result = new DatabaseConnection().Retrieve("SELECT ProductName FROM StockInfo ORDER BY ProductName ASC");
                while (result.Read())
                {
                    cbProductsList.Items.Add(result.GetValue(0).ToString());
                }
                result.Close();

                //client information populate
                btnClearFilter.PerformClick(); // Clear any existing filters before loading new client data
                var clientResult = new DatabaseConnection().Retrieve("SELECT ClientIdentifier FROM CustomerInformation ORDER BY ClientIdentifier ASC");
                cbClientInfo.Items.Clear(); //clearing previous items
                cbClientInfo.Text = "";
                while (clientResult.Read())
                {
                    cbClientInfo.Items.Add(clientResult.GetValue(0).ToString());
                }
                clientResult.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading default data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadDefaultData();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                trackingTable.Clear(); //clear previous results
                DatabaseConnection db = new DatabaseConnection();
                string query = $"SELECT Invoice, ClientIdentifier, ItemDetails, OrderDate, Notes FROM SalesData WHERE ItemDetails LIKE '%|{tbItemCode.Text},%' ORDER BY INVOICE DESC";
                var result = db.Retrieve(query);
                if (!result.HasRows)
                {
                    MessageBox.Show("No sales records found for the given item code.", "No Records", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Properties.Settings.Default.AccessLevel == "EMPLOYEE")
                {
                    dgvTrackingResult.Columns["BaseValue"].Visible = false; // Hide the BaseValue column for employees
                }
                if (chkbxConfidential.Checked && Properties.Settings.Default.AccessLevel != "EMPLOYEE")
                {
                    dgvTrackingResult.Columns["BaseValue"].Visible = false; // Hide the BaseValue column for confidential view
                }
                while (result.Read())
                {
                    // Assuming ItemDetails is in the format "ItemID,Quantity,Price,BaseValue|ItemID,Quantity,Price,BaseValue|..."
                    // Extracting values from the result set
                    string invoice = result.GetValue(0).ToString();
                    string clientIdentifier = result.GetValue(1).ToString();
                    string ItemCodes = result.GetValue(2).ToString();
                    string date = result.GetValue(3).ToString();
                    string notes = result.GetValue(4).ToString();

                    // Split the ItemDetails string into individual items group
                    List<string> itemList;
                    itemList = ItemCodes.Split('|').ToList();
                    itemList.Remove("");

                    string itemID, itemQuantity, itemPrice, itemBaseValue;
                    List<string> tempArray;
                    foreach (string item in itemList)
                    {
                        // Split each item into its components
                        tempArray = item.Split(',').ToList();
                        itemID = tempArray[0];
                        // Check if the itemID matches the input item code
                        if (itemID != tbItemCode.Text)
                        {
                            continue;
                        }
                        itemQuantity = tempArray[1];
                        itemPrice = tempArray[2];
                        itemBaseValue = tempArray[3];
                        // Add the information row to the DataGridView
                        trackingTable.Rows.Add(invoice, clientIdentifier, int.Parse(itemQuantity), decimal.Parse(itemBaseValue), decimal.Parse(itemPrice), DateTime.Parse(date), notes);
                    }
                }
                // Update the tracking result textbox with the item details
                tbTrackingResult.Text = "Product Name : " + new InformationRetriever().SingleDataGetter($"SELECT ProductName FROM StockInfo WHERE ID='{tbItemCode.Text}'") + Environment.NewLine;
                tbTrackingResult.Text += $"Total Records Found : {trackingTable.Rows.Count}" + " Entries" + Environment.NewLine;
                tbTrackingResult.Text += $"Total Quantity Sold : {trackingTable.AsEnumerable().Sum(r => r.Field<int>("Quantity"))}" + " ps" + Environment.NewLine;
                tbTrackingResult.Text += $"Remaining Stock : {new InformationRetriever().SingleDataGetter($"SELECT Remains FROM StockInfo WHERE ID='{tbItemCode.Text}'")}" + " ps" + Environment.NewLine;
                tbTrackingResult.Text += $"Product Type : {new InformationRetriever().SingleDataGetter($"SELECT ProductType FROM StockInfo WHERE ID='{tbItemCode.Text}'")}" + Environment.NewLine;
                tbTrackingResult.Text += $"Last Shipment :{new InformationRetriever().SingleDataGetter($"SELECT ShipmentNoAndQty FROM StockInfo WHERE ID='{tbItemCode.Text}'")}" + Environment.NewLine;
                result.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while processing your request: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbProductsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbItemCode.Text = new InformationRetriever().SingleDataGetter($"SELECT ID FROM StockInfo WHERE ProductName='{cbProductsList.Text}'");
            tbItemCode.Focus();
            tbItemCode_KeyDown(sender, new KeyEventArgs(Keys.Enter)); // Simulate Enter key press to trigger search
        }

        private void tbItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent the 'ding' sound on Enter key
                btnSubmit.PerformClick(); // Trigger the submit button click event
            }
        }

        private void cbClientInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string clientIdentifier = cbClientInfo.Text;
            var bs = dgvTrackingResult.DataSource as BindingSource;
            if (bs != null)
            {
                bs.Filter = $"ClientIdentifier = '{clientIdentifier.Replace("'", "''")}'";
                return;
            }

            // Manual filter for unbound rows:
            foreach (DataGridViewRow row in dgvTrackingResult.Rows)
            {
                var cell = row.Cells["ClientIdentifier"].Value;
                row.Visible = (cell != null && cell.ToString() == clientIdentifier);
            }
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            var bs = dgvTrackingResult.DataSource as BindingSource;
            if (bs != null)
            {
                bs.RemoveFilter(); // or bs.Filter = string.Empty; }
            }
            cbClientInfo.Text = "";
        }

        private void dgvTrackingResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }
                int RowIndex = e.RowIndex;
                cbClientInfo.Text = dgvTrackingResult.Rows[RowIndex].Cells[1].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkbxConfidential_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkbxConfidential.Checked)
            {
                dgvTrackingResult.Columns["BaseValue"].Visible = true; // Show the BaseValue column when confidential view is unchecked
            }
            else
            {
                dgvTrackingResult.Columns["BaseValue"].Visible = false; // Hide the BaseValue column when confidential view is checked
            }
        }
    }
}
