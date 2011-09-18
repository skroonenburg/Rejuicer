using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rejuicer.Model
{
    // Represents a configuration rule for a content source
    internal interface IContentSourceRule
    {
        IOrderedEnumerable<IContentSource> Evaluate(ResourceType resourceType);
    }
}
