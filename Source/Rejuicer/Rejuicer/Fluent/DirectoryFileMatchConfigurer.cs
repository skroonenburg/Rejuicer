using System.Collections.Generic;
using Rejuicer.Model;
using Rejuicer.Rules;

namespace Rejuicer
{
    internal class DirectoryFileMatchConfigurer : IDirectoryFileMatchConfigurer
    {
        private readonly ICompactorConfigurer _compactorConfiguration;
        private readonly RejuicerConfigurationSource _configuration;
        private readonly string _directoryPath;
        private readonly Mode _mode;
        private readonly IList<string> _excludeFiles = new List<string>(); 

        public DirectoryFileMatchConfigurer(ICompactorConfigurer compactorConfiguration, RejuicerConfigurationSource configuration, string directoryPath, Mode mode)
        {
            _compactorConfiguration = compactorConfiguration;
            _configuration = configuration;
            _directoryPath = directoryPath;
            _mode = mode;
        }

        public ICompactorConfigurer Matching(string wildcard)
        {
            _configuration.AddRule(new WildcardMatchFileRule(_directoryPath, wildcard, false, _mode, null, _excludeFiles));

            return _compactorConfiguration;
        }

        public ICompactorConfigurer All
        {
            get
            {
                _configuration.AddRule(new WildcardMatchFileRule(_directoryPath, null, false, Mode.Minify, null, _excludeFiles));
                return _compactorConfiguration;
            }
        }

        public IDirectoryFileMatchConfigurer Excluding(string filename)
        {
            _excludeFiles.Add(filename);
            return this;
        }
    }
}
