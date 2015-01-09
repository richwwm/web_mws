using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;
using MWS.Lib;

namespace LabelPrintingInterface
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        string constr = "Data Source=192.168.103.150\\INFLOWSQL;Initial Catalog=MWS;User ID=mws;Password=p@ssw0rd";
        SqlDataHandler dbHandler;
        string sMerchantID;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.User.Identity.IsAuthenticated)
            {
                string strLoginID = this.Page.User.Identity.Name;
                if (strLoginID != null)
                {
                    dbHandler = new SqlDataHandler(constr);
                    sMerchantID = dbHandler.GetMerchantIDByUserID(strLoginID);
                    Session["MerchantID"] = sMerchantID;
                }
            }
            else
            {
                string currentUrl = HttpContext.Current.Request.Url.ToString();
                if(!currentUrl.Contains("Login"))
                    FormsAuthentication.RedirectToLoginPage();
            }
        }

        protected void Menu1_MenuItemClick(object sender, MenuEventArgs e)
        {
            if (e.Item.Text == "Logout")
            {
                FormsAuthentication.SignOut();
                Response.Clear();
                Response.Redirect("~/login.aspx");
            }
        }

        public Menu PropertyMasterMenu
        {
            get { return Menu1; }
        }


    }
}