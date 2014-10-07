using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace LabelPrintingInterface.Reports
{
    public partial class TotalCurrentInventory : System.Web.UI.Page
    {
        double dPageTotal = 0;
        double dPageInboundTotal = 0;
        double dPageFulfillableTotal = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void SearchImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (e.X > 0 && e.Y > 0)
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
                    this.ObjectDataSource1.FilterExpression = "[Product Name] LIKE " + "'%" + sKeyWord + "%'";
                    this.ObjectDataSource1.Select();
                    this.ListView1.DataSource = this.ObjectDataSource1;

                    this.ListView1.DataBind();
                }
            }
        }

        private string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890.,";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        protected void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
        {

            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Label CostLabel = e.Item.FindControl("CostLabel") as Label;
                Label InboundLabel = e.Item.FindControl("InboundLabel") as Label;
                Label InboundTotalLabel = e.Item.FindControl("InboundTotalLabel") as Label;
                Label FulfillableLabel = e.Item.FindControl("FulfillableLabel") as Label;
                Label FulfillableTotalLabel = e.Item.FindControl("FulfillableTotalLabel") as Label;
                Label SubTotalLabel = e.Item.FindControl("SubTotalLabel") as Label;

                double dCost = Convert.ToDouble(RemoveExtraText(CostLabel.Text));
                double dInboundTotal = dCost * Convert.ToInt32(InboundLabel.Text);
                double dFulfillableTotal = dCost * Convert.ToInt32(FulfillableLabel.Text);
                double dSubTotal = dInboundTotal + dFulfillableTotal;

                dPageTotal += dSubTotal;
                dPageInboundTotal += dInboundTotal;
                dPageFulfillableTotal += dFulfillableTotal;

                InboundTotalLabel.Text = dInboundTotal.ToString("C");
                FulfillableTotalLabel.Text = dFulfillableTotal.ToString("C");
                SubTotalLabel.Text = dSubTotal.ToString("C");
            }
        }

        protected void ListView1_PreRender(object sender, EventArgs e)
        {
            Label totalInboundLbl = (Label)this.ListView1.FindControl("TotalInboundLabel");
            Label totalFulfillableLbl = (Label)this.ListView1.FindControl("TotalFulfillableLabel");
            Label totalCountLbl = (Label)this.ListView1.FindControl("TotalCountLabel");

            totalInboundLbl.Text = dPageInboundTotal.ToString("C");
            totalFulfillableLbl.Text = dPageFulfillableTotal.ToString("C");
            totalCountLbl.Text = dPageTotal.ToString("C");
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
        }
    }
}