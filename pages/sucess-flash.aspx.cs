using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMS.pages
{
    public partial class sucess_flash : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void contbtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("load.aspx?target=log-in.aspx");
        }
    }
}