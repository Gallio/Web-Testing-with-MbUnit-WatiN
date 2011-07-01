using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SampleWebApplication
{
    public partial class GetResultByJQueryAjax : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.Sleep(2000); // Simulate long process.
            Response.Expires = -1;
            Response.ContentType = "text/plain";
            Response.Write(String.Format("Hello from async postback, {0}.", Request.QueryString["name"]));
            Response.End();
        }
    }
}