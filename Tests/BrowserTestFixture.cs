using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Web;
using WatiN.Core;

namespace Tests
{
    public abstract class BrowserTestFixture<TPage> : AbstractBrowserTestFixture<TPage>
        where TPage : Page, new()
    {
        protected BrowserTestFixture()
            : base(() => DevelopmentServer<ScopeMarker>.Instance.LocalHostUrl)
        {
        }
    }

    // Empty type to let the development server controller to know in which assembly to find the settings.
    class ScopeMarker
    {
    }
}
