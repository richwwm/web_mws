using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using LabelPrintingInterface.DataSource;

namespace LabelPrintingInterface
{
    public partial class ProductList : System.Web.UI.Page
    {
        DataSet PrintLabelDataSet = null;
        DataTable LabelTable = null;

        protected void Page_Init(object sender, EventArgs e)
        {
           
            //_connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            if (IsPostBack == false)
            {
                InitPrintList();

                // this.Master.LabelPrintDataSet = PrintLabelDataSet;
            }
            else
            {
                PrintLabelDataSet = (DataSet)Session["LabelPrintDataSet"];
                //PrintLabelDataSet = this.Master.LabelPrintDataSet;
            }
        }

        private void InitPrintList()
        {
            if (PrintLabelDataSet == null)
                PrintLabelDataSet = new DataSet();
            if (LabelTable == null)
                LabelTable = new DataTable("LabelPrintList");
            if (LabelTable.Columns.Count == 0)
            {
                LabelTable.Columns.Add("FNSKU", typeof(String));
                LabelTable.Columns.Add("SellerSKU", typeof(String));
                LabelTable.Columns.Add("ASIN", typeof(String));
                LabelTable.Columns.Add("PrintQuantity", typeof(Int32));
            }
            if (PrintLabelDataSet.Tables.Count == 0)
                PrintLabelDataSet.Tables.Add(LabelTable);

            Session["LabelPrintDataSet"] = PrintLabelDataSet;
        }

        private void ClearPrintList()
        {
            PrintLabelDataSet.Tables[0].Rows.Clear();
            Session["LabelPrintDataSet"] = PrintLabelDataSet;
        }

        private void AddToPrintList(string sFNSKU, string sSellerSKU, string sASIN, DataSet tempDataSet)
        {

            if (tempDataSet != null)
            {

            }
            else
            {
                tempDataSet = new DataSet();
            }

            if (tempDataSet.Tables.Count > 0)
            {
                DataRow[] rowsFind = tempDataSet.Tables[0].Select("FNSKU = " + "'" + sFNSKU + "'");
                if (rowsFind.Length == 0)
                {
                    DataRow newLabelRow = tempDataSet.Tables[0].NewRow();
                    newLabelRow["FNSKU"] = sFNSKU;
                    newLabelRow["SellerSKU"] = sSellerSKU;
                    newLabelRow["ASIN"] = sASIN;
                    newLabelRow["PrintQuantity"] = 0;

                    tempDataSet.Tables[0].Rows.Add(newLabelRow);
                }
            }
        }

        private void UpdatePrintListSource()
        {
            this.LabelPrintList.DataSource = (DataSet)Session["LabelPrintDataSet"];
            this.LabelPrintList.DataBind();
        }

        private void CancelUnexpectedRePost()
        {
            string clientCode = _repostcheckcode.Value;

            //Get Server Code from session (Or Empty if null)
            string serverCode = Session["_repostcheckcode"] as string ?? "";

            if (!IsPostBack || clientCode.Equals(serverCode))
            {
                //Codes are equals - The action was initiated by the user
                //Save new code (Can use simple counter instead Guid)
                string code = Guid.NewGuid().ToString();
                _repostcheckcode.Value = code;
                Session["_repostcheckcode"] = code;
            }
            else
            {
                //Unexpected action - caused by F5 (Refresh) button
                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //  CancelUnexpectedRePost();
        }

        private void UpdateCurrentQuantity2PrintList(ListView currentList)
        {
            DataSet tempDataSet = new DataSet();
            DataTable tempTable = new DataTable();
            tempTable.Columns.Add("FNSKU", typeof(String));
            tempTable.Columns.Add("SellerSKU", typeof(String));
            tempTable.Columns.Add("ASIN", typeof(String));
            tempTable.Columns.Add("PrintQuantity", typeof(Int32));
            foreach (ListViewItem itemRow in currentList.Items)
            {
                int iPrintQuantity = Int32.Parse((itemRow.FindControl("QuantityTextBox") as TextBox).Text);
                string sFNSKU = (itemRow.FindControl("FNSKULabel") as Label).Text.ToString();
                string sSellerSKU = (itemRow.FindControl("SellerSKULabel") as Label).Text.ToString();
                string sAsin = (itemRow.FindControl("ASINLabel") as HyperLink).Text.ToString();
                DataRow row = tempTable.NewRow();
                row["FNSKU"] = sFNSKU;
                row["SellerSKU"] = sSellerSKU;
                row["ASIN"] = sAsin;
                row["PrintQuantity"] = iPrintQuantity;
                tempTable.Rows.Add(row);
            }

            if (tempTable.Rows.Count > 0)
            {
                tempDataSet.Tables.Add(tempTable);
                Session["LabelPrintDataSet"] = tempDataSet;
            }
        }

        protected void MaxiumRecordTextBox_OnTextChanged(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                this.DataPager1.PageSize = Convert.ToInt32(t.Text);
            }
            catch (Exception ex)
            {
                this.DataPager1.PageSize = 10;
            }
        }

