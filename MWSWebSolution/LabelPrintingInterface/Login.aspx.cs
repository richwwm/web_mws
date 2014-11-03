using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

namespace LabelPrintingInterface
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Menu masteMenu = Master.FindControl("Menu1") as Menu;
            masteMenu.Visible = false;
            Session["MerchantID"] = "";
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {

        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            ValidateUser(sender, e);
        }

        protected void ValidateUser(object sender, EventArgs e)
        {
            int userId = 0;
            string constr = "Data Source=192.168.103.150\\INFLOWSQL;Initial Catalog=MWS;User ID=mws;Password=p@ssw0rd";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("Validate_User"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", Login1.UserName);
                    cmd.Parameters.AddWithValue("@Password", Login1.Password);
                    cmd.Connection = con;
                    con.Open();
                    userId = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                switch (userId)
                {
                    case -1:
                        Login1.FailureText = "Username and/or password is incorrect.";
                        break;
                    case -2:
                        Login1.FailureText = "Account has not been activated.";
                        break;
                    default:
                        string userData = userId.ToString();

                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                          userId.ToString(),
                          DateTime.Now,
                          DateTime.Now.AddMinutes(30),
                          Login1.RememberMeSet,
                          userData,
                          FormsAuthentication.FormsCookiePath);

                        // Encrypt the ticket.
                        string encTicket = FormsAuthentication.Encrypt(ticket);

                        // Create the cookie.
                        Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                        // Redirect back to original URL.
                        FormsAuthentication.RedirectFromLoginPage(userId.ToString(), Login1.RememberMeSet);
                        break;
                }
            }
        }
    }
}