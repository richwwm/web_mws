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
    public partial class NetProfitSummary : System.Web.UI.Page
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
        
        protected void ListView1_ItemCommand1(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort") // Process header sort command
            {
                string sExpression = e.CommandArgument.ToString();
                Session["SortedBy"] = sExpression;
                this.ListView1.DataSourceID = null;
                this.NetProfitDataSource.Select();
                this.ListView1.DataSource = this.NetProfitDataSource;
                this.ListView1.DataBind();
            }
            else
                Session["SortedBy"] = "";
        }

        protected void ListViewLinkButton_Command(object sender, CommandEventArgs  e)
        {
            if (e.CommandArgument != null)
            {
                string sCommandArgument = e.CommandArgument.ToString();
                string[] argumentArray = sCommandArgument.Split(',');
                Session["Selected_Year"] = this.DropDownList2.Text;

                Response.Redirect("~/YearlyNetProfit.aspx");
            }

        }
        
        protected void NetProfitDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!this.Page.User.Identity.IsAuthenticated)
            {
                e.Cancel = true;
            }
        }


        protected void DropDownList2_TextChanged(object sender, EventArgs e)
        {
            this.ListView1.DataSourceID = "";
            this.ListView1.DataSource = null;
            this.NetProfitDataSource.Select();
            this.ListView1.DataSource = this.NetProfitDataSource;
            this.ListView1.DataBind();
        }
    }
}