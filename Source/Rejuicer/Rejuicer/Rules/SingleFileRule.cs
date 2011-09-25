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
        public static IVirtualPathResolver VirtualPathResolver { get; private set; }

        public SingleFileRule(string virtualPath, Mode mode)
            : this(virtualPath, mode, new VirtualPathResolver())
        {}

        public SingleFileRule(string virtualPath, Mode mode, IVirtualPathResolver virtualPathResolver)
        {
            VirtualPath = virtualPath;
            VirtualPathResolver = virtualPathResolver;
            Mode = mode;
        }

        internal string VirtualPath { get; private set; }
        internal Mode Mode { get; private set; }

        public IOrderedEnumerable<IContentSource> Evaluate(ResourceType resourceType)
        {
            return new IContentSource[] { PhysicalFileRegister.For(VirtualPathResolver.ResolveVirtualPathToFile(VirtualPath), resourceType, Mode) }.OrderBy(x => x);
        }
    }
}
