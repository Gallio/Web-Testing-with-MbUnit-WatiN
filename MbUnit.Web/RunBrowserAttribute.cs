using System;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using WatiN.Core;

namespace MbUnit.Web
{
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple = true, Inherited = true)]
    public class RunBrowserAttribute : TestMethodDecoratorPatternAttribute, IBrowserConfiguration
    {
        private const string BrowserDataParameterName = "BrowserData";
        private static readonly Key<BrowserData> BrowserDataKey = new Key<BrowserData>("BrowserData");
        private readonly BrowserType browserType;

        public RunBrowserAttribute(BrowserType browserType = BrowserType.IE)
        {
            this.browserType = browserType;
            BrowserSnapshotTriggerEvent = TriggerEvent.Never;
            BrowserSnapshotZoom = 1.0;
            ScreenRecordingTriggerEvent = TriggerEvent.TestFailedOrInconclusive;
            ScreenRecordingZoom = 0.25;
            ScreenRecordingFramesPerSecond = 5;
        }

        public BrowserType BrowserType
        {
            get { return browserType; }
        }

        public TriggerEvent BrowserSnapshotTriggerEvent { get; set; }
        public double BrowserSnapshotZoom { get; set; }
        public TriggerEvent ScreenRecordingTriggerEvent { get; set; }
        public double ScreenRecordingZoom { get; set; }
        public double ScreenRecordingFramesPerSecond { get; set; }

        public virtual string Label
        {
            get { return browserType.ToString(); }
        }

        public virtual BrowserContext CreateBrowserContext()
        {
            return new BrowserContext(this);
        }

        protected override void DecorateMethodTest(IPatternScope methodScope, IMethodInfo method)
        {
            ITestParameterBuilder parameterBuilder = GetOrCreateBrowserDataTestParameter(methodScope);
            var browserData = new BrowserData { BrowserConfiguration = this };
            EnlistBrowserData(parameterBuilder, browserData);
        }

        private static ITestParameterBuilder GetOrCreateBrowserDataTestParameter(IPatternScope methodScope)
        {
            ITestParameterBuilder parameterBuilder = methodScope.TestBuilder.GetParameter(BrowserDataParameterName);

            if (parameterBuilder == null)
            {
                // Add a new test parameter for some BrowserData.  This makes the test data-driven.
                var parameterDataContextBuilder = methodScope.TestDataContextBuilder.CreateChild();
                parameterBuilder = methodScope.TestBuilder.CreateParameter(BrowserDataParameterName, null, parameterDataContextBuilder);

                // When the BrowserData is bound to the parameter before the initialization phase
                // of the test, add it to the test instance state so we can access it later.
                parameterBuilder.TestParameterActions.BindTestParameterChain.After((state, obj) =>
                {
                    var browserData = (BrowserData)obj;
                    state.Data.SetValue(BrowserDataKey, browserData);

                    state.AddNameSuffix(browserData.BrowserConfiguration.Label);
                });

                // When the test is initialized (before other set up actions occur), set up the browser context.
                methodScope.TestBuilder.TestInstanceActions.InitializeTestInstanceChain.After(state =>
                {
                    BrowserData browserData = state.Data.GetValue(BrowserDataKey);
                    var browserContext = new BrowserContext(browserData.BrowserConfiguration);
                    BrowserContext.SetBrowserContext(TestContext.CurrentContext, browserContext);

                    browserContext.SetUp();
                });

                // When the test is disposed (after other tear down actions occur), tear down the browser context.
                methodScope.TestBuilder.TestInstanceActions.DisposeTestInstanceChain.Before(state =>
                {
                    if (BrowserContext.HasCurrentBrowserContext)
                    {
                        try
                        {
                            BrowserContext.CurrentBrowserContext.TearDown();
                        }
                        finally
                        {
                            BrowserContext.SetBrowserContext(TestContext.CurrentContext, null);
                        }
                    }
                });
            }

            return parameterBuilder;
        }

        private static void EnlistBrowserData(ITestParameterBuilder parameterBuilder, BrowserData browserData)
        {
            DataSource dataSource = parameterBuilder.TestDataContextBuilder.DefineDataSource("");
            dataSource.AddDataSet(new ValueSequenceDataSet(new[] { browserData }, null, false));
        }

        private sealed class BrowserData
        {
            public IBrowserConfiguration BrowserConfiguration { get; set; }
        }
    }
}
