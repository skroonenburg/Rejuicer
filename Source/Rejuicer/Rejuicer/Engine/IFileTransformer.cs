using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer.Engine
{
    public interface IFileTransformer
    {
        Stream TransformFile(PhysicalFileSource source, Stream file);
    }
}
