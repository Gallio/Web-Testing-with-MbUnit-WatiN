using System;
using System.Collections.Generic;
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
        public readonly static Settings Default = new Settings(DefaultSitePath, DefaultVirtualPath, DefaultPortNumber);
        private readonly string sitePath;
        private readonly string virtualPath;
        private readonly string portNumber;

        public Settings(string sitePath, string virtualPath, string portNumber)
        {
            this.sitePath = sitePath ?? DefaultSitePath;
            this.virtualPath = virtualPath ?? DefaultVirtualPath;
            this.portNumber = portNumber ?? DefaultPortNumber;
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
    }
}
