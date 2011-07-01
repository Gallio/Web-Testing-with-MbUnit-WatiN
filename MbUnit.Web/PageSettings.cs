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
        private readonly Lazy<UrlAttribute> lazyUrlAttribute;

        public string Url
        {
            get { return lazyUrlAttribute.Value.Url; }
        }

        public bool OnLocalHost
        {
            get { return lazyUrlAttribute.Value.OnLocalHost; }
        }

        public PageSettings()
        {
            lazyUrlAttribute = new Lazy<UrlAttribute>(GetUrlAttribute);
        }

        private static UrlAttribute GetUrlAttribute()
        {
            return GetAttributeInstance<UrlAttribute>(true);
        }

        private static T GetAttributeInstance<T>(bool throwIfMissing)
            where T : Attribute
        {
            object[] array = typeof(TPage).GetCustomAttributes(typeof(T), true);

            if (array.Length == 0)
            {
                if (throwIfMissing)
                    throw new InvalidOperationException(String.Format("The page object should be decorated with the attribute {0}.", typeof(T).FullName));

                return default(T);
            }

            return (T)array[0];
        }
    }
}
