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
    public partial class TotalCurrentInventory : System.Web.UI.Page
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

        private void DoSearchProduct()
        {
            string sKeyWord = this.SearchBox.Text;  //sKeyWord can be ASIN or FNSKU
            sKeyWord = sKeyWord.Trim();
            this.ListView1.DataSourceID = null;
            if (!string.IsNullOrEmpty(sKeyWord))
                this.ObjectDataSource1.FilterExpression = "FNSKU LIKE " + "'%" + sKeyWord + "%'";
            else
                this.ObjectDataSource1.FilterExpression = "";
            this.ObjectDataSource1.Select();
            this.ListView1.DataSource = this.ObjectDataSource1;

            this.ListView1.DataBind();

            if (this.ListView1.Items.Count == 0)
            {
                this.ObjectDataSource1.FilterExpression = "SellerSKU LIKE " + "'%" + sKeyWord + "%'";
                this.ObjectDataSource1.Select();
                this.ListView1.DataSource = this.ObjectDataSource1;

                this.ListView1.DataBind();
            }

            if (this.ListView1.Items.Count == 0)
            {
                this.ObjectDataSource1.FilterExpression = "ProductTitle LIKE " + "'%" + sKeyWord + "%'";
                this.ObjectDataSource1.Select();
                this.ListView1.DataSource = this.ObjectDataSource1;

                this.ListView1.DataBind();
            }
        }

        protected void SearchImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            DoSearchProduct();
        }

        private string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890.,";
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
            this.ListView1.DataSource = this.ObjectDataSource1;
            this.ListView1.DataBind();

            double dPageTotal = 0;
            double dPageInboundTotal = 0;
            double dPageFulfillableTotal = 0;
            foreach (ListViewDataItem Item in this.ListView1.Items)
            {
                Label InboundTotalLabel = Item.FindControl("InboundTotalLabel") as Label;
                Label FulfillableTotalLabel = Item.FindControl("FulfillableTotalLabel") as Label;
                Label SubTotalLabel = Item.FindControl("SubTotalLabel") as Label;

                double dInboundTotal = Convert.ToDouble(RemoveExtraText(InboundTotalLabel.Text));
                double dFulfillableTotal = Convert.ToDouble(RemoveExtraText(FulfillableTotalLabel.Text));
                double dSubTotal = Convert.ToDouble(RemoveExtraText(SubTotalLabel.Text));
                dPageTotal += dSubTotal;
                dPageInboundTotal += dInboundTotal;
                dPageFulfillableTotal += dFulfillableTotal;
            }
            Label totalInboundLbl = (Label)this.ListView1.FindControl("TotalInboundLabel");
            Label totalFulfillableLbl = (Label)this.ListView1.FindControl("TotalFulfillableLabel");
            Label totalCountLbl = (Label)this.ListView1.FindControl("TotalCountLabel");

            if(null != totalInboundLbl)
                totalInboundLbl.Text = dPageInboundTotal.ToString("C");
            if (null != totalFulfillableLbl)
                totalFulfillableLbl.Text = dPageFulfillableTotal.ToString("C");
            if (null != totalCountLbl)
                totalCountLbl.Text = dPageTotal.ToString("C");
        }

        protected void ListView1tiesChanged(object sender, EventArgs e)
        {

        }

        protected void ListView1_ItemCommand1(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort") // Process header sort command
            {
                string sExpression = e.CommandArgument.ToString();
                Session["SortedBy"] = sExpression;
                this.ListView1.DataSourceID = null;
                string sKeyWord = this.SearchBox.Text.ToString();
                if (!string.IsNullOrEmpty(sKeyWord))
                {
                    this.ObjectDataSource1.FilterExpression = "FNSKU LIKE " + "'%" + sKeyWord + "%'";
                }
                else
                {
                    this.ObjectDataSource1.FilterExpression = "";
                }
                this.ObjectDataSource1.Select();
                this.ListView1.DataSource = this.ObjectDataSource1;
                this.ListView1.DataBind();

                if (this.ListView1.Items.Count == 0)
                {
                    this.ObjectDataSource1.FilterExpression = "SellerSKU LIKE " + "'%" + sKeyWord + "%'";
                    this.ObjectDataSource1.Select();
                    this.ListView1.DataSource = this.ObjectDataSource1;
                    this.ListView1.DataBind();
                }

                if (this.ListView1.Items.Count == 0)
                {
                    this.ObjectDataSource1.FilterExpression = "ProductTitle LIKE " + "'%" + sKeyWord + "%'";
                    this.ObjectDataSource1.Select();
                    this.ListView1.DataSource = this.ObjectDataSource1;
                    this.ListView1.DataBind();
                }
            }
            else
                Session["SortedBy"] = "";
        }

        protected void ListView1_Sorting(object sender, ListViewSortEventArgs e)
        {
            //this cannot be deleted , to handle the sorting event
        }
        
        protected void DropDownList1_TextChanged(object sender, EventArgs e)
        {
            this.ListView1.DataSource = null;
            this.ObjectDataSource1.Select();
            this.ListView1.DataSource = this.ObjectDataSource1;
            this.ListView1.DataBind();
            DoSearchProduct();
        }

        protected void ObjectDataSource1_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!this.Page.User.Identity.IsAuthenticated)
            {
                e.Cancel = true;
            }
        }

        protected void ObjectDataSource2_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!this.Page.User.Identity.IsAuthenticated)
            {
                e.Cancel = true;
            }
        }

    }
}