using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rejuicer.Engine
{
    // Provides minification of a stream of data
    public interface IMinificationProvider
    {
        Stream Minify(Stream data);
        Stream Combine(IEnumerable<Stream> data);
        string GetContentType(string filename);
    }
}
