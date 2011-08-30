using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Rejuicer.Engine
{
    public class DefaultCssTransformer : IFileTransformer
    {
        private readonly IVirtualPathResolver _virtualPathResolver;

        public DefaultCssTransformer()
        {
            _virtualPathResolver = new VirtualPathResolver();
        }

        public DefaultCssTransformer(IVirtualPathResolver virtualPathResolver)
        {
            _virtualPathResolver = virtualPathResolver;
        }

        public string TransformFile(string inputContent)
        {
            inputContent = Regex.Replace(inputContent, "url\\((?<quotation>['\"]?)(?<capturedUrl>[^\\)]*)", x =>
                                                                              {
                                                                                  var url = x.Groups["capturedUrl"];
                                                                                  if (url != null)
                                                                                  {
                                                                                      var quotation = x.Groups["quotation"];
                                                                                      var quoteWrapper = quotation != null ? quotation.Value : "";
                                                                                      var urlValue = _virtualPathResolver.GetRelativeUrl(url.Value);

                                                                                      return string.Format("url({1}{0}", urlValue, quoteWrapper);
                                                                                  }

                                                                                  return x.Value;
                                                                              });
            return inputContent;
        }
    }
}
