using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer.Engine
{
    internal class OutputContent
    {
        public OutputContent()
        {
            LastModifiedDate = DateTime.Now.ToString();
        }

        public string Content { get; set; }
        public string ContentType { get; set; }
        public bool AllowClientCaching { get; set; }
        public string LastModifiedDate { get; set; }

        public OutputContent Configure(RejuicedFileModel model)
        {
            AllowClientCaching = model.ContainsPlaceHolder;

            return this;
        }
    }
}
