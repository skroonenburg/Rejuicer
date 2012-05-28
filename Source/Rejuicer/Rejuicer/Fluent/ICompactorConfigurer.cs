using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer
{
    public interface ICompactorConfigurer
    {
        ICompactorConfigurer File(string filename);
        ICompactorConfigurer File(string virtualPath, Mode mode);
        IDirectoryFileMatchConfigurer FilesIn(string path);
        IDirectoryFileMatchConfigurer FilesIn(string path, Mode mode);
        ICompactorConfigurer CacheFor(TimeSpan cacheTime);

        void Configure();
    }
}
