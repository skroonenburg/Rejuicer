using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Rejuicer.Model;
using Rejuicer.Configuration;
using Rejuicer.Engine;

namespace Rejuicer
{
    internal static class RejuicerEngine
    {
        private static ReaderWriterLockSlim _configurationLock = new ReaderWriterLockSlim();
        internal static Dictionary<string, RejuicedFileModel> _configurations = new Dictionary<string,RejuicedFileModel>();
        private static ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private static ICacheProvider cacheProvider = new CacheProvider();
        private static bool _configurationContainsPlaceholder = false;

        internal static void AddConfiguration(RejuicedFileModel config)
        {
            try
            {
                _configurationLock.EnterWriteLock();
                
                if (config.ContainsPlaceHolder)
                {
                    // Remember that there is a placeholder in one of the configurations.
                    _configurationContainsPlaceholder = true;
                }

                _configurations.Add(config.RequestFor, config);
            }
            finally
            {
                _configurationLock.ExitWriteLock();
            }
        }

        internal static void ClearConfigurations()
        {
            try
            {
                _configurationLock.EnterWriteLock();
                
                _configurations.Clear();
                _configurationContainsPlaceholder = false;
            }
            finally
            {
                _configurationLock.ExitWriteLock();
            }
        }

        public static bool HasConfigurationFor(Uri uri)
        {
            return HasConfigurationFor(VirtualPathUtility.ToAppRelative(uri.AbsolutePath));
        }

        public static bool HasConfigurationFor(string virtualPath)
        {
            return GetConfigFor(virtualPath) != null;
        }

        internal static IEnumerable<string> GetVirtualPathsFor(string requestedFilename)
        {
            return FileResolver.VirtualPathsFor(GetConfigFor(requestedFilename));
        }

        public static IEnumerable<FileInfo> GetDependenciesFor(string requestedFilename)
        {
            return FileResolver.Resolve(GetConfigFor(requestedFilename));
        }

        private static RejuicedFileModel GetConfigFor(string requestedFilename)
        {
            try
            {
                _configurationLock.EnterReadLock();

                if (!requestedFilename.StartsWith("~"))
                {
                    requestedFilename = string.Format("~{0}", requestedFilename);
                }

                if (!_configurationContainsPlaceholder)
                {
                    // There are no placeholders in our configurations, so perform a quick dictionary lookup
                    RejuicedFileModel model = null;
                    
                    if (_configurations.TryGetValue(requestedFilename, out model))
                    {
                        return model;
                    }

                    // No matching file configurations were found, so return nothing.
                    return null;
                }

                // There are placeholders in the configuration, so iterate over each and look for a match
                foreach (var pair in _configurations)
                {
                    var index = pair.Value.RequestFor.IndexOf(RejuicedFileModel.FilenameUniquePlaceholder);

                    if (index >= 0)
                    {
                        if (requestedFilename.StartsWith(pair.Value.RequestFor.Substring(0, index))
                                    && requestedFilename.EndsWith(pair.Value.RequestFor.Substring(index + RejuicedFileModel.
                                                                                                    FilenameUniquePlaceholder
                                                                                                    .Length)))
                        {
                            return pair.Value;
                        }
                    }
                    else if (pair.Value.RequestFor.Equals(requestedFilename))
                    {
                        return pair.Value;
                    }
                }

                return null;
            }
            finally
            {
                _configurationLock.ExitReadLock();
            }
        }

        internal static RejuicerConfiguration Configuration
        {
            get
            {
                return RejuicerConfiguration.Current;
            }
        }

        internal static bool IsCache(RejuicedFileModel model)
        {
            return model.Cache || (Configuration != null && Configuration.Cache.HasValue && Configuration.Cache.Value);
        }

        public static OutputContent GetContentFor(Uri uri)
        {
            return GetContentFor(VirtualPathUtility.ToAppRelative(uri.AbsolutePath));
        }

        public static OutputContent GetContentFor(string requestedFilename)
        {
            var config = GetConfigFor(requestedFilename);
            var isCacheEnabled = IsCache(config);

            if (!isCacheEnabled)
            {
                // Caching is not enabled, so generate the content now
                return GenerateContentFor(config.ResourceType, config.Mode, FileResolver.Resolve(config)).Configure(config);
            }

            var upgraded = false;
            try
            {
                // Open a read lock for the cache
                _cacheLock.EnterUpgradeableReadLock();

                // Lookup & return the cached value
                var compacted = cacheProvider.Get<OutputContent>(CacheKeyFor(config));

                if (compacted != null)
                {
                    // Return the cached value
                    return compacted;
                }

                // We need to update the cache, so enter a write lock.
                // This will prevent multiple requests from running the compactor
                // and updating the cache.
                _cacheLock.EnterWriteLock();
                upgraded = true;

                // First check if the cache is still empty, or if another thread has updated it.
                var cacheValue = cacheProvider.Get<OutputContent>(CacheKeyFor(config));
                if (cacheValue != null)
                {
                    return cacheValue;
                }

                // Still no cache value, let's generate it.
                // Resolve all of the files in this configuration
                var files = FileResolver.Resolve(config);

                // Compact all of these files
                var compactedValue = GenerateContentFor(config.ResourceType, config.Mode, files).Configure(config);

                // Create a depdency on all of the files
                cacheProvider.Add(CacheKeyFor(config), compactedValue, files);

                return compactedValue;
            }
            finally
            {
                if (upgraded)
                {
                    _cacheLock.ExitWriteLock();
                }

                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        private static OutputContent GenerateContentFor(ResourceType type, Mode mode, IEnumerable<FileInfo> files)
        {
            var output = new OutputContent();

            // Combine all of the files into one string
            var combined = string.Join(Environment.NewLine, files.Select(f => { using (var reader = f.OpenText()) { return FileTransformationPipeline.TransformInputFile(reader.ReadToEnd(), type); }}).ToArray());

            if (mode == Mode.Compact)
            {
                var culture = Thread.CurrentThread.CurrentCulture;

                try
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                    if (type == ResourceType.Css)
                    {
                        // Perform compaction on files
                        combined = Yahoo.Yui.Compressor.CssCompressor.Compress(combined);
                    }
                    else
                    {
                        combined = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(combined);
                    }
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                }
            }

            output.Content = combined;
            output.ContentType = type == ResourceType.Css ? "text/css" : "text/javascript";

            // Return compaction
            return output;
        }

        private static string CacheKeyFor(RejuicedFileModel config)
        {
            return string.Format("Compactor_{0}", config.RequestFor);
        }
    }
}
