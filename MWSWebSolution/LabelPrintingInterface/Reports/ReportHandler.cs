using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LabelPrintingInterface
{
    public class ReportHandler
    {
        string _connectionString;
        public ReportHandler()
        {
            //use local db file to debug
            //_connectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=D:\\Richard\\progarm_src\\mws\\mws\\MWSWebSolution\\MWSServer\\bin\\Release\\Database1.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
            //use production database
            _connectionString = "Data Source=192.168.103.150\\INFLOWSQL;User ID=mws;Password=p@ssw0rd";
        }

        public DataSet GetDataByStoredProcedure(string sStoredProcedureName, SqlParameter[] paraCollection)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sStoredProcedureName))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter para in paraCollection)
                    {
                        cmd.Parameters.Add(para);
                    }
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {      
                            sda.Fill(ds);
                    }
                    con.Close();
                }
            }
            
            return ds;
        }

        public DataSet GetAllData(string query)
        {
            SqlCommand cmd = new SqlCommand(query);

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataSet ds = new DataSet())
                    {
                        sda.Fill(ds);
                        return ds;
                    }
                }
            }


        }

        public DataSet GetDataByCriteria(string query, string value)
        {
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@parameter", value);
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataSet ds = new DataSet())
                    {
                        sda.Fill(ds);
                        return ds;
                    }
                }
            }
        }

        //    public DataSet GetDataByCriteria(string parameter, string value)
        //    {
        //        string query = "SELECT [MWS].[dbo].[ProductAvailability].[FNSKU] AS " + '"' + "FNSKU" + '"'
        //+ ",[inFlow].[dbo].[BASE_Product].[Name] AS " + '"' + "Product Name" + '"'
        //+ ",ISNULL([inFlow].[dbo].[Base_InventoryCost].[AverageCost],0) AS " + '"' + "Cost" + '"'
        //+ ",[MWS].[dbo].[ProductAvailability].[Inbound] AS " + '"' + "Inbound" + '"'
        //+ ",[MWS].[dbo].[ProductAvailability].[Fulfillable] AS " + '"' + "Fulfillable" + '"'
        //+ "FROM [inFlow].[dbo].[BASE_Product] "
        //+ "INNER JOIN [inFlow].[dbo].[BASE_InventoryCost] ON [inFlow].[dbo].[BASE_Product].[ProdId]=[inFlow].[dbo].[BASE_InventoryCost].[ProdId] "
        //+ "INNER JOIN [MWS].[dbo].[ProductAvailability] ON [MWS].[dbo].[ProductAvailability].SellerSKU = [inFlow].[dbo].[BASE_Product].[Name] "
        //+ "WHERE [inFlow].[dbo].[BASE_InventoryCost].[CurrencyId] = '8'"
        //+ "AND (" + parameter + " LIKE '%'+@parameter+'%')";
        //        SqlCommand cmd = new SqlCommand(query);
        //        cmd.Parameters.AddWithValue("@parameter", value);
        //        using (SqlConnection con = new SqlConnection(_connectionString))
        //        {
        //            using (SqlDataAdapter sda = new SqlDataAdapter())
        //            {
        //                cmd.Connection = con;
        //                sda.SelectCommand = cmd;
        //                using (DataSet ds = new DataSet())
        //                {
        //                    sda.Fill(ds);
        //                    return ds;
        //                }
        //            }
        //        }
        //    }
    }
}