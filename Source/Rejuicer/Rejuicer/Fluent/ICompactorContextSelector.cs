using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejuicer
{
    public interface ICompactorModeSelector
    {
        ICompactorConfigurer Compact { get; }
        ICompactorConfigurer Combine { get; }
    }
}
