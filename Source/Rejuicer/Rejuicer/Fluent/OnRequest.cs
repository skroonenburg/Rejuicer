using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer
{
    public static class OnRequest
    {
        public static ICompactorModeSelector ForCss(Func<string, string> filenameConstructor)
        {
            return ForCss(filenameConstructor(RejuicerConfigurationSource.FilenameUniquePlaceholder));
        }

        public static ICompactorModeSelector ForJs(Func<string, string> filenameConstructor)
        {
            return ForJs(filenameConstructor(RejuicerConfigurationSource.FilenameUniquePlaceholder));
        }

        public static ICompactorModeSelector ForCss(string requestedFilename)
        {
            var config = new RejuicerConfigurationSource(ResourceType.Css, requestedFilename);
            return new CompactorConfigurer(config);
        }

        public static ICompactorModeSelector ForJs(string requestedFilename)
        {
            var config = new RejuicerConfigurationSource(ResourceType.Js, requestedFilename);
            return new CompactorConfigurer(config);
        }

        public static ICompactorModeSelector ForImage(Func<string, string> filenameConstructor)
        {
            return ForImage(filenameConstructor(RejuicerConfigurationSource.FilenameUniquePlaceholder));
        }

        public static ICompactorModeSelector ForImage(string requestedFilename)
        {
            var config = new RejuicerConfigurationSource(ResourceType.Image, requestedFilename);
            return new CompactorConfigurer(config);
        }
    }
}
