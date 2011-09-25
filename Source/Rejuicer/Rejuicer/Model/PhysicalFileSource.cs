using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Rejuicer.Engine;

namespace Rejuicer.Model
{
    public class PhysicalFileSource : IContentSource, IPhysicalFileDependency
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public PhysicalFileSource(ResourceType resourceType, string virtualPath, string physicalPath)
        {
            VirtualPath = virtualPath;
            ResourceType = resourceType;
            PhysicalPath = physicalPath;
            _dependencies = new List<IContentSource>();
        }

        public ResourceType ResourceType { get; private set; }
        public Mode Mode { get; private set; }

        public IEnumerable<FileInfo> GetDependencies()
        {
            return GetDependencies(null);
        }

        public IEnumerable<FileInfo> GetDependencies(ResourceType? resourceType)
        {
            _lock.EnterReadLock();
            try
            {
                var dependencies = _dependencies.SelectMany(x => x.GetDependencies(resourceType));
                
                if (resourceType.HasValue && ResourceType == resourceType.Value)
                {
                    dependencies = dependencies.Union(new[] { new FileInfo(PhysicalPath) });
                }

                return dependencies;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public OutputContent GetContent(ICacheProvider cacheProvider)
        {
            var upgraded = false;
            _lock.EnterUpgradeableReadLock();

            try
            {
                // check the cache first
                var cachedValue = cacheProvider.Get<OutputContent>(GetCacheKeyFor(VirtualPath));

                if (cachedValue == null)
                {
                    _lock.EnterWriteLock();
                    upgraded = true;

                    Stream rejuicedValue = null;

                    var file = new FileInfo(PhysicalPath);

                    // clear existing dependencies
                    _dependencies.Clear();

                    using (var fileStream = file.OpenRead())
                    {
                        rejuicedValue = FileTransformationPipeline.TransformInputFile(this, fileStream);
                    }

                    // Combine all of the files into one string
                    cachedValue = new OutputContent
                                      {
                                          Content = rejuicedValue,
                                          AllowClientCaching = false,
                                          ContentType =
                                              ResourceType == ResourceType.Css ? "text/css" : "text/javascript",
                                          LastModifiedDate = file.LastWriteTimeUtc
                                      };

                    cacheProvider.Add(GetCacheKeyFor(VirtualPath), cachedValue, GetDependencies());
                }

                return cachedValue;
            }
            finally
            {
                if (upgraded)
                {
                    _lock.ExitWriteLock();
                }

                _lock.ExitUpgradeableReadLock();
            }
        }

        private static string GetCacheKeyFor(string filename)
        {
            return string.Format("rejuicer-filesource-{0}", filename);
        }
            
        public string VirtualPath { get; private set; }
        public string PhysicalPath { get; private set; }
        private List<IContentSource> _dependencies;
 
        public void ClearDependencies()
        {
            _lock.EnterWriteLock();
            try
            {
                _dependencies.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void AddDependency(IContentSource source)
        {
            _lock.EnterWriteLock();
            try
            {
                _dependencies.Add(source);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
