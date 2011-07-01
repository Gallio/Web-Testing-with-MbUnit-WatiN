using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SampleWebApplication
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ButtonOkSync_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2000); // Simulate long process.
            LabelResultSync.Text = String.Format("Hello from sync postback, {0}.", TextBoxName.Text);
        }

        protected void ButtonOkAsync_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2000); // Simulate long process.
            LabelResultAsync.Text = String.Format("Hello from async postback, {0}.", TextBoxName.Text);
        }
    }
}
