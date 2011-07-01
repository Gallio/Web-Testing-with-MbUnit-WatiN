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
    public class BingSearchTest : BrowserTestFixture<BingSearchTest.Page>
    {
        [Test, RunBrowser]
        public void Search_for_Gallio_on_Bing()
        {
            var page = GoToPage();
            page.SearchBox.TypeText("Gallio");
            page.ButtonGo.Click();
            Assert.Contains(Browser.Text, "Automation Platform for .NET");
        }

        [Page, Url("http://www.bing.com", OnLocalHost = false)]
        public class Page : WatiN.Core.Page
        {
            [FindBy(Name = "q")]
            public TextField SearchBox;

            [FindBy(Name = "go")]
            public Button ButtonGo;
        }
    }
}
