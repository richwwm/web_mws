using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using DebugLogHandler;
using System.IO;
using MWSUser;
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

        public List<MWSUserProfile> GetAWSLoginProfile()
        {
            List<MWSUserProfile> profileList = new List<MWSUserProfile>();
            SqlCommand cmd;
            string query;

            query = "SELECT * FROM [MWS].[dbo].[AWSLoginToken]";
            cmd = new SqlCommand(query);

            DataTable dTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(dTable);
                        }
                    }
                }
            }

            foreach (DataRow row in dTable.Rows)
            {
                string sAccessKeyID = row["AWSAccessKeyID"].ToString();
                string sSecretKey = row["SecretKey"].ToString();
                string sMerchantID = row["MerchantID"].ToString();
                string sMarketplaceID = row["MarketplaceID"].ToString();
                MWSUserProfile profile = new MWSUserProfile(sAccessKeyID, sSecretKey, sMarketplaceID, sMerchantID, "");
                profileList.Add(profile);
            }
           
            return profileList;
        }

        public void UpdateData(string sTableName, DataSet dsTemp)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            string insertQueryString = "INSERT INTO " + sTableName + " (FNSKU, SellerSKU,ASIN,ProductName,Inbound,Fulfillable,Unfulfillable,Reserved,Last_update,MerchantID) " +
        " VALUES (@FNSKU, @SellerSKU,@ASIN,@ProductName,@Inbound,@Fulfillable,@Unfulfillable,@Reserved,@Last_update,@MerchantID)";

            string updateQueryString = "UPDATE " + sTableName + " SET Inbound = @Inbound, Fulfillable = @Fulfillable,  Unfulfillable = @Unfulfillable,  Reserved = @Reserved, Last_update = @Last_update" +
        " WHERE FNSKU = @FNSKU and MerchantID = @MerchantID";

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
                        command.Parameters.AddWithValue("@MerchantID", row["MerchantID"].ToString());

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
                            insertCommand.Parameters.AddWithValue("@MerchantID", row["MerchantID"].ToString());
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
