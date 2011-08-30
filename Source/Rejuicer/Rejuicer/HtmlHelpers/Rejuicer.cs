using System;
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
        public static string CssFor(string filename)
        {
            return IncludeRejuicedCssFor(null, filename);
        }

        public static string JsFor(string filename)
        {
            return IncludeRejuicedJsFor(null, filename);
        }

        private static IVirtualPathResolver virtualPathResolver = new VirtualPathResolver();
        private static ICacheProvider cacheProvider = new CacheProvider();

        public static string IncludeRejuicedJsFor(this HtmlHelper instance, string filename)
        {
            var cachedValue = GetCachedIncludesFor(filename);
            if (cachedValue != null)
            {
                return cachedValue.RenderHtml();
            }

            var toInclude = GetIncludesFor(filename);
            var dependencies = GetDependenciesFor(filename);

            var scripts = string.Join("\n", toInclude.Select(f =>
            {
                // Output <script src='' type=''>
                var script = new TagBuilder("script");
                script.Attributes.Add("src", UrlHelper.GenerateContentUrl(f, new HttpContextWrapper(HttpContext.Current)));
                script.Attributes.Add("type", "text/javascript");

                return script.ToString(TagRenderMode.Normal);
            }).ToArray());

            var cachedIncludes = new IncludesCacheModel { IncludesHtml = scripts, UniqueCode = Guid.NewGuid() };

            SetCachedIncludesFor(filename, cachedIncludes, dependencies);

            return cachedIncludes.RenderHtml();
        }

        public static string IncludeRejuicedCssFor(this HtmlHelper instance, string filename)
        {
            var cachedValue = GetCachedIncludesFor(filename);
            if (cachedValue != null)
            {
                return cachedValue.RenderHtml();
            }

            var toInclude = GetIncludesFor(filename);
            var dependencies = GetDependenciesFor(filename);

            var links = string.Join("\n", toInclude.Select(f =>
            {
                // Output <script src='' type=''>
                var link = new TagBuilder("link");
                link.Attributes.Add("href", virtualPathResolver.GetRelativeUrl(f));
                link.Attributes.Add("rel", "stylesheet");
                link.Attributes.Add("type", "text/css");

                return link.ToString(TagRenderMode.SelfClosing);
            }).ToArray());

            var cachedIncludes = new IncludesCacheModel { IncludesHtml = links, UniqueCode = Guid.NewGuid() };

            SetCachedIncludesFor(filename, cachedIncludes, dependencies);

            return cachedIncludes.RenderHtml();
        }

        private static bool PassThroughOnDebugging
        {
            get
            {
                return HttpContext.Current.IsDebuggingEnabled && !(RejuicerConfiguration.Current != null && RejuicerConfiguration.Current.PreventPassThroughOnDebug.HasValue && RejuicerConfiguration.Current.PreventPassThroughOnDebug.Value);
            }
        }

        private static IEnumerable<string> GetIncludesFor(string filename)
        {
            var toInclude = new List<string>();

            if (PassThroughOnDebugging)
            {
                toInclude.AddRange(RejuicerEngine.GetVirtualPathsFor(filename));
            }
            else
            {
                toInclude.Add(filename);
            }

            return toInclude;
        }

        private static IEnumerable<FileInfo> GetDependenciesFor(string filename)
        {
            return RejuicerEngine.GetDependenciesFor(filename);
        }

        private static IncludesCacheModel GetCachedIncludesFor(string filename)
        {
            return cacheProvider.Get<IncludesCacheModel>(filename);
        }

        private static void SetCachedIncludesFor(string filename, IncludesCacheModel value, IEnumerable<FileInfo> files)
        {
            // Create a depdency on all of the files
            var dependencies = PassThroughOnDebugging ? null :
                files.Where(f => f != null);

            cacheProvider.Add(filename, value, dependencies);
        }
    }
}
