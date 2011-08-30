using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Rejuicer.Engine
{
    public interface ICacheProvider
    {
        void Add(string key, object value, IEnumerable<FileInfo> dependencies);
        T Get<T>(string key);
    }
}
