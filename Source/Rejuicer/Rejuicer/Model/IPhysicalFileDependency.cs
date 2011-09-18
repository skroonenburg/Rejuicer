using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejuicer.Model
{
    public interface IPhysicalFileDependency
    {
        void ClearDependencies();
        void AddDependency(IContentSource source);
    }
}
