using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Rejuicer.Model;

namespace Rejuicer
{
    internal static class FileResolver
    {
        static FileResolver()
        {
            VirtualPathResolver = new VirtualPathResolver();
        }

        public static IVirtualPathResolver VirtualPathResolver { get; set; }
        
        public static IEnumerable<string> VirtualPathsFor(RejuicerConfigurationSource config)
        {
            return config == null ? Enumerable.Empty<string>() : config.GetFiles(config.ResourceType);
        }

        private class FileComparer : IEqualityComparer<FileInfo>
        {
            public bool Equals(FileInfo x, FileInfo y)
            {
                return x.FullName == y.FullName;
            }

            public int GetHashCode(FileInfo obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
