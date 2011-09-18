using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rejuicer.Model
{
    // A rule for including multiple files with a wildcard match. Immutable.
    internal sealed class WildcardMatchFileRule : IContentSourceRule
    {
        public static IVirtualPathResolver VirtualPathResolver { get; private set; }

        public WildcardMatchFileRule(string folderVirtualPath, string searchPattern, bool recursive, Mode mode, IVirtualPathResolver virtualPathResolver)
        {
            FolderVirtualPath = folderVirtualPath;
            SearchPattern = searchPattern;
            IsRecursive = recursive;
            VirtualPathResolver = virtualPathResolver;
            Mode = mode;
        }

        public WildcardMatchFileRule(string folderVirtualPath, string searchPattern, bool recursive, Mode mode)
            : this(folderVirtualPath, searchPattern, recursive, mode, new VirtualPathResolver()) 
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
                        .Select(file => new PhysicalFileSource(resourceType, Mode, VirtualPathResolver.GetVirtualPathFor(file), file.FullName))
                        .OfType<IContentSource>()
                        .OrderBy(x => new FileInfo(((PhysicalFileSource)x).PhysicalPath).Name);
        }
    }
}
