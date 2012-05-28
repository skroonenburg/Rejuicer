using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rejuicer.Model;

namespace Rejuicer.Engine
{
    public class OutputContent
    {
        public OutputContent()
        {
            LastModifiedDate = DateTime.Now;
        }

        public byte[] Content { get; set; }
        public byte[] ContentHash { get; set; }
        public string ContentType { get; set; }
        public bool AllowClientCaching { get; set; }
        public TimeSpan? CacheFor { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
