using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;

namespace LabelPrintingInterface
{
    public partial class NetProfitSimPage : System.Web.UI.Page
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

            loadDynamicGrid();
        }

        private DataTable GetFileList(string sSortExpression = "")
        {
            DataTable dt = new DataTable();
            
            return dt;
        }

        private void loadDynamicGrid()
        {
            #region Code for preparing the DataTable
            this.GridView1.Columns.Clear();
            //Create an instance of DataTable
            DataTable dt = GetFileList();
            NetProfitDataSource netProfitHandler = new NetProfitDataSource();
            Dictionary<string, string> ditMerchantID2Name = new Dictionary<string, string>();
            //Create an ID column for adding to the Datatable
            DataColumn dcol = new DataColumn("Date", typeof(System.String));
            dt.Columns.Add(dcol);
            dcol = new DataColumn("File", typeof(System.String));
            dt.Columns.Add(dcol);


            DataRow tempRow = dt.NewRow();
            tempRow[0] = "2/3/2015";
            tempRow[1] = "AppeagleExport_201503011310_Fung.csv";
            dt.Rows.Add(tempRow);
            DataRow tempRow2 = dt.NewRow();
            tempRow2[0] = "2/3/2015";
            tempRow2[1] = "AppeagleExport_201503012210_KM.csv";
            dt.Rows.Add(tempRow2);
            DataRow tempRow3 = dt.NewRow();
            tempRow3[0] = "1/3/2015";
            tempRow3[1] = "ProfitSim_201503011215_KM.xlsx";  
            dt.Rows.Add(tempRow3);
            foreach (DataRow row in dt.Rows)
            {
                
            }

            #endregion


            //Iterate through the columns of the datatable to set the data bound field dynamically.
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == "Date")
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
                else
                {
                    ButtonField buttonField = new ButtonField();
                    buttonField.ButtonType = ButtonType.Link;
                    buttonField.DataTextField = col.ColumnName;
                    buttonField.HeaderText = col.ColumnName;
                    GridView1.Columns.Add(buttonField);
                }

            }

            //Initialize the DataSource
            this.GridView1.DataSource = dt;

            //Bind the datatable with the GridView.
            this.GridView1.DataBind();
        }

        protected void AmazonAccountCheckBoxList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {

        }


        protected void GridView1_RowCommand1(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectMonthlyNetProfit")
            {
                //string sArgument = e.CommandArgument.ToString();
                //string[] sVal = sArgument.Split(',');
                //if (sVal.Length == 3)
                //{
                //    Session["Selected_Year"] = sVal[0];
                //    Session["Selected_Month"] = sVal[1];
                //    Session["Selected_MerchantID"] = sVal[2];
                //    Response.Redirect("~/MonthlyNetProfit.aspx");
                //}
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    //int iCellIndex = e.Row.Cells.GetCellIndex(cell);
                    //if (cell != e.Row.Cells[0] && cell != e.Row.Cells[e.Row.Cells.Count - 1]) // not the month column & TOTAL column
                    //{
                    //    LinkButton LinkButton1 = (LinkButton)cell.Controls[0];
                    //    LinkButton1.Enabled = false;
                    //    if (LinkButton1.Text != "-")
                    //    {
                    //        int iSelectedMonth = e.Row.RowIndex + 1;
                    //        DataTable dataTable = (DataTable)this.GridView1.DataSource;
                    //        if (dataTable != null)
                    //        {
                    //            string sMerchantID = dataTable.Columns[iCellIndex].ColumnName;
                    //            LinkButton1.CommandArgument = this.DropDownList2.Text + "," + iSelectedMonth + "," + sMerchantID;
                    //            LinkButton1.CommandName = "SelectMonthlyNetProfit";
                    //            LinkButton1.Enabled = true;
                    //        }
                    //    }
                    //}
                }
            }
        }

        protected void AmazonAccountCheckBoxList_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem itm in AmazonAccountCheckBoxList.Items)
            {
                itm.Selected = true;
            }
        }


        private Dictionary<string, string> getAllMerchantID2NameDictionaryByLoginID(string sID)
        {
            Dictionary<string, string> ditMerchantID2Name = new Dictionary<string, string>();
            AmazonAccountDataSource accountHandler = new AmazonAccountDataSource();
            DataSet ds =
            accountHandler.GetAmazonAccountListByUserID(sID);
            if (ds.Tables.Count != 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ditMerchantID2Name.Add(row["MerchantID"].ToString(), row["Description"].ToString());
                }
            }
            return ditMerchantID2Name;
        }

        protected void GenerateNewFileButton_Click(object sender, EventArgs e)
        {

        }

    }
}