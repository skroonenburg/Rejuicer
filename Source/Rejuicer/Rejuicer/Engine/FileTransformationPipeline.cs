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
        public static Stream TransformInputFile(PhysicalFileSource source, Stream inputContent)
        {
            var transformations = FileTransformerRegistry.GetTransformationsFor(source.ResourceType);

            if (transformations.Count() == 0)
            {
                return inputContent.Clone();
            }

            foreach (var transformation in transformations)
            {
                inputContent = transformation.TransformFile(source, inputContent);
                inputContent.Seek(0, SeekOrigin.Begin);
            }

            return inputContent;
        }
    }
}
