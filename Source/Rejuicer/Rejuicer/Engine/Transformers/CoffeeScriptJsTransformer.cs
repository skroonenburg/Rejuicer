using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer.Engine.Transformers
{
    class CoffeeScriptJsTransformer : IFileTransformer
    {
        public byte[] TransformFile(PhysicalFileSource source, byte[] file)
        {
            var extension = Path.GetExtension(source.VirtualPath);

            if (extension != null && extension.ToUpperInvariant() == ".COFFEE")
            {
                return new CoffeeSharp.CoffeeScriptEngine().Compile(file.ReadString()).AsBytes();
            }

            return file;
        }
    }
}
