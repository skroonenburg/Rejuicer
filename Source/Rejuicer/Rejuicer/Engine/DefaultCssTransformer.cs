using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Rejuicer.Model;

namespace Rejuicer.Engine
{
    public class DefaultCssTransformer : IFileTransformer
    {
        private readonly IVirtualPathResolver _virtualPathResolver;
        private ICacheProvider _cacheProvider;

        public DefaultCssTransformer()
            : this(new VirtualPathResolver(), new CacheProvider())
        {
            _virtualPathResolver = new VirtualPathResolver();
            _cacheProvider = new CacheProvider();
        }

        public DefaultCssTransformer(IVirtualPathResolver virtualPathResolver)
            : this(virtualPathResolver, new CacheProvider())
        {
        }

        public DefaultCssTransformer(IVirtualPathResolver virtualPathResolver, ICacheProvider cacheProvider)
        {
            _virtualPathResolver = virtualPathResolver;
            _cacheProvider = cacheProvider;
        }

        public byte[] TransformFile(PhysicalFileSource source, byte[] inputContent)
        {
            Log.WriteLine("Transforming CSS For '{0}'", source.VirtualPath);
            string stringValue = inputContent.ReadString();

            stringValue = Regex.Replace(stringValue, "url\\((?<quotation>['\"]?)(?<capturedUrl>[^\\)]*)", x =>
                                                                              {
                                                                                  var url = x.Groups["capturedUrl"];
                                                                                  if (url != null)
                                                                                  {
                                                                                      var urlValue = url.Value.Trim();

                                                                                      if (!urlValue.StartsWith("~"))
                                                                                      {
                                                                                          return x.Value;
                                                                                      }

                                                                                      var quotation = x.Groups["quotation"];
                                                                                      var quoteWrapper = quotation != null ? quotation.Value : "";

                                                                                      if (urlValue.EndsWith(quoteWrapper))
                                                                                      {
                                                                                          urlValue = urlValue.Substring(0, urlValue.Length - quoteWrapper.Length);
                                                                                      }

                                                                                      var virtualPath = urlValue;

                                                                                      // look for a placeholder in the virtual path
                                                                                      if (virtualPath.Contains(RejuicerConfigurationSource.FilenameUniquePlaceholder))
                                                                                      {
                                                                                          // Look for a configuration
                                                                                          if (!RejuicerEngine.HasConfigurationFor(virtualPath))
                                                                                          {
                                                                                              // Create a configuration for this file
                                                                                              OnRequest.ForImage(virtualPath)
                                                                                                  .Combine
                                                                                                  .File(virtualPath.Replace(RejuicerConfigurationSource.FilenameUniquePlaceholder, ""))
                                                                                                  .Configure();
                                                                                          }

                                                                                          // get the timestamp and write it out into the URL... 
                                                                                          var config = RejuicerEngine.GetConfigFor(virtualPath);

                                                                                          if (config != null)
                                                                                          {
                                                                                              // don't timestamp the URL in pass through mode
                                                                                              virtualPath = RejuicerEngine.IsPassThroughEnabled ? config.GetNonTimestampedUrl(_cacheProvider) : config.GetTimestampedUrl(_cacheProvider);

                                                                                              // Need to add a dependency of this CSS file to the now linked image.
                                                                                              source.AddDependency(RejuicerEngine.GetConfigFor(urlValue));
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              // Could not configure rejuicer for this file. It may not exist,
                                                                                              // so just return the url
                                                                                              return url.Value;
                                                                                          }
                                                                                      }

                                                                                      var outputUrl = _virtualPathResolver.GetRelativeUrl(virtualPath);

                                                                                      return string.Format("url({1}{0}{1}", outputUrl, quoteWrapper);
                                                                                  }

                                                                                  return x.Value;
                                                                              });

            

            return stringValue.AsBytes();
        }
    }
}
