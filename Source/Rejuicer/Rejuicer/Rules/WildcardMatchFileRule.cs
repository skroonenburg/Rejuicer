using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rejuicer.Engine;

namespace Rejuicer.Model
{
    // A rule for including multiple files with a wildcard match. Immutable.
    internal sealed class WildcardMatchFileRule : IContentSourceRule
    {
        public static IVirtualPathResolver VirtualPathResolver { get; internal set; }

        static WildcardMatchFileRule()
        {
            VirtualPathResolver = new VirtualPathResolver();
        }

        public WildcardMatchFileRule(string folderVirtualPath, string searchPattern, bool recursive, Mode mode, IVirtualPathResolver virtualPathResolver)
        {
            FolderVirtualPath = folderVirtualPath;
            SearchPattern = searchPattern;
            IsRecursive = recursive;

            if (virtualPathResolver != null)
            {
                VirtualPathResolver = virtualPathResolver;
            }

            Mode = mode;
        }

        public WildcardMatchFileRule(string folderVirtualPath, string searchPattern, bool recursive, Mode mode)
            : this(folderVirtualPath, searchPattern, recursive, mode, null) 
        {
        }

        internal string FolderVirtualPath { get; private set; }
        internal string SearchPattern { get; private set; }
        internal bool IsRecursive{ get; private set; }
        internal Mode Mode { get; private set; }

        public IOrderedEnumerable<IContentSource> Evaluate(ResourceType resourceType)
        {
            var physicalPath = VirtualPathResolver.ResolveVirtualPathToDirectory(FolderVirtualPath);

            if (physicalPath == null)
                return Enumerable.Empty<IContentSource>().OrderBy(x => x);

            return Directory.EnumerateFiles(physicalPath.FullName, SearchPattern ?? "*", IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                        .Select(f => new FileInfo(f))
                        .Select(file => PhysicalFileRegister.For(file, resourceType, Mode))
                        .OfType<IContentSource>()
                        .OrderBy(x => new FileInfo(((PhysicalFileSource)x).PhysicalPath).Name);
        }
    }
}
