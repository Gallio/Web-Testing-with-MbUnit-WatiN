using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using MbUnit.Web;
using WatiN.Core;

namespace Tests
{
    [TestFixture]
    public class DefaultPageTest : BrowserTestFixture<DefaultPageTest.Page>
    {
        [Test, RunBrowser]
        public void Sync_postback()
        {
            var page = GoToPage();
            page.TextBoxName.TypeText("McFly");
            page.ButtonOkSync.Click();
            Assert.AreEqual(page.SpanResultSync.Text, "Hello from sync postback, McFly.");
            Assert.IsNull(page.SpanResultAsync.Text);
            Assert.IsNull(page.SpanResultAsyncJQuery.Text);
        }

        [Test, RunBrowser]
        public void Async_postback_with_UpdatePanel()
        {
            var page = GoToPage();
            page.TextBoxName.TypeText("McFly");
            page.ButtonOkAsync.Click();
            WaitForAsyncPostBackComplete(); // WatiN does not handle with async postback requests automatically.
            Assert.AreEqual(page.SpanResultAsync.Text, "Hello from async postback, McFly.");
            Assert.IsNull(page.SpanResultSync.Text);
            Assert.IsNull(page.SpanResultAsyncJQuery.Text);
        }

        [Test, RunBrowser]
        public void Async_postback_with_jQuery_Ajax()
        {
            var page = GoToPage();
            StartJQueryAjaxMonitoring(); // Attaches a monitor to count async jQuery/AJAX requests in progress.
            page.TextBoxName.TypeText("McFly");
            page.ButtonOkAsyncJQuery.Click();
            WaitForJQueryAjaxComplete(); // Wait for all jQuery/AJAX requests to complete.
            Assert.AreEqual(page.SpanResultAsyncJQuery.Text, "Hello from async postback, McFly.");
            Assert.IsNull(page.SpanResultSync.Text);
            Assert.IsNull(page.SpanResultAsync.Text);
        }

        [Page, Url("/Default.aspx")]
        public class Page : WatiN.Core.Page
        {
            [FindBy(Id = "MainContent_TextBoxName")]
            public TextField TextBoxName;

            [FindBy(Id = "MainContent_ButtonOkSync")]
            public Button ButtonOkSync;

            [FindBy(Id = "MainContent_LabelResultSync")]
            public Span SpanResultSync;

            [FindBy(Id = "MainContent_ButtonOkAsync")]
            public Button ButtonOkAsync;

            [FindBy(Id = "MainContent_LabelResultAsync")]
            public Span SpanResultAsync;

            [FindBy(Id = "ButtonOkAsyncJQuery")]
            public Button ButtonOkAsyncJQuery;

            [FindBy(Id = "LabelResultAsyncJQuery")]
            public Span SpanResultAsyncJQuery;
        }
    }
}
