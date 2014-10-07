using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using DebugLogHandler;
using System.IO;
namespace MWSServer
{
    class SqlDataHandler
    {
        string _connectionString;
        string _sClass = "SqlDataHandler.cs";
        string _sLogPath = Directory.GetCurrentDirectory() + "\\";
        public SqlDataHandler(string sConnectionString)
        {
            _connectionString = sConnectionString;
        }

        public void UpdateData(string sTableName, DataSet dsTemp)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            string insertQueryString = "INSERT INTO " + sTableName + " (FNSKU, SellerSKU,ASIN,ProductName,Inbound,Fulfillable,Unfulfillable,Reserved,Last_update) " +
        " VALUES (@FNSKU, @SellerSKU,@ASIN,@ProductName,@Inbound,@Fulfillable,@Unfulfillable,@Reserved,@Last_update)";

            string updateQueryString = "UPDATE " + sTableName + " SET Inbound = @Inbound, Fulfillable = @Fulfillable,  Unfulfillable = @Unfulfillable,  Reserved = @Reserved, Last_update = @Last_update" +
        " WHERE FNSKU = @FNSKU";

            using (SqlConnection connection =
            new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    foreach (DataRow row in dsTemp.Tables[0].Rows)
                    {
                        SqlCommand command = new SqlCommand(updateQueryString, connection);
                        command.Parameters.AddWithValue("@FNSKU", row["FNSKU"].ToString());
                        command.Parameters.AddWithValue("@Inbound", row["Inbound"].ToString());
                        command.Parameters.AddWithValue("@Fulfillable", row["Fulfillable"].ToString());
                        command.Parameters.AddWithValue("@Unfulfillable", row["Unfulfillable"].ToString());
                        command.Parameters.AddWithValue("@Reserved", row["Reserved"].ToString());
                        command.Parameters.AddWithValue("@Last_update", DateTime.Now);

                        int result = command.ExecuteNonQuery(); //number of rows returned.
                        if (result == 0) //no data updated , thus insert new data to database;
                        {
                            SqlCommand insertCommand = new SqlCommand(insertQueryString, connection);
                            insertCommand.Parameters.AddWithValue("@FNSKU", row["FNSKU"].ToString());
                            insertCommand.Parameters.AddWithValue("@SellerSKU", row["SellerSKU"].ToString());
                            insertCommand.Parameters.AddWithValue("@ASIN", row["ASIN"].ToString());
                            insertCommand.Parameters.AddWithValue("@ProductName", row["ProductName"].ToString());
                            insertCommand.Parameters.AddWithValue("@Inbound", row["Inbound"].ToString());
                            insertCommand.Parameters.AddWithValue("@Fulfillable", row["Fulfillable"].ToString());
                            insertCommand.Parameters.AddWithValue("@Unfulfillable", row["Unfulfillable"].ToString());
                            insertCommand.Parameters.AddWithValue("@Reserved", row["Reserved"].ToString());
                            insertCommand.Parameters.AddWithValue("@Last_update", DateTime.Now);
                            result = insertCommand.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateData() exception message: " + ex.Message);
                }
            }

        }


    }
}
