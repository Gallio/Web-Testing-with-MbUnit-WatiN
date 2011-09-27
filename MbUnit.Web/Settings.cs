using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.ObjectModel;

namespace MbUnit.Web
{
    public sealed class Settings
    {
        private const string DefaultSitePath = "";
        private const string DefaultVirtualPath = "";
        private const string DefaultPortNumber = "1162";
        public readonly static Settings Default = new Settings("DevelopmentServer", DefaultSitePath, DefaultVirtualPath, DefaultPortNumber);
        private readonly IWebServerType server;
        private readonly string sitePath;
        private readonly string virtualPath;
        private readonly string portNumber;

        public Settings(string server, string sitePath, string virtualPath, string portNumber)
        {
            this.server = WebServerTypeFactory(server);
            this.sitePath = sitePath ?? DefaultSitePath;
            this.virtualPath = virtualPath ?? DefaultVirtualPath;
            this.portNumber = portNumber ?? DefaultPortNumber;
        }

        private IWebServerType WebServerTypeFactory(string server)
        {
            if (String.IsNullOrWhiteSpace(server))
                return new DevelopmentServer();

            if (server.Replace(" ", String.Empty).Equals("iisexpress", StringComparison.OrdinalIgnoreCase))
                return new IisExpress();

            return new DevelopmentServer();
        }

        public IWebServerType Server
        {
            get { return server; }
        }

        public string SitePath
        {
            get { return sitePath; }
        }

        public string VirtualPath
        {
            get { return virtualPath; }
        }

        public string PortNumber
        {
            get { return portNumber; }
        }

        public class DevelopmentServer : IWebServerType
        {
            private readonly Lazy<string> lazyProcessFileName = new Lazy<string>(() => GetPossiblePaths().First(File.Exists));

            public string ProcessName
            {
                get { return "WebDev.WebServer40"; }
            }

            public string FormatArguments(string portNumber, string physicalPath, string virtualPath)
            {
                return String.Format("/port:{0} /path:\"{1}\" /virtual:\"{2}\"", portNumber, physicalPath, virtualPath);
            }

            private static IEnumerable<string> GetPossiblePaths()
            {
                string common = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                const string inner = @"Microsoft Shared\DevServer\10.0\WebDev.WebServer40.exe";
                yield return Path.Combine(common, inner);
                string commonX86 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);
                yield return Path.Combine(commonX86, inner);
            }

            public string GetProcessFileName()
            {
                return lazyProcessFileName.Value;
            }
        }

        public class IisExpress : IWebServerType
        {
            private readonly Lazy<string> lazyProcessFileName = new Lazy<string>(() => GetPossiblePaths().First(File.Exists));

            public string ProcessName
            {
                get { return "iisexpress"; }
            }

            public string FormatArguments(string portNumber, string physicalPath, string virtualPath)
            {
                return String.Format("/port:{0} /path:\"{1}\"", portNumber, physicalPath);
            }

            private static IEnumerable<string> GetPossiblePaths()
            {
                string common = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                const string inner = @"IIS Express\iisexpress.exe";
                yield return Path.Combine(common, inner);
                string commonX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                yield return Path.Combine(commonX86, inner);
            }

            public string GetProcessFileName()
            {
                return lazyProcessFileName.Value;
            }
        }
    }

    public interface IWebServerType
    {
        string ProcessName { get; }
        string FormatArguments(string portNumber, string physicalPath, string virtualPath);
        string GetProcessFileName();
    }
}
