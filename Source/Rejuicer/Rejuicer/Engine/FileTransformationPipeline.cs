using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer.Engine
{
    public static class FileTransformationPipeline
    {
        public static byte[] TransformInputFile(PhysicalFileSource source, byte[] inputContent)
        {
            var transformations = FileTransformerRegistry.GetTransformationsFor(source.ResourceType);

            if (transformations.Count() == 0)
            {
                return inputContent.CloneBytes();
            }

            return transformations.Aggregate(inputContent, (current, transformation) => transformation.TransformFile(source, current));
        }
    }
}
