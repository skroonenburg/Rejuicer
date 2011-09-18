using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Rejuicer
{
    public interface IVirtualPathResolver
    {
        System.IO.DirectoryInfo ResolveVirtualPathToDirectory(string virtualPath);
        System.IO.FileInfo ResolveVirtualPathToFile(string virtualPath);
        string ResolveVirtualPath(string virtualPath);
        string GetRelativeUrl(string virtualPath);
        string GetVirtualPathFor(FileInfo file);
    }
}
