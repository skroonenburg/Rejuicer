using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;

namespace Rejuicer.Engine
{
    public class CacheProvider : ICacheProvider
    {
        public void Add(string key, object value, IEnumerable<System.IO.FileInfo> dependentFiles)
        {
            var dependencies = dependentFiles != null ? new CacheDependency(dependentFiles.Select(f => f.FullName).ToArray()) : null;

            HttpContext.Current.Cache.Add(key, value, dependencies,
                                            System.Web.Caching.Cache.NoAbsoluteExpiration,
                                            System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal,
                                            null);
        }

        public T Get<T>(string key)
        {
            return (T) HttpContext.Current.Cache.Get(key);
        }
    }
}
