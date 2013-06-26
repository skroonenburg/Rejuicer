using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer.Engine
{
    public static class PhysicalFileRegister
    {
        private static readonly IDictionary<string, PhysicalFileSource> fileSourceRegister = new Dictionary<string, PhysicalFileSource>();
        internal static IVirtualPathResolver VirtualPathResolver { get; set; }

        static PhysicalFileRegister()
        {
            VirtualPathResolver = new VirtualPathResolver();
        }

        public static void Set(PhysicalFileSource fileSource)
        {
            fileSourceRegister[fileSource.VirtualPath] = fileSource;
        }

        public static PhysicalFileSource For(string virtualPath)
        {
            return fileSourceRegister.ContainsKey(virtualPath) ? fileSourceRegister[virtualPath] : null;
        }

        public static PhysicalFileSource For(string virtualPath, ResourceType resourceType, Mode mode)
        {
            var file = new FileInfo(VirtualPathResolver.ResolveVirtualPath(virtualPath));

            var physicalFileSource = For(virtualPath);

            if (physicalFileSource == null)
            {
                physicalFileSource = new PhysicalFileSource(resourceType,
                                                            virtualPath,
                                                            file.FullName,
                                                            mode);

                Set(physicalFileSource);
            }

            return physicalFileSource;
        }
    }
}
