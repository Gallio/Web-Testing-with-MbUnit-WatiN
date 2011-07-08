using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace MbUnit.Web
{
    internal sealed class PageSettings<TPage>
        where TPage : Page
    {
        private readonly Lazy<UrlAttribute> lazyUrlAttribute = new Lazy<UrlAttribute>(GetAttributeInstance);

        public string Url
        {
            get { return lazyUrlAttribute.Value.Url; }
        }

        public bool OnLocalHost
        {
            get { return lazyUrlAttribute.Value.OnLocalHost; }
        }

        private static UrlAttribute GetAttributeInstance()
        {
            object[] array = typeof(TPage).GetCustomAttributes(typeof(UrlAttribute), true);
            return (array.Length == 0) ? new UrlAttribute(String.Empty) : (UrlAttribute)array[0];
        }
    }
}
