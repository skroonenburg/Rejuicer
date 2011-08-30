using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Mvc;

namespace Rejuicer
{
    public class VirtualPathResolver : IVirtualPathResolver
    {
        public System.IO.DirectoryInfo ResolveVirtualPathToDirectory(string virtualPath)
        {
            var directoryPath = ResolveVirtualPath(virtualPath);

            if (!Directory.Exists(directoryPath))
            {
                return null;
            }

            return new DirectoryInfo(directoryPath);
        }

        public System.IO.FileInfo ResolveVirtualPathToFile(string virtualPath)
        {
            var filePath = ResolveVirtualPath(virtualPath);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return new FileInfo(filePath);
        }

        public string ResolveVirtualPath(string virtualPath)
        {
            return HttpContext.Current.Request.MapPath(virtualPath);
        }

        public string GetRelativeUrl(string virtualPath)
        {
            return UrlHelper.GenerateContentUrl(virtualPath, new HttpContextWrapper(HttpContext.Current));
        }
    }
}
