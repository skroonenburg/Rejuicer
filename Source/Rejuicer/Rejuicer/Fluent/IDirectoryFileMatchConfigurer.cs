using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejuicer
{
    public interface IDirectoryFileMatchConfigurer
    {
        ICompactorConfigurer Matching(string wildcard);
        ICompactorConfigurer All { get; }
    }
}
