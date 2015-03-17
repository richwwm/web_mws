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

        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.Page.User.Identity.IsAuthenticated)
            {
                string strLoginID = this.Page.User.Identity.Name;
                if (strLoginID != null)
                {
                    dbHandler = new SqlDataHandler(constr);
                    sMerchantID = dbHandler.GetMerchantIDByUserID(strLoginID);
                    Session["MerchantID"] = sMerchantID;

                    DataSet dsPermittedPage = dbHandler.GetUserIDPagePermission(strLoginID);
                    
                    
                    //foreach (DataRow parentItem in dsPermittedPage.Tables["Categories"].Rows)
                    //{
                    //    MenuItem categoryItem = new MenuItem((string)parentItem["CategoryName"]);
                    //    menu.Items.Add("text");

                    //    foreach (DataRow childItem in parentItem.GetChildRows("Children"))
                    //    {
                    //        MenuItem childrenItem = new MenuItem((string)childItem["ProductName"]);
                    //        categoryItem.ChildItems.Add(childrenItem);
                    //    }
                    //}

                    List<MenuItem> disabledMenuItmeList = new List<MenuItem>();
                    foreach (MenuItem item in this.Menu1.Items)
                    {
                        item.Enabled = false;
                        if (item.Text == "Logoff")
                            item.Enabled = true;

                        if (item.Text == "Sales")
                        {
                            item.Enabled = true;

                        }

                        foreach (MenuItem childItem in item.ChildItems)
                        {
                            childItem.Enabled = false;

                            if (childItem.Text == "NetProfitSim")
                                childItem.Enabled = true;
                        }

                    }

                    if (dsPermittedPage.Tables.Count != 0)
                    {
                        foreach (DataRow row in dsPermittedPage.Tables[0].Rows)
                        {
                            string sPath = row["PagePath"].ToString();
                            foreach (MenuItem item in this.Menu1.Items)
                            {
                                if (item.NavigateUrl == sPath)
                                {
                                    item.Enabled = true;
                                    continue;
                                }
                                foreach (MenuItem childItem in item.ChildItems)
                                {
                                    if (sPath == childItem.NavigateUrl)
                                    {
                                        item.Enabled = true;
                                        childItem.Enabled = true;
                                        continue;
                                    }

                                }
                            }
                        }

                    }


                }
            }
            else
            {
                string currentUrl = HttpContext.Current.Request.Url.ToString();
                if (!currentUrl.Contains("Login"))
                    FormsAuthentication.RedirectToLoginPage();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }




        protected void Menu1_MenuItemClick(object sender, MenuEventArgs e)
        {
            if (e.Item.Text == "Logoff")
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