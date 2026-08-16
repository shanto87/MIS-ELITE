using MIS_ELITE.Properties;
using MySqlConnector;
using System;
using System.Data;
using System.Windows.Forms;

namespace MIS_ELITE
{
    internal class DatabaseConnection
    {
        private readonly string connectionString;

        public DatabaseConnection()
        {
            string serverIp = Properties.Settings.Default.Server_IP;
            string serverUser = Properties.Settings.Default.Server_USER;
            string serverPassword = Properties.Settings.Default.Server_PASSWORD;
            string database = Properties.Settings.Default.Database;
            connectionString = $"Server={serverIp};User Id={serverUser};Password={serverPassword};Database={database};";
        }

        // Generic execute (optionally transactional)
        public void Execute(string query, bool useTransaction = false)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = useTransaction ? conn.BeginTransaction() : null)
                {
                    try
                    {
                        using (var cmd = new MySqlCommand(query, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        transaction?.Commit();
                        MessageBox.Show("Execution Successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Execute without alert
        public bool ExecuteWithoutAlert(string query, bool useTransaction = false)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = useTransaction ? conn.BeginTransaction() : null)
                {
                    try
                    {
                        using (var cmd = new MySqlCommand(query, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        transaction?.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        // Retrieve data (read-only)
        public MySqlDataReader Retrieve(string query)
        {
            try
            {
                var conn = new MySqlConnection(connectionString);
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Populate DataGridView
        public void DataGridViewPopulate(string query, DataGridView dgvName)
        {
            try
            {
                dgvName.DataSource = null;
                using (var dataAdapter = new MySqlDataAdapter(query, connectionString))
                {
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    dgvName.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Check server connectivity
        public string IsServerConnected()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return "true";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        // Row-lock aware stock update (reduce Remains, increase SOLD)
        public void UpdateStockWithRowLock(int skuId, int reduceBy)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Lock the row
                        string lockQuery = "SELECT Remains, SOLD FROM StockInfo WHERE ID = @sku FOR UPDATE";
                        using (var lockCmd = new MySqlCommand(lockQuery, conn, transaction))
                        {
                            lockCmd.Parameters.AddWithValue("@sku", skuId);
                            lockCmd.ExecuteScalar(); // acquire lock
                        }

                        // Step 2: Reduce stock and increase SOLD atomically
                        string updateQuery = @"
                            UPDATE StockInfo 
                            SET Remains = Remains - @reduce, 
                                SOLD = SOLD + @reduce 
                            WHERE ID = @sku AND Remains >= @reduce";
                        using (var updateCmd = new MySqlCommand(updateQuery, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@reduce", reduceBy);
                            updateCmd.Parameters.AddWithValue("@sku", skuId);
                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                                throw new Exception("Not enough stock available to reduce.");
                        }

                        // Step 3: Commit
                        transaction.Commit();
                        //MessageBox.Show("Stock reduced and SOLD updated safely.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void CloseConnection()
        {
            // No persistent connection to close since we're using 'using' statements
        }
    }
}
