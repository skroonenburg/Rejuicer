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
        byte[] TransformFile(PhysicalFileSource source, byte[] file);
    }
}
