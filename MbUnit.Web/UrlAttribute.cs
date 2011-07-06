using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MbUnit.Web
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UrlAttribute : Attribute
    {
        private readonly string url;
        private readonly bool onLocalHost;

        public string Url
        {
            get { return url; }
        }

        public bool OnLocalHost
        {
            get { return onLocalHost; }
        }

        public UrlAttribute(string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            this.url = url;
            onLocalHost = url.StartsWith("/");
        }
    }
}
