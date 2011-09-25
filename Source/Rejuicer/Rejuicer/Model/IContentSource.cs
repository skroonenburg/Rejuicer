using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rejuicer.Engine;

namespace Rejuicer.Model
{
    public interface IContentSource
    {
        ResourceType ResourceType { get; }
        IEnumerable<FileInfo> GetDependencies(ResourceType? resourceType);
        IEnumerable<FileInfo> GetDependencies();
        OutputContent GetContent(ICacheProvider cacheProvider);
    }
}
