using System;
using System.Net;
using System.Web;
using System.Web.UI;

namespace NMS
{
    public class BasePage : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Check for internet connectivity
            if (!IsConnectedToInternet())
            {
                Response.Redirect("no-connection.aspx");
                return;
            }

            // Prevent browser caching
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.MinValue);
        }

        private bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}