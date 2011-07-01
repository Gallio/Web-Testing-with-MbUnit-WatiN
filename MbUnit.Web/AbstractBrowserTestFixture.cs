using System;
using System.Text;
using System.Threading;
using WatiN.Core;

namespace MbUnit.Web
{
    public abstract class AbstractBrowserTestFixture<TPage>
        where TPage : Page, new()
    {
        private readonly string localHostUrl;
        private readonly PageSettings<TPage> pageSettings = new PageSettings<TPage>();

        protected AbstractBrowserTestFixture(string localHostUrl = null)
        {
            this.localHostUrl = localHostUrl ?? String.Empty;
        }

        public static Browser Browser
        {
            get { return BrowserContext.Browser; }
        }

        public static BrowserContext BrowserContext
        {
            get { return BrowserContext.CurrentBrowserContext; }
        }

        public void EmbedBrowserSnapshot(string attachmentName)
        {
            BrowserContext.EmbedBrowserSnapshot(attachmentName);
        }

        public void EmbedBrowserSnapshot(string attachmentName, Browser browser)
        {
            BrowserContext.EmbedBrowserSnapshot(attachmentName, browser);
        }

        protected TPage GoToPage(string extra = null)
        {
            WatiN.Core.Settings.Instance.Reset();
            Browser.GoTo(BuildPageUrl(extra));
            Browser.WaitForComplete();
            Pause();
            return Browser.Page<TPage>();
        }

        private string BuildPageUrl(string extra)
        {
            var builder = new StringBuilder();

            if (pageSettings.OnLocalHost)
                builder.Append(localHostUrl);

            builder.Append(pageSettings.Url);

            if (extra != null)
                builder.Append(extra);

            return builder.ToString();
        }

        protected void WaitForAsyncPostBackComplete()
        {
            switch (BrowserContext.BrowserConfiguration.BrowserType)
            {
                case BrowserType.IE:
                {
                    Wait(() => !Convert.ToBoolean(Browser.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
                    break;
                }
            }
        }

        protected void StartJQueryAjaxMonitoring()
        {
            Browser.Eval(
                "function AjaxMonitorForWatiN() {" +
                "  var count = 0;" +
                "  $(document).ajaxStart(function() { count++; });" +
                "  $(document).ajaxComplete(function() { count--; });" +
                "  this.asyncInProgress = function () { return (count > 0); }; }" +
                "var ajaxMonitorForWatiN = new AjaxMonitorForWatiN();");
        }

        protected void WaitForJQueryAjaxComplete()
        {
            Wait(() => !Convert.ToBoolean(Browser.Eval("ajaxMonitorForWatiN.asyncInProgress()")));
        }

        private static void Wait(Func<bool> condition)
        {
            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(20);

            while (!condition() && DateTime.UtcNow < timeout)
            {
                Pause();
            }

            Pause();
        }

        protected static void Pause(double seconds = 0.5)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }
    }
}
