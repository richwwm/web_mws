using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;
using System.Globalization;

namespace LabelPrintingInterface.Reports
{
    public partial class YearlyNetProfit : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!this.Page.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            else
            {
                Session["UserID"] = this.Page.User.Identity.Name;
            }

            Session["SortedBy"] = "";

        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == "" || Session["UserID"] == null)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

        }

        private Dictionary<string,string> getAllMerchantID2NameDictionaryByLoginID(string sID)
        {
            Dictionary<string, string> ditMerchantID2Name = new Dictionary<string, string>();
            AmazonAccountDataSource accountHandler = new AmazonAccountDataSource();
            DataSet ds =
            accountHandler.GetAmazonAccountListByUserID(sID);
            if (ds.Tables.Count != 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ditMerchantID2Name.Add(row["MerchantID"].ToString(),row["Description"].ToString());
                }
            }
            return ditMerchantID2Name;
        }
             
        private void GetAccountYearlyNetProfitData(string sMerchantID, int iTargetYear)
        {
            NetProfitDataSource netProfitHandler = new NetProfitDataSource();
            DataSet ds =
            netProfitHandler.GetAllSortedYearlyDataByMerchantID("", sMerchantID, iTargetYear);

        }

        private void loadDynamicGrid(int selectedYear)
        {
            #region Code for preparing the DataTable
            this.GridView1.Columns.Clear();
            //Create an instance of DataTable
            DataTable dt = new DataTable();
            NetProfitDataSource netProfitHandler = new NetProfitDataSource();
            Dictionary<string, string> ditMerchantID2Name = new Dictionary<string,string>();
            //Create an ID column for adding to the Datatable
            DataColumn dcol = new DataColumn("Month", typeof(System.String));
            dt.Columns.Add(dcol);
            if (Session["UserID"] != null)
            {
                ditMerchantID2Name = getAllMerchantID2NameDictionaryByLoginID(Session["UserID"].ToString());
                //Create an ID column for adding to the Datatable
                foreach (KeyValuePair<string, string> MerchantAccountPair in ditMerchantID2Name)
                {
                    dcol = new DataColumn(MerchantAccountPair.Key, typeof(System.String));
                    dt.Columns.Add(dcol);
                }
            }
            dcol = new DataColumn("TOTAL", typeof(System.String));
            dt.Columns.Add(dcol);
            for (int nIndex = 1; nIndex <= 12; nIndex++)
            {
                DataRow drow = dt.NewRow();
                drow[0] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(nIndex);

                foreach (KeyValuePair<string, string> MerchantAccountPair in ditMerchantID2Name)
                {
                    drow[MerchantAccountPair.Key] = "-";
                    DataSet ds =
                    netProfitHandler.GetAllSortedYearlyDataByMerchantID("", MerchantAccountPair.Key, selectedYear);
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                if ((int)row[0] == nIndex)
                                {
                                    double dProfit = (double)row[2];
                                    dProfit = Math.Round(dProfit, 2);
                                    drow[MerchantAccountPair.Key] = "$" + dProfit.ToString();
                                }
                            }
 
                        }
                    }
                }

                dt.Rows.Add(drow);
            }
            #endregion
            //calculate the total sum of netprofit 
            foreach (DataRow row in dt.Rows)
            {
                double dTotal = 0;
                for (int i = 0; i < row.ItemArray.Length ; i++)
                {
                    double dNetProfit = 0;
                    if (i == row.ItemArray.Length - 1)
                    {
                        if (dTotal == 0)
                            row[i] = "-";
                        else
                            row[i] = "$"+ dTotal;
                    }
                    else if (i != 0)
                    {
                        if (row[i] != "-")
                        {
                            string sNetProftString = row[i].ToString();
                            dNetProfit = double.Parse(sNetProftString.Substring(1));
   
                        }
                        dTotal = dTotal + dNetProfit;
                    }
                }

   
            }

            //Iterate through the columns of the datatable to set the data bound field dynamically.
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == "Month")
                {
                    //Declare the bound field and allocate memory for the bound field.
                    BoundField bfield = new BoundField();

                    //Initalize the DataField value.
                    bfield.DataField = col.ColumnName;

                    //Initialize the HeaderText field value.
                    bfield.HeaderText = col.ColumnName;

                    //Add the newly created bound field to the GridView.
                    GridView1.Columns.Add(bfield);
                } 
                else if (col.ColumnName == "TOTAL")
                {
                    BoundField bfield = new BoundField();

                    //Initalize the DataField value.
                    bfield.DataField = col.ColumnName;

                    //Initialize the HeaderText field value.
                    bfield.HeaderText = col.ColumnName;

                    //Add the newly created bound field to the GridView.
                    GridView1.Columns.Add(bfield);
                } 
                else
                {
                    ButtonField buttonField = new ButtonField();
                    buttonField.ButtonType = ButtonType.Link;
                    buttonField.DataTextField = col.ColumnName;
                    buttonField.HeaderText =ditMerchantID2Name[col.ColumnName];
                    GridView1.Columns.Add(buttonField);
                }

            }

            //Initialize the DataSource
            this.GridView1.DataSource = dt;

            //Bind the datatable with the GridView.
            this.GridView1.DataBind();
        }

        private string RemoveExtraText(string value)
        {
            var allowedChars = "-01234567890.,";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        protected void DropDownList1_TextChanged(object sender, EventArgs e)
        {
            this.DropDownList2.DataSourceID = "";
            this.DropDownList2.DataSource = null;
            this.NetProfitYearPeriodDataSource1.Select();
            this.DropDownList2.DataSource = this.NetProfitYearPeriodDataSource1;
            this.DropDownList2.DataBind();
        }

        protected void NetProfitDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!this.Page.User.Identity.IsAuthenticated)
            {
                e.Cancel = true;
            }
        }

        protected void AmazonAccountDataSource2_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!this.Page.User.Identity.IsAuthenticated)
            {
                e.Cancel = true;
            }
        }

        protected void DropDownList2_TextChanged(object sender, EventArgs e)
        {
            LoadNetProfitData();
        }

        protected void DropDownList2_DataBinding(object sender, EventArgs e)
        {

        }

        protected void DropDownList2_PreRender(object sender, EventArgs e)
        {
            LoadNetProfitData();
        }

        private void LoadNetProfitData()
        {
            int iTargetYear = 2014;
            try
            {
                iTargetYear = int.Parse(this.DropDownList2.Text);
            }
            catch (Exception ex)
            {

            }
            loadDynamicGrid(iTargetYear);
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    int iCellIndex = e.Row.Cells.GetCellIndex(cell);
                    if (cell !=e.Row.Cells[0] && cell != e.Row.Cells[e.Row.Cells.Count -1] ) // not the month column & TOTAL column
                    {
                        LinkButton LinkButton1 = (LinkButton)cell.Controls[0];
                        LinkButton1.Enabled = false;
                        if (LinkButton1.Text != "-")
                        {
                            int iSelectedMonth = e.Row.RowIndex + 1;
                            DataTable dataTable = (DataTable) this.GridView1.DataSource;
                            if (dataTable != null)
                            {
                                string sMerchantID = dataTable.Columns[iCellIndex].ColumnName;
                                LinkButton1.CommandArgument = this.DropDownList2.Text + "," + iSelectedMonth + "," + sMerchantID;
                                LinkButton1.CommandName = "SelectMonthlyNetProfit";
                                LinkButton1.Enabled = true;
                            }
                         }
                    }
                }
            }
        }

        protected void GridView1_RowCommand1(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectMonthlyNetProfit")
            {
                string sArgument = e.CommandArgument.ToString();
                string[] sVal = sArgument.Split(',');
                if (sVal.Length == 3)
                {
                    Session["Selected_Year"] = sVal[0];
                    Session["Selected_Month"] = sVal[1];
                    Session["Selected_MerchantID"] = sVal[2];
                    Response.Redirect("~/MonthlyNetProfit.aspx");
                }
            }
        }

        protected void DropDownList2_DataBound(object sender, EventArgs e)
        {
            if (Session["Selected_Year"] != null)
            {
                this.DropDownList2.SelectedValue = Session["Selected_Year"].ToString();

            }
        }
    }
}