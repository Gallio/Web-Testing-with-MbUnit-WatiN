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
    public class GoogleSearchTest : BrowserTestFixture<GoogleSearchTest.Page>
    {
        [Test, RunBrowser]
        public void Search_for_MbUnit_on_Google()
        {
            var page = GoToPage();
            page.QueryTextField.TypeText("MbUnit");
            page.SearchButton.Click();
            Assert.Contains(Browser.Text, "Generative Unit Test Framework for the .NET Framework.");
        }

        [Url("http://www.google.com", OnLocalHost = false)]
        public class Page : WatiN.Core.Page
        {
            [FindBy(Name = "q")]
            public TextField QueryTextField;

            [FindBy(Name = "btnG")]
            public Button SearchButton;
        }
    }
}
