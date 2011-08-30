using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer
{
    internal class CompactorConfigurer : ICompactorModeSelector, ICompactorConfigurer, ICompactorContextSelector
    {
        protected internal RejuicedFileModel _config;

        public CompactorConfigurer(RejuicedFileModel config)
        {
            _config = config;
        }

        public ICompactorConfigurer Compact
        {
            get { _config.Mode = Mode.Compact; return this; }
        }

        public ICompactorConfigurer Combine
        {
            get { _config.Mode = Mode.Combine; return this; }
        }

        public ICompactorConfigurer File(string filename)
        {
            _config.AddFile(filename);
            return this;
        }

        public IDirectoryFileMatchConfigurer FilesIn(string path)
        {
            var fileMatchConfig = new FileMatchModel(path);
            _config.AddFilesMatching(fileMatchConfig);

            return new DirectoryFileMatchConfigurer(this, fileMatchConfig);
        }

        public void Configure()
        {
            // Pass this configuration to the compactor, so that it is remembered.
            RejuicerEngine.AddConfiguration(_config);
        }


        public ICompactorConfigurer DoNotCache
        {
            get { _config.Cache = false; return this; }
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
