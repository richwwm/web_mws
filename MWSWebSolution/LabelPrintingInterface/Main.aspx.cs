using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using MWS.Lib;

namespace LabelPrintingInterface
{
    public partial class Main : System.Web.UI.Page
    {
        string constr = "Data Source=192.168.103.150\\INFLOWSQL;Initial Catalog=MWS;User ID=mws;Password=p@ssw0rd";
        SqlDataHandler dbHandler;
        string sMerchantID;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                Session["MerchantID"] = "";
            }
            else
            {
                string strLoginID = User.Identity.Name;
                if (strLoginID != null)
                {
                    dbHandler = new SqlDataHandler(constr);
                    sMerchantID = dbHandler.GetMerchantIDByUserID(strLoginID);
                    Session["MerchantID"] = sMerchantID;
                }
            }
        }
    }
}