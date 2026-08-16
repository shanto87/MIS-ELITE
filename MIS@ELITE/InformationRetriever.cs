using MySqlConnector;
using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    internal class InformationRetriever
    {
        private readonly DatabaseConnection db;

        public InformationRetriever()
        {
            db = new DatabaseConnection();
        }

        public string SingleDataGetter(string query)
        {
            try
            {
                using (var reader = db.Retrieve(query))
                {
                    if (reader != null && reader.Read())
                    {
                        return reader.GetValue(0).ToString();
                    }
                }
                return string.Empty; // no rows
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // error
            }
        }
    }
}
