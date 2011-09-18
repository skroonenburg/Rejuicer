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

        public Stream TransformFile(PhysicalFileSource source, Stream inputContent)
        {
            string stringValue = inputContent.ReadString();

            stringValue = Regex.Replace(stringValue, "url\\((?<quotation>['\"]?)(?<capturedUrl>[^\\)]*)", x =>
                                                                              {
                                                                                  var url = x.Groups["capturedUrl"];
                                                                                  if (url != null)
                                                                                  {
                                                                                      var quotation = x.Groups["quotation"];
                                                                                      var quoteWrapper = quotation != null ? quotation.Value : "";
                                                                                      var virtualPath = url.Value;

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
                                                                                          virtualPath = RejuicerEngine.GetConfigFor(virtualPath).GetTimestampedUrl(_cacheProvider);

                                                                                          // Need to add a dependency of this CSS file to the now linked image.
                                                                                          source.AddDependency(RejuicerEngine.GetConfigFor(url.Value));
                                                                                      }

                                                                                      var urlValue = _virtualPathResolver.GetRelativeUrl(virtualPath);

                                                                                      return string.Format("url({1}{0}", urlValue, quoteWrapper);
                                                                                  }

                                                                                  return x.Value;
                                                                              });

            

            return stringValue.AsStream();
        }
    }
}
