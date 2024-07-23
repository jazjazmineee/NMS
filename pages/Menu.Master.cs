using System;
using System.Web;
using System.Web.UI;

namespace NMS
{
    public partial class Menu : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            // Clear and abandon the session
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Cookies.Add(new HttpCookie("Username", ""));

            // Redirect to the login page
            Response.Redirect("log-in.aspx");
        }
    }
}
