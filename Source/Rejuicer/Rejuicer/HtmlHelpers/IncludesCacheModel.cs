using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Rejuicer.Model;

namespace Rejuicer.HtmlHelpers
{
    public class IncludesCacheModel
    {
        public string IncludesHtml { get; set; }
        public DateTime Timestamp { get; set; }

        public string RenderHtml()
        {
            return IncludesHtml.Replace(RejuicerConfigurationSource.FilenameUniquePlaceholder, RejuicerConfigurationSource.GetTimeStampString(Timestamp));
        }
    }
}
