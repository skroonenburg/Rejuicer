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
        private FileMatchModel _config;

        public DirectoryFileMatchConfigurer(ICompactorConfigurer compactorConfiguration, FileMatchModel config)
        {
            _compactorConfiguration = compactorConfiguration;
            _config = config;
        }

        public ICompactorConfigurer Matching(string wildcard)
        {
            _config.WildCard = wildcard;

            return _compactorConfiguration;
        }

        public ICompactorConfigurer All
        {
            get { _config.WildCard = null; return _compactorConfiguration; }
        }
    }
}
