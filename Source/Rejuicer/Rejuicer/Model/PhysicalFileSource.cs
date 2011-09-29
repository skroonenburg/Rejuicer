using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public PhysicalFileSource(ResourceType resourceType, string virtualPath, string physicalPath, Mode mode)
        {
            Mode = mode;
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

                if (!resourceType.HasValue || (resourceType.HasValue && ResourceType == resourceType.Value))
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
            return GetContent(cacheProvider, Mode);
        }

        public OutputContent GetContent(ICacheProvider cacheProvider, Mode mode)
        {
            var upgraded = false;
            _lock.EnterUpgradeableReadLock();

            try
            {
                // check the cache first
                var returnValue = cacheProvider.Get<OutputContent>(GetCacheKeyFor(VirtualPath, mode));

                if (returnValue == null)
                {
                    Log.WriteLine("Generating Content For '{0}'", VirtualPath);

                    _lock.EnterWriteLock();
                    upgraded = true;

                    byte[] rejuicedValue = null;

                    var file = new FileInfo(PhysicalPath);

                    // clear existing dependencies
                    _dependencies.Clear();

                    var minificationProvider = MinificationRegistry.Get(ResourceType);

                    var notFound = false;
                    try
                    {
                        var fileBytes = File.ReadAllBytes(PhysicalPath);
                        rejuicedValue = FileTransformationPipeline.TransformInputFile(this, fileBytes);
                    }
                    catch (IOException)
                    {
                    }
                    
                    // Combined value
                    var combinedValue = new OutputContent
                    {
                        Content = rejuicedValue,
                        AllowClientCaching = false,
                        ContentType =
                            ResourceType == ResourceType.Css ? "text/css" : "text/javascript",
                        LastModifiedDate = file.LastWriteTimeUtc
                    };

                    var dependencies = GetDependencies();
                    cacheProvider.Add(GetCacheKeyFor(VirtualPath, Mode.Combine), combinedValue, dependencies);
                    returnValue = combinedValue;

                    if (Mode == Mode.Minify && !notFound)
                    {
                        Log.WriteLine("Minifying Content For '{0}'", VirtualPath);

                        // Minified value
                        var minifiedValue = new OutputContent
                                                {
                                                    Content = minificationProvider.Minify(rejuicedValue),
                                                    AllowClientCaching = false,
                                                    ContentType =
                                                        ResourceType == ResourceType.Css
                                                            ? "text/css"
                                                            : "text/javascript",
                                                    LastModifiedDate = file.LastWriteTimeUtc
                                                };

                        cacheProvider.Add(GetCacheKeyFor(VirtualPath, mode), minifiedValue, dependencies);

                        returnValue = minifiedValue;
                    }
                }

                return returnValue;
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

        private static string GetCacheKeyFor(string filename, Mode mode)
        {
            return string.Format("rejuicer-{1}-filesource-{0}", filename, mode == Mode.Combine ? "combined" : "minified");
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
