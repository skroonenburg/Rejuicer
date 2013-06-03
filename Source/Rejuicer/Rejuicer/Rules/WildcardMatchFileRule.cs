using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rejuicer.Engine;
using Rejuicer.Model;

namespace Rejuicer.Rules
{
    // A rule for including multiple files with a wildcard match. Immutable.
    internal sealed class WildcardMatchFileRule : IContentSourceRule
    {
        private readonly IList<string> excludeFiles;

        public static IVirtualPathResolver VirtualPathResolver { get; internal set; }

        static WildcardMatchFileRule()
        {
            VirtualPathResolver = new VirtualPathResolver();
        }

        public WildcardMatchFileRule(string folderVirtualPath, string searchPattern, bool recursive, Mode mode, IVirtualPathResolver virtualPathResolver = null, IList<string> excludeFiles = null)
        {
            FolderVirtualPath = folderVirtualPath;
            SearchPattern = searchPattern;
            IsRecursive = recursive;

            if (virtualPathResolver != null)
            {
                VirtualPathResolver = virtualPathResolver;
            }

            this.excludeFiles = excludeFiles;

            Mode = mode;
        }

        internal string FolderVirtualPath { get; private set; }
        internal string SearchPattern { get; private set; }
        internal bool IsRecursive{ get; private set; }
        internal Mode Mode { get; private set; }

        private bool FileNotExcluded(FileInfo file)
        {
            return !excludeFiles.Contains(file.Name);
        }

        public IOrderedEnumerable<IContentSource> Evaluate(ResourceType resourceType)
        {
            var physicalPath = VirtualPathResolver.ResolveVirtualPathToDirectory(FolderVirtualPath);

            if (physicalPath == null)
                return Enumerable.Empty<IContentSource>().OrderBy(x => x);

            return Directory.GetFiles(physicalPath.FullName, SearchPattern ?? "*", IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                        .Select(f => new FileInfo(f))
                        .Where(FileNotExcluded)
                        .Select(file => PhysicalFileRegister.For(file, resourceType, Mode))
                        .OfType<IContentSource>()
                        .OrderBy(x => new FileInfo(((PhysicalFileSource)x).PhysicalPath).Name);
        }
    }
}
