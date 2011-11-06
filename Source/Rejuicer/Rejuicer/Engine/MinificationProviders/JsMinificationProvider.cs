using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Rejuicer.Engine
{
    public class JsMinificationProvider : BaseStringMinificationProvider
    {
        public override string MinifyStringValue(string data)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                if (string.IsNullOrEmpty(data))
                {
                    return "";
                }

                return Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(data);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("Encountered exception trying minify invalid JavaScript.", e);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            
        }

        public override string GetContentType(string filename)
        {
            return "text/javascript";
        }
    }
}
