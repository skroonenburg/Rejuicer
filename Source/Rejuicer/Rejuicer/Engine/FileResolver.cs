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
        public static IEnumerable<FileInfo> Resolve(RejuicedFileModel config)
        {
            List<FileInfo> allFiles = new List<FileInfo>();

            if (config == null)
            {
                return allFiles;
            }

            var orderedFiles = config.OrderedFiles;
            
            foreach (var fileSpec in orderedFiles)
            {
                if(fileSpec is string)
                {
                    var fileName = (string) fileSpec;
                    allFiles.Add(VirtualPathResolver.ResolveVirtualPathToFile(fileName));
                }

                else if(fileSpec is FileMatchModel)
                {
                    var model = (FileMatchModel)fileSpec;
                    var physicalPath = VirtualPathResolver.ResolveVirtualPathToDirectory(model.Path);

                    if (physicalPath == null)
                        continue;

                    allFiles.AddRange(Directory.GetFiles(physicalPath.FullName, model.WildCard, SearchOption.TopDirectoryOnly)
                        .Select(file => new FileInfo(file)));
                }
            }

            return allFiles.Distinct(new FileComparer());
        }

        public static IEnumerable<string> VirtualPathsFor(RejuicedFileModel config)
        {
            var allFiles = new List<string>();

            if (config == null)
                return allFiles;

            allFiles.AddRange(Resolve(config).Where(f=> f != null).Select(f => GetVirtualRelativeFor(f.FullName)));

            return allFiles;
        }

        internal static string GetVirtualRelativeFor(string filename)
        {
            var root = HttpContext.Current.Request.MapPath("~");
            return string.Format("~/{0}", filename.Substring(root.Length).Replace('\\', '/'));
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
