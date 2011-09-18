using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer
{
    public interface IDirectoryFileMatchConfigurer
    {
        ICompactorConfigurer Matching(string wildcard);
        ICompactorConfigurer All { get; }
    }
}
