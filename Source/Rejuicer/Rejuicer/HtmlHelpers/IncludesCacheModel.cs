using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Rejuicer.Model;

namespace Rejuicer.HtmlHelpers
{
    public class IncludesCacheModel
    {
        public MvcHtmlString IncludesHtml { get; set; }
        public string HashValue { get; set; }

        public MvcHtmlString RenderHtml()
        {
            return MvcHtmlString.Create(IncludesHtml.ToString().Replace(RejuicerConfigurationSource.FilenameUniquePlaceholder, HashValue));
        }
    }
}
