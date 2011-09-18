using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer
{
    internal class CompactorConfigurer : ICompactorModeSelector, ICompactorConfigurer, ICompactorContextSelector
    {
        protected internal RejuicerConfigurationSource _config;

        public CompactorConfigurer(RejuicerConfigurationSource config)
        {
            _config = config;
        }

        public ICompactorConfigurer Compact
        {
            get { _config.Mode = Mode.Minify; return this; }
        }

        public ICompactorConfigurer Combine
        {
            get { _config.Mode = Mode.Combine; return this; }
        }

        public ICompactorConfigurer File(string virtualPath)
        {
            return File(virtualPath, Mode.Minify);
        }

        public ICompactorConfigurer File(string virtualPath, Mode mode)
        {
            _config.AddRule(new SingleFileRule(virtualPath, mode));
            return this;
        }

        public IDirectoryFileMatchConfigurer FilesIn(string path)
        {
            return FilesIn(path, Mode.Minify);
        }

        public IDirectoryFileMatchConfigurer FilesIn(string path, Mode mode)
        {
            return new DirectoryFileMatchConfigurer(this, _config, path, mode);
        }

        public void Configure()
        {
            // Pass this configuration to the compactor, so that it is remembered.
            RejuicerEngine.AddConfiguration(_config);
        }

        public ICompactorContextSelector Always
        {
            get { _config.Context = Context.Always; return this; }
        }

        public ICompactorContextSelector ExceptWhenDebugging
        {
            get { _config.Context = Context.ExceptWhenDebugging; return this; }
        }
    }
}
