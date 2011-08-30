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
        public static string TransformInputFile(string inputContent, ResourceType resourceType)
        {
            foreach (var transformation in FileTransformerRegistry.GetTransformationsFor(resourceType))
            {
                inputContent = transformation.TransformFile(inputContent);
            }

            return inputContent;
        }
    }
}
