using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Rejuicer.Engine;

namespace Rejuicer.Model
{
    // A Rejuicer Configuration Source represents a configuration of combined/minified content.
    internal class RejuicerConfigurationSource : List<IContentSource>, IContentSource
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public RejuicerConfigurationSource(ResourceType resourceType, string requestedFile)
        {
            RequestFor = requestedFile;
            ResourceType = resourceType;

            Mode = Mode.Minify;
            ContainsPlaceHolder = requestedFile.Contains(FilenameUniquePlaceholder);

            if (ContainsPlaceHolder && requestedFile.IndexOf(FilenameUniquePlaceholder) != requestedFile.LastIndexOf(FilenameUniquePlaceholder))
            {
                throw new ArgumentException("Combined filename cannot contain two unique-code placeholders.");
            }

            Rules = new List<IContentSourceRule>();
        }

        internal List<IContentSourceRule> Rules { get; private set; }
        
        public Context Context { get; set; }

        public bool ContainsPlaceHolder { get; private set; }

        public ResourceType ResourceType { get; private set; }

        public string RequestFor { get; private set; }

        public const string FilenameUniquePlaceholder = "{0}";

        public Mode Mode { get; set; }

        public TimeSpan? CacheFor { get; set; }

        public void AddRule(IContentSourceRule rule)
        {
            _lock.EnterWriteLock();

            try
            {
                // Remember this rule, by adding it to the list of rules for this ContentSource
                Rules.Add(rule);

                // Evaluate the rule and add all resultant content sources
                // as children of this ContentSource
                foreach (var source in rule.Evaluate(ResourceType))
                {
                    Add(source);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void ReevaluateRules()
        {
            _lock.EnterWriteLock();
            try
            {
                Clear();
                AddRange(Rules.SelectMany(rule => rule.Evaluate(ResourceType)));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IEnumerable<FileInfo> GetDependencies()
        {
            return GetDependencies(null);
        }

        public IEnumerable<FileInfo> GetDependencies(ResourceType? resourceType)
        {
            _lock.EnterReadLock();
            try
            {
                return this.SelectMany(x => x.GetDependencies(resourceType)).Distinct();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IEnumerable<string> GetFiles()
        {
            return GetFiles(null);
        }

        public IEnumerable<string> GetFiles(ResourceType? resourceType)
        {
            _lock.EnterReadLock();
            try
            {
                return this.SelectMany(x => x.GetFiles(resourceType)).Distinct();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public DateTime GetLastModifiedDate(ICacheProvider cacheProvider)
        {
            return GetContent(cacheProvider).LastModifiedDate;
        }

        public string GetHashValue(ICacheProvider cacheProvider)
        {
            return GetContent(cacheProvider).ContentHash.HashStringValue();
        }

        public string GetTimestampedUrl(ICacheProvider cacheProvider)
        {
            return RequestFor.Replace(FilenameUniquePlaceholder, GetHashValue(cacheProvider));
        }

        public string GetNonTimestampedUrl(ICacheProvider cacheProvider)
        {
            return RequestFor.Replace(FilenameUniquePlaceholder, "");
        }

        public OutputContent GetContent(ICacheProvider cacheProvider)
        {
            var upgraded = false;
            _lock.EnterUpgradeableReadLock();

            try
            {
                // check the cache first
                var cachedValue = cacheProvider.Get<OutputContent>(GetCacheKeyFor(RequestFor));

                if (cachedValue == null)
                {
                    _lock.EnterWriteLock();
                    upgraded = true;

                    var content = this.Select(f => f.GetContent(cacheProvider));

                    var minificationProvider = MinificationRegistry.Get(ResourceType);

                    Log.WriteLine("Combining content for '{0}'", RequestFor);

                    // Combine all of the files into one string
                    var minifiedContent = minificationProvider.Combine(content.Select(x => x.Content));
                    cachedValue = new OutputContent
                                      {
                                          Content = minifiedContent,
                                          ContentHash = minifiedContent.HashArray(),
                                          AllowClientCaching = ContainsPlaceHolder || CacheFor.HasValue,
                                          CacheFor = CacheFor,
                                          ContentType = minificationProvider.GetContentType(RequestFor),
                                          LastModifiedDate = content.Max(x => x.LastModifiedDate)
                                      };

                    cacheProvider.Add(GetCacheKeyFor(RequestFor), cachedValue, GetDependencies());
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
            return string.Format("rejuicer-cfgsource-{0}", filename);
        }
    }
}
