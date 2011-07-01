using System;
using Gallio.Framework;

namespace MbUnit.Web
{
    public interface IBrowserConfiguration
    {
        BrowserType BrowserType { get; }
        TriggerEvent BrowserSnapshotTriggerEvent { get; }
        double BrowserSnapshotZoom { get; }
        TriggerEvent ScreenRecordingTriggerEvent { get; }
        double ScreenRecordingZoom { get; }
        double ScreenRecordingFramesPerSecond { get; }
        string Label { get; }
        BrowserContext CreateBrowserContext();
    }
}
