using System;
using Gallio.Common.Collections;
using Gallio.Common.Media;
using Gallio.Framework;
using WatiN.Core;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.UtilityClasses;

namespace MbUnit.Web
{
    public class BrowserContext
    {
        private static readonly Key<BrowserContext> BrowserContextKey = new Key<BrowserContext>("BrowserContext");
        private readonly IBrowserConfiguration browserConfiguration;
        private Browser browser;
        private bool isBrowserAvailable;
        private ScreenRecorder screenRecorder;

        public BrowserContext(IBrowserConfiguration browserConfiguration)
        {
            if (browserConfiguration == null)
                throw new ArgumentNullException("browserConfiguration");

            this.browserConfiguration = browserConfiguration;
        }

        public static BrowserContext GetBrowserContext(TestContext testContext)
        {
            if (testContext == null)
                throw new ArgumentNullException("testContext");

            return testContext.Data.GetValueOrDefault(BrowserContextKey, null);
        }

        public static void SetBrowserContext(TestContext testContext, BrowserContext browserContext)
        {
            if (testContext == null)
                throw new ArgumentNullException("testContext");

            if (browserContext == null)
                testContext.Data.RemoveValue(BrowserContextKey);
            else
                testContext.Data.SetValue(BrowserContextKey, browserContext);
        }

        public static bool HasCurrentBrowserContext
        {
            get { return GetBrowserContext(TestContext.CurrentContext) != null; }
        }

        public static BrowserContext CurrentBrowserContext
        {
            get
            {
                BrowserContext browserContext = GetBrowserContext(TestContext.CurrentContext);

                if (browserContext == null)
                    throw new InvalidOperationException("There is no current browser context. Does the test have a [RunBrowser] attribute?");

                return browserContext;
            }
        }

        public bool IsBrowserAvailable
        {
            get { return isBrowserAvailable; }
        }

        public bool IsBrowserOpen
        {
            get { return isBrowserAvailable && browser != null; }
        }

        public Browser Browser
        {
            get
            {
                if (!isBrowserAvailable)
                    throw new InvalidOperationException("The browser is not available. The test might not have been set up or has already been torn down.");

                return browser ?? (browser = CreateBrowser());
            }
        }

        public IBrowserConfiguration BrowserConfiguration
        {
            get { return browserConfiguration; }
        }

        public virtual Browser CreateBrowser()
        {
            switch (browserConfiguration.BrowserType)
            {
                case BrowserType.IE:
                    var ie = new IE();
                    ie.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.Maximize);
                    return ie;

                case BrowserType.FireFox:
                    var fireFox = new FireFox();
                    return fireFox;

                case BrowserType.Chrome:
                    throw new NotSupportedException("Chrome browser not fully supported by WatiN at this time!");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void SetUp()
        {
            isBrowserAvailable = true;
            StartScreenRecordingIfNeeded();
            ConfigureWatiNSettings();
        }

        public virtual void TearDown()
        {
            try
            {
                if (browser != null)
                {
                    StopScreenRecordingAndEmbedIfNeeded();
                    EmbedFinalBrowserSnapshotIfNeeded();

                    try
                    {
                        WatiN.Core.Settings.WaitForCompleteTimeOut = 1;
                        browser.Close();
                    }
                    catch
                    {
                        // Ignore problems closing the browser.  It's possible that the user forcibly
                        // closed the browser before the test was finished in which case attempting
                        // to close it again could yield an error.
                    }

                    browser = null;
                }
            }
            finally
            {
                isBrowserAvailable = false;
            }
        }

        public virtual void ConfigureCaptureSettings()
        {
            Capture.SetCaptionFontSize(24);
            Capture.SetCaptionAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }

        public virtual void ConfigureWatiNSettings()
        {
            if (!(Logger.LogWriter is GallioLogger))
                Logger.LogWriter = new GallioLogger();

            WatiN.Core.Settings.AutoMoveMousePointerToTopLeft = false;
            WatiN.Core.Settings.MakeNewIeInstanceVisible = true;
        }

        public void EmbedBrowserSnapshot(string attachmentName)
        {
            if (IsBrowserOpen)
                EmbedBrowserSnapshot(attachmentName, browser);
        }

        public void EmbedBrowserSnapshot(string attachmentName, Browser browser)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            try
            {
                var capture = new CaptureWebPage(browser);
                System.Drawing.Image image = capture.CaptureWebPageImage(true, false, 100);

                using (TestLog.BeginSection(browser.Url))
                    TestLog.EmbedImage(attachmentName, image);
            }
            catch
            {
                // Ignore the failure since the snapshot is for diagnostic purposes only.
                // If we can't capture it then too bad.
            }
        }

        private void EmbedFinalBrowserSnapshotIfNeeded()
        {
            if (ShouldEmbedFinalBrowserSnapshotGivenCurrentTestContext())
            {
                EmbedBrowserSnapshot("Final browser contents.");
            }
        }

        private void StartScreenRecordingIfNeeded()
        {
            if (ShouldStartScreenRecordingGivenCurrentTestContext())
            {
                screenRecorder = Capture.StartRecording(new CaptureParameters()
                {
                    Zoom = browserConfiguration.ScreenRecordingZoom
                }, browserConfiguration.ScreenRecordingFramesPerSecond);
            }
        }

        private void StopScreenRecordingAndEmbedIfNeeded()
        {
            if (screenRecorder != null)
            {
                screenRecorder.Stop();
                Video video = screenRecorder.Video;
                screenRecorder = null;

                if (ShouldEmbedScreenRecordingGivenCurrentTestContext())
                {
                    TestLog.EmbedVideo("Screen Recording", video);
                }
            }
        }

        private bool ShouldStartScreenRecordingGivenCurrentTestContext()
        {
            return browserConfiguration.ScreenRecordingTriggerEvent != TriggerEvent.Never;
        }

        private bool ShouldEmbedScreenRecordingGivenCurrentTestContext()
        {
            return TestContext.CurrentContext.IsTriggerEventSatisfied(browserConfiguration.ScreenRecordingTriggerEvent);
        }

        private bool ShouldEmbedFinalBrowserSnapshotGivenCurrentTestContext()
        {
            return TestContext.CurrentContext.IsTriggerEventSatisfied(browserConfiguration.BrowserSnapshotTriggerEvent);
        }

        private sealed class GallioLogger : ILogWriter
        {
            public void LogAction(string message)
            {
                TestLog.WriteLine(message);
                Capture.SetCaption(message);
            }

            public void LogDebug(string message)
            {
                TestLog.WriteLine(message);
            }

            public bool HandlesLogAction
            {
                get { return true; }
            }

            public bool HandlesLogDebug
            {
                get { return true; }
            }
        }
    }
}
