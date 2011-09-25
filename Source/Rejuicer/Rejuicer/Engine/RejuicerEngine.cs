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
        internal static Dictionary<string, RejuicerConfigurationSource> _configurations = new Dictionary<string,RejuicerConfigurationSource>();
        private static ICacheProvider cacheProvider = new CacheProvider();
        private static bool _configurationContainsPlaceholder = false;
        internal static IVirtualPathResolver _virtualPathResolver = new VirtualPathResolver();

        internal static void AddConfiguration(RejuicerConfigurationSource config)
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

            // Generate the content so that it is cached.
            config.GetContent(cacheProvider);

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
            // Get all dependencies of the matching content type of the parent.
            return FileResolver.VirtualPathsFor(GetConfigFor(requestedFilename));
        }

        public static IEnumerable<FileInfo> GetDependenciesFor(string requestedFilename)
        {
            return GetConfigFor(requestedFilename).GetDependencies();
        }

        private static PhysicalFileSource GetPhysicalFileFor(string requestedFilename)
        {
            requestedFilename = MakeVirtualPathFromRequestUrl(requestedFilename);

            return PhysicalFileRegister.For(requestedFilename);
        }

        public static bool PassThroughDebuggingFor(Uri requestedUrl)
        {
            return IsPassThroughEnabled && GetPhysicalFileFor(VirtualPathUtility.ToAppRelative(requestedUrl.AbsolutePath)) != null;
        }

        private static string MakeVirtualPathFromRequestUrl(string requestUrl)
        {
            return !requestUrl.StartsWith("~") ? string.Format("~{0}", requestUrl) : requestUrl;
        }

        internal static RejuicerConfigurationSource GetConfigFor(string requestedFilename)
        {
            try
            {
                _configurationLock.EnterReadLock();

                requestedFilename = MakeVirtualPathFromRequestUrl(requestedFilename);

                if (!_configurationContainsPlaceholder)
                {
                    // There are no placeholders in our configurations, so perform a quick dictionary lookup
                    RejuicerConfigurationSource model = null;
                    
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
                    var index = pair.Value.RequestFor.IndexOf(RejuicerConfigurationSource.FilenameUniquePlaceholder);

                    if (index >= 0)
                    {
                        if (requestedFilename.StartsWith(pair.Value.RequestFor.Substring(0, index))
                                    && requestedFilename.EndsWith(pair.Value.RequestFor.Substring(index + RejuicerConfigurationSource.
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

        public static OutputContent GetContentFor(Uri uri)
        {
            return GetContentFor(VirtualPathUtility.ToAppRelative(uri.AbsolutePath));
        }

        public static OutputContent GetContentFor(string requestedFilename)
        {
            return ((IContentSource)GetConfigFor(requestedFilename) ?? (IsPassThroughEnabled ? GetPhysicalFileFor(requestedFilename) : null)).GetContent(cacheProvider);
        }

        public static bool IsPassThroughEnabled
        {
            get
            {
                return HttpContext.Current.IsDebuggingEnabled && (Configuration == null || ((!Configuration.PreventPassThroughOnDebug.HasValue || !Configuration.PreventPassThroughOnDebug.Value)));
            }
        }
    }
}
