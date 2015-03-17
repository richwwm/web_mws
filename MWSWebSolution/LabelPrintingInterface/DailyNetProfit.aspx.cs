using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;

namespace LabelPrintingInterface.Reports
{
    public partial class DailyNetProfit : System.Web.UI.Page
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

        private string RemoveExtraText(string value)
        {
            var allowedChars = "-01234567890.,";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }
        
        protected void ListView1_PreRender(object sender, EventArgs e)
        {

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

        protected void DataPager1_PreRender(object sender, EventArgs e)
        {
            this.ListView1.DataSourceID = null;
            this.ListView1.DataSource = this.NetProfitDataSource;
            this.ListView1.DataBind();

            double dPageSubProfitTotal = 0;

            foreach (ListViewDataItem Item in this.ListView1.Items)
            {
                Label NetProfitLabel = Item.FindControl("NetProfitLabel") as Label;
                 double dNetProfitTotal = 0;
                if(NetProfitLabel.Text != "")
                    dNetProfitTotal = Convert.ToDouble(RemoveExtraText(NetProfitLabel.Text));

                dPageSubProfitTotal += dNetProfitTotal;
            }
            Label totalNetProfitLbl = (Label)this.ListView1.FindControl("SubNetProfitTotalTextLabel");


            if (null != totalNetProfitLbl)
                totalNetProfitLbl.Text = "Sub Total:  "+dPageSubProfitTotal.ToString("C");
        }

        protected void ListViewSKUImageButton_OnClick(object sender, EventArgs e)
        {
            //string sExpression = "ProductName ASC";
            //Session["SortedBy"] = sExpression;
            //this.ListView1.DataSourceID = null;
            //this.NetProfitDataSource.Select();
            //this.ListView1.DataSource = this.NetProfitDataSource;
            //this.ListView1.DataBind();
        }

        protected void ListViewNetProfitImageButton_OnClick(object sender, EventArgs e)
        {
            string sExpression = "NetProfit_USD DESC";
            Session["SortedBy"] = sExpression;
            this.ListView1.DataSourceID = null;
            this.NetProfitDataSource.Select();
            this.ListView1.DataSource = this.NetProfitDataSource;
            this.ListView1.DataBind();
        }

        protected void ListView1_Sorting(object sender, ListViewSortEventArgs e)
        {
            //this cannot be deleted , to handle the sorting event
        }
        
        protected void DropDownList1_TextChanged(object sender, EventArgs e)
        {
            Session["Selected_MerchantID"] = null;
            Session["Selected_Date"] = null;
            this.DropDownList2.DataSourceID = "";
            this.DropDownList2.DataSource = null;
            this.NetProfitDayPeriodDataSource1.Select();
            this.DropDownList2.DataSource = this.NetProfitDayPeriodDataSource1;
            this.DropDownList2.DataBind();
        }


        protected void DropDownList2_TextChanged(object sender, EventArgs e)
        {
            Session["Selected_Date"] = null;
            this.ListView1.DataSourceID = "";
            this.ListView1.DataSource = null;
            this.NetProfitDataSource.Select();
            this.ListView1.DataSource = this.NetProfitDataSource;
            this.ListView1.DataBind();
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


        protected void DropDownList2_DataBinding(object sender, EventArgs e)
        {

        }

        protected void DropDownList2_PreRender(object sender, EventArgs e)
        {
            this.ListView1.DataSourceID = "";
            this.ListView1.DataSource = null;
            this.NetProfitDataSource.Select();
            this.ListView1.DataSource = this.NetProfitDataSource;
            this.ListView1.DataBind();
        }

        protected void DropDownList2_DataBound(object sender, EventArgs e)
        {
            if (Session["Selected_Date"] != null)
            {
                string sDate = Session["Selected_Date"].ToString();
                
                ListItem selectedItem = this.DropDownList2.Items.FindByText(sDate);
                if(selectedItem != null)
                {
                    selectedItem.Selected = true;
                }


            }
        }

        protected void DropDownList1_DataBound(object sender, EventArgs e)
        {
            if (Session["Selected_MerchantID"] != null)
            {
                this.DropDownList1.SelectedValue = Session["Selected_MerchantID"].ToString();
            }
        }

        protected void ListView1_DataBound(object sender, EventArgs e)
        {
            double dPageSubProfitTotal = 0;

            foreach (ListViewDataItem Item in this.ListView1.Items)
            {
                Label NetProfitLabel = Item.FindControl("NetProfitLabel") as Label;
                double dNetProfitTotal = 0;
                if (NetProfitLabel.Text != "")
                    dNetProfitTotal = Convert.ToDouble(RemoveExtraText(NetProfitLabel.Text));

                dPageSubProfitTotal += dNetProfitTotal;
            }
            Label totalNetProfitLbl = (Label)this.ListView1.FindControl("SubNetProfitTotalTextLabel");


            if (null != totalNetProfitLbl)
                totalNetProfitLbl.Text = "Sub Total:  " + dPageSubProfitTotal.ToString("C");
        }


    }
}