        protected void PrintAllButton_Click(object sender, EventArgs e)
        {
            //    UpdateCurrentQuantity2PrintList(this.LabelPrintList);
            //    DataSet tempDataSet = ((DataSet)Session["LabelPrintDataSet"]);
            //    if (tempDataSet != null && tempDataSet.Tables[0].Rows.Count > 0)
            //    {
            //        LabelReportHandler printAllLabel = new LabelReportHandler(tempDataSet);
            //        printAllLabel.PrintLabel();
            //    }
            if (!ClientScript.IsStartupScriptRegistered("alert"))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(),
                    "alert", "runNotepad();", true);
            }
        }

        protected void ClearPrintListButton_Click(object sender, EventArgs e)
        {
            ClearPrintList();
            UpdatePrintListSource();
            this.Label4.Visible = false;
        }

        protected void ListView1_ItemCommand1(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName != "Sort")
            {
                if (string.IsNullOrEmpty(e.CommandArgument.ToString()))
                    return;
                string[] commardArg = e.CommandArgument.ToString().Split(',');

                UpdateCurrentQuantity2PrintList(this.LabelPrintList);
                AddToPrintList(commardArg[0], commardArg[1], commardArg[2], (DataSet)Session["LabelPrintDataSet"]);
                UpdatePrintListSource();
                this.Label4.Visible = true;
            }

            if (e.CommandName == "Sort") // Process header sort command
            {
                string sExpression = e.CommandArgument.ToString();
                this.ListView1.Sort(sExpression, SortDirection.Descending);
            }
        }

        protected void LabelPrintList_ItemCommand1(object sender, ListViewCommandEventArgs e)
        {
            string sCommandType = e.CommandName.ToString();
            string sFNSKU = e.CommandArgument.ToString();
            DataSet tempDataSet = (DataSet)Session["LabelPrintDataSet"];
            if (sCommandType == "DeleteItem")
            {
                DataRow[] rowToDel = tempDataSet.Tables[0].Select("FNSKU = " + "'" + sFNSKU + "'");
                foreach (DataRow row in rowToDel)
                {
                    tempDataSet.Tables[0].Rows.Remove(row);
                }
                UpdatePrintListSource();
            }
        }

        protected void SearchImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            DataSet searchResultSet = null;
            string sKeyWord = this.SearchBox.Text;  //sKeyWord can be ASIN or FNSKU
            sKeyWord = sKeyWord.Trim();
            SellerInventorySupplyList getSupplistByInput = new SellerInventorySupplyList();
            if (!string.IsNullOrEmpty(sKeyWord))
            {
                searchResultSet = getSupplistByInput.GetDataByCriteria("FNSKU", sKeyWord);
                if (searchResultSet.Tables[0].Rows.Count == 0)
                {
                    searchResultSet = getSupplistByInput.GetDataByCriteria("ASIN", sKeyWord);
                    if (searchResultSet.Tables[0].Rows.Count == 0)
                    {
                        searchResultSet = getSupplistByInput.GetDataByCriteria("SellerSKU", sKeyWord);
                    }

                }
            }
            else
            {
                searchResultSet = getSupplistByInput.GetAllData();
            }
            if ((searchResultSet != null) && searchResultSet.Tables.Count > 0 && searchResultSet.Tables[0].Rows.Count > 0)
            {
                this.ListView1.DataSourceID = null;
                this.ListView1.DataSource = searchResultSet;
                this.ListView1.DataBind();
            }
        }

    }
}