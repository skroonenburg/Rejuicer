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
        byte[] Minify(byte[] data);
        byte[] Combine(IEnumerable<byte[]> data);
        string GetContentType(string filename);
    }
}
