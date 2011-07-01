﻿using System;
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
    public class DevelopmentServer<TScope>
    {
        private static readonly Lazy<DevelopmentServer<TScope>> LazyInstance = new Lazy<DevelopmentServer<TScope>>(CreateNewInstance);

        private static readonly Lazy<Settings> LazySettings = new Lazy<Settings>(() =>
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = typeof(TScope).Assembly.Location + ".config" };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            ConfigurationSection section = config.GetSection("developmentServer");

            if (section == null)
                return Settings.Default;

            string xml = section.SectionInformation.GetRawXml();
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return (Settings)new ConfigurationSectionHandler().Create(null, null, doc.SelectSingleNode("/developmentServer"));
        });

        public string LocalHostUrl
        {
            get { return "http://localhost:" + LazySettings.Value.PortNumber; }
        }

        public string RootUrl
        {
            get { return LocalHostUrl + LazySettings.Value.VirtualPath; }
        }

        public static DevelopmentServer<TScope> Instance
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

        protected DevelopmentServer()
        {
        }

        private static DevelopmentServer<TScope> CreateNewInstance()
        {
            KillExistingProcesses();
            var server = new DevelopmentServer<TScope>();
            StartProcess();
            return server;
        }

        private static void KillExistingProcesses()
        {
            foreach (var process in Process.GetProcessesByName("WebDev.WebServer40"))
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
            process.StartInfo.FileName = WebServerPaths.First(x => File.Exists(x.Value)).Value;
            process.StartInfo.Arguments = String.Format("/port:{0} /path:\"{1}\" /virtual:\"{2}\"", LazySettings.Value.PortNumber, PhysicalPath, LazySettings.Value.VirtualPath);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
        }

        private static readonly Lazy<string>[] WebServerPaths = new[]
        {
            new Lazy<string>(() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), @"Microsoft Shared\DevServer\10.0\WebDev.WebServer40.exe")),                                
            new Lazy<string>(() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), @"Microsoft Shared\DevServer\10.0\WebDev.WebServer40.exe")),                                
        };
    }
}
