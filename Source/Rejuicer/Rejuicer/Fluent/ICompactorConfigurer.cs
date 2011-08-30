using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rejuicer
{
    public interface ICompactorConfigurer
    {
        ICompactorConfigurer File(string filename);
        IDirectoryFileMatchConfigurer FilesIn(string path);

        ICompactorConfigurer DoNotCache { get; }
        void Configure();
    }
}
