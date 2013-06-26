using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Engine;

namespace Rejuicer.Model
{
    // A rule for including a single file. Immutable.
    internal sealed class SingleFileRule : IContentSourceRule
    {
        public static IVirtualPathResolver VirtualPathResolver { get; internal set; }

        static SingleFileRule()
        {
            VirtualPathResolver = new VirtualPathResolver();
        }

        public SingleFileRule(string virtualPath, Mode mode)
            : this(virtualPath, mode, null)
        {}

        public SingleFileRule(string virtualPath, Mode mode, IVirtualPathResolver virtualPathResolver)
        {
            VirtualPath = virtualPath;
            if (virtualPathResolver != null)
            {
                VirtualPathResolver = virtualPathResolver;
            }
            Mode = mode;
        }

        internal string VirtualPath { get; private set; }
        internal Mode Mode { get; private set; }

        public IOrderedEnumerable<IContentSource> Evaluate(ResourceType resourceType)
        {
            var file = VirtualPathResolver.ResolveVirtualPathToFile(VirtualPath);

            if (file == null)
            {
                return Enumerable.Empty<IContentSource>().OrderBy(x => x);
            }

            return new IContentSource[] { PhysicalFileRegister.For(VirtualPath, resourceType, Mode) }.OrderBy(x => x);
        }
    }
}
