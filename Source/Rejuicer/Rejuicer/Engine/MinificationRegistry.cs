using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Engine.MinificationProviders;
using Rejuicer.Model;

namespace Rejuicer.Engine
{
    public static class MinificationRegistry
    {
        private static Dictionary<ResourceType, IMinificationProvider> _minificationProviders = new Dictionary<ResourceType, IMinificationProvider>();

        static MinificationRegistry()
        {
            _minificationProviders[ResourceType.Css] = new CssMinificationProvider();
            _minificationProviders[ResourceType.Js] = new JsMinificationProvider();
            _minificationProviders[ResourceType.Image] = new ImageMinificationProvider();
        }

        public static void Register(ResourceType resourceType, IMinificationProvider minificationProvider)
        {
            _minificationProviders[resourceType] = minificationProvider;
        }

        public static IMinificationProvider Get(ResourceType resourceType)
        {
            return _minificationProviders[resourceType];
        }
    }
}
