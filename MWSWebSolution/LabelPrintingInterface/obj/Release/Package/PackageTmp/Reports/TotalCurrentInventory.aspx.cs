using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LabelPrintingInterface.Reports
{
    public partial class TotalCurrentInventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           Label totalCountLbl =(Label) this.ListView1.FindControl("TotalCountLabel");
           totalCountLbl.Text = "1";
        }

        protected void SearchImageButton1_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected int GetSubTotal(object oFulfillable, object oInbound)
        {
            return (Int32)oFulfillable + (Int32)oInbound;
        }
    }
}