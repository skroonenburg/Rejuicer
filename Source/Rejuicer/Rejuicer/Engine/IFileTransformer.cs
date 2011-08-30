using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rejuicer.Engine
{
    public interface IFileTransformer
    {
        string TransformFile(string file);
    }
}
