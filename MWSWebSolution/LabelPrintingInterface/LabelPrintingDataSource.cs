using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LabelPrintingInterface.DataSource
{
    public class SellerInventorySupplyList
    {
        string _connectionString;
        public SellerInventorySupplyList()
        {
            //use local db file to debug
            //_connectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=D:\\Richard\\progarm_src\\mws\\mws\\MWSWebSolution\\MWSServer\\bin\\debug\\Database1.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
            //use production database
            _connectionString = "Data Source=192.168.103.150\\INFLOWSQL;Initial Catalog=MWS;User ID=mws;Password=p@ssw0rd";
        }

        public DataSet GetAllSortedData(string sortExpression)
        {
            sortExpression = sortExpression.Replace("Ascending", "ASC");
            sortExpression = sortExpression.Replace("Descending", "DESC");
            string query = "";
            SqlCommand cmd;


            query = "SELECT * FROM ProductAvailability order by " + sortExpression;
            cmd = new SqlCommand(query);


            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                DataTable dTable = new DataTable();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(dTable);
                            ds.Tables.Add(dTable);
                            return ds;
                        }
                    }
                }
            }
        }

        public DataSet GetAllData()
        {
            string query = "SELECT * FROM ProductAvailability";
            SqlCommand cmd = new SqlCommand(query);
            DataTable dTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataSet ds = new DataSet())
                    {
                        sda.Fill(dTable);
                        ds.Tables.Add(dTable);
                        return ds;
                    }
                }
            }


        }

        public DataSet GetDataByCriteria(string parameter, string value)
        {
            string query = "SELECT * FROM ProductAvailability where " + parameter + " LIKE '%'+@parameter+'%'";
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
    }
}