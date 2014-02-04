﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Rejuicer.Configuration;
using System.Web.Caching;
using Rejuicer.Engine;
using System.IO;
using Rejuicer.HtmlHelpers;

namespace Rejuicer
{
    public static class Rejuiced
    {
        public static MvcHtmlString CssFor(string filename)
        {
            return IncludeRejuicedCssFor(null, filename);
        }

        public static MvcHtmlString JsFor(string filename)
        {
            return IncludeRejuicedJsFor(null, filename);
        }

        private static IVirtualPathResolver virtualPathResolver = new VirtualPathResolver();
        private static ICacheProvider cacheProvider = new CacheProvider();

        public static MvcHtmlString IncludeRejuicedJsFor(HtmlHelper instance, string filename)
        {
            var cachedValue = GetCachedIncludesFor(filename);
            if (cachedValue != null)
            {
                return cachedValue.RenderHtml();
            }

            var toInclude = GetIncludesFor(filename);
            var config = RejuicerEngine.GetConfigFor(filename);

            if (config == null)
            {
                return MvcHtmlString.Create("");
            }

            var dependencies = config.GetDependencies();

            var scripts = MvcHtmlString.Create(string.Join("\n", toInclude.Select(f =>
            {
                // Output <script src='' type=''>
                var script = new TagBuilder("script");
                script.Attributes.Add("src", virtualPathResolver.GetRelativeUrl(f));
                script.Attributes.Add("type", "text/javascript");

                return script.ToString(TagRenderMode.Normal);
            }).ToArray()));

            var cachedIncludes = new IncludesCacheModel { IncludesHtml = scripts, HashValue = config.GetHashValue(cacheProvider) };

            SetCachedIncludesFor(filename, cachedIncludes, dependencies);

            return cachedIncludes.RenderHtml();
        }

        public static MvcHtmlString IncludeRejuicedCssFor(this HtmlHelper instance, string filename)
        {
            var cachedValue = GetCachedIncludesFor(filename);
            if (cachedValue != null)
            {
                return cachedValue.RenderHtml();
            }

            var toInclude = GetIncludesFor(filename);
            var config = RejuicerEngine.GetConfigFor(filename);
            if (config == null)
            {
                return MvcHtmlString.Create("");
            }

            var dependencies = config.GetDependencies();

            var links = MvcHtmlString.Create(string.Join("\n", toInclude.Select(f =>
            {
                // Output <script src='' type=''>
                var link = new TagBuilder("link");
                link.Attributes.Add("href", virtualPathResolver.GetRelativeUrl(f));
                link.Attributes.Add("rel", "stylesheet");
                link.Attributes.Add("type", "text/css");

                return link.ToString(TagRenderMode.SelfClosing);
            }).ToArray()));

            var cachedIncludes = new IncludesCacheModel { IncludesHtml = links, HashValue = config.GetHashValue(cacheProvider) };

            SetCachedIncludesFor(filename, cachedIncludes, dependencies);

            return cachedIncludes.RenderHtml();
        }

        private static IEnumerable<string> GetIncludesFor(string filename)
        {
            var toInclude = new List<string>();

            if (RejuicerEngine.IsPassThroughEnabled)
            {
                toInclude.AddRange(RejuicerEngine.GetVirtualPathsFor(filename));
            }
            else
            {
                toInclude.Add(filename);
            }

            return toInclude;
        }

        private static IncludesCacheModel GetCachedIncludesFor(string filename)
        {
            return cacheProvider.Get<IncludesCacheModel>(GetCacheKeyFor(filename));
        }

        private static void SetCachedIncludesFor(string filename, IncludesCacheModel value, IEnumerable<FileInfo> files)
        {
            // Create a depdency on all of the files
            var dependencies = RejuicerEngine.IsPassThroughEnabled ? null :
                files.Where(f => f != null);

            cacheProvider.Add(GetCacheKeyFor(filename), value, dependencies);
        }

        private static string GetCacheKeyFor(string filename)
        {
            return string.Format("rejuicer-includes-{0}", filename);
        }
    }
}
