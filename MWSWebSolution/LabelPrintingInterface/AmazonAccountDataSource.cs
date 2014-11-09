using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace LabelPrintingInterface
{
    public class AmazonAccountDataSource
    {
        public AmazonAccountDataSource()
        {
            System.Configuration.Configuration rootWebConfig =
               System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/WebSiteName");
            //check if the AppSettings section has items
            if (rootWebConfig.AppSettings.Settings.Count > 0)
            {
                _connectionString = "Data Source=192.168.103.150\\INFLOWSQL;User ID=mws;Password=p@ssw0rd";
            }
        }

        string _connectionString =  "";

        public DataSet GetAmazonAccountListByUserID(string sUserID)
        {
            DataTable dTable = new DataTable();
            DataSet ds = new DataSet();
            ds.Tables.Add(dTable);
            if (!string.IsNullOrEmpty(sUserID))
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        SqlCommand cmd = new SqlCommand("GetAmazonAccountListByUserID");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", sUserID);
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dTable);

                    }
                }
            }

            return ds;
        }
    }
}