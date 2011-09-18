using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer
{
    internal class DirectoryFileMatchConfigurer : IDirectoryFileMatchConfigurer
    {
        private ICompactorConfigurer _compactorConfiguration;
        private readonly RejuicerConfigurationSource _configuration;
        private string _directoryPath;
        private readonly Mode _mode;

        public DirectoryFileMatchConfigurer(ICompactorConfigurer compactorConfiguration, RejuicerConfigurationSource configuration, string directoryPath, Mode mode)
        {
            _compactorConfiguration = compactorConfiguration;
            _configuration = configuration;
            _directoryPath = directoryPath;
            _mode = mode;
        }

        public ICompactorConfigurer Matching(string wildcard)
        {
            _configuration.AddRule(new WildcardMatchFileRule(_directoryPath, wildcard, false, _mode));

            return _compactorConfiguration;
        }

        public ICompactorConfigurer All
        {
            get { _configuration.AddRule(new WildcardMatchFileRule(_directoryPath, null, false, Mode.Minify)); return _compactorConfiguration; }
        }
    }
}
