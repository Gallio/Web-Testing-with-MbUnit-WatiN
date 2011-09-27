using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace MbUnit.Web
{
    public class WebTestServer<TScope>
    {
        private static readonly Lazy<WebTestServer<TScope>> LazyInstance = new Lazy<WebTestServer<TScope>>(CreateNewInstance);

        private static readonly Lazy<Settings> LazySettings = new Lazy<Settings>(() =>
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = typeof(TScope).Assembly.Location + ".config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            ConfigurationSection section = config.GetSection("webTestServer");

            if (section == null)
                return Settings.Default;

            string xml = section.SectionInformation.GetRawXml();
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return (Settings)new ConfigurationSectionHandler().Create(null, null, doc.SelectSingleNode("/webTestServer"));
        });

        public string LocalHostUrl
        {
            get { return "http://localhost:" + LazySettings.Value.PortNumber; }
        }

        public string RootUrl
        {
            get { return LocalHostUrl + LazySettings.Value.VirtualPath; }
        }

        public static WebTestServer<TScope> Instance
        {
            get { return LazyInstance.Value; }
        }

        private static string PhysicalPath
        {
            get 
            { 
                return Path.IsPathRooted(LazySettings.Value.SitePath) 
                    ? LazySettings.Value.SitePath 
                    : Path.GetFullPath(Directory.GetCurrentDirectory() + LazySettings.Value.SitePath); 
            }
        }

        protected WebTestServer()
        {
        }

        private static WebTestServer<TScope> CreateNewInstance()
        {
            KillExistingProcesses();
            var server = new WebTestServer<TScope>();
            StartProcess();
            return server;
        }

        private static void KillExistingProcesses()
        {
            foreach (var process in Process.GetProcessesByName(LazySettings.Value.Server.ProcessName))
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit(10000);
                }
            }
        }

        private static void StartProcess()
        {
            var process = new Process();
            process.StartInfo.FileName = LazySettings.Value.Server.GetProcessFileName();
            process.StartInfo.Arguments = LazySettings.Value.Server.FormatArguments(LazySettings.Value.PortNumber, PhysicalPath, LazySettings.Value.VirtualPath);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
        }
    }
}
