using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using DebugLogHandler;
using System.IO;

namespace MWS.Lib
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

        public string GetMerchantIDByUserID(string sUserID)
        {
            string constr = _connectionString;
            string sMerchantID = "";
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("GetUserMerchantID"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", sUserID);
                        cmd.Connection = con;
                        con.Open();
                        sMerchantID = cmd.ExecuteScalar().ToString();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
 
            }
            return sMerchantID;
        }



    }
}
