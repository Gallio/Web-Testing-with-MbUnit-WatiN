using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace MbUnit.Web
{
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return new Settings(
                GetNodeValueOfNull(section, "sitePath"),
                GetNodeValueOfNull(section, "virtualPath"),
                GetNodeValueOfNull(section, "portNumber"));
        }

        private static string GetNodeValueOfNull(XmlNode section, string name)
        {
            XmlNode node = section.SelectSingleNode(name);
            return (node == null) ? null : node.FirstChild.Value;
        }
    }
}
