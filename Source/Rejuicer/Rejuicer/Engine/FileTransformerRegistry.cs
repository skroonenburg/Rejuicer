using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer.Engine
{
    public static class FileTransformerRegistry
    {
        private static IDictionary<ResourceType, IList<IFileTransformer>> _transformerRegistrations = new Dictionary<ResourceType, IList<IFileTransformer>>();

        static FileTransformerRegistry()
        {
            _transformerRegistrations.Add(ResourceType.Css, new List<IFileTransformer>(new DefaultCssTransformer[] { new DefaultCssTransformer()}));
            _transformerRegistrations.Add(ResourceType.Js, new List<IFileTransformer>());
        }

        public static void Register(ResourceType resourceType, IFileTransformer fileTransformer)
        {
            if (!_transformerRegistrations.ContainsKey(resourceType))
            {
                _transformerRegistrations[resourceType] = new List<IFileTransformer>();
            }

            _transformerRegistrations[resourceType].Add(fileTransformer);
        }

        public static void ClearRegistrations(ResourceType resourceType)
        {
            if (_transformerRegistrations.ContainsKey(resourceType))
            {
                _transformerRegistrations[resourceType].Clear();
            }
        }

        public static void Unregister(ResourceType resourceType, IFileTransformer fileTransformer)
        {
            if (_transformerRegistrations.ContainsKey(resourceType))
            {
                var list = _transformerRegistrations[resourceType];
                if (list.Contains(fileTransformer))
                {
                    list.Remove(fileTransformer);
                }
            }
        }

        public static IEnumerable<IFileTransformer> GetTransformationsFor(ResourceType resourceType)
        {
            if (_transformerRegistrations.ContainsKey(resourceType))
            {
                return _transformerRegistrations[resourceType];
            }

            return Enumerable.Empty<IFileTransformer>();
        }
    }
}
