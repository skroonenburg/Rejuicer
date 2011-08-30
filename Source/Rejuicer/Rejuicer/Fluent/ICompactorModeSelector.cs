using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejuicer
{
    public interface ICompactorContextSelector
    {
        ICompactorContextSelector Always { get; }
        ICompactorContextSelector ExceptWhenDebugging { get; }
    }
}
