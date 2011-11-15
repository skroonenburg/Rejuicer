using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Rejuicer.Engine.MinificationProviders
{
    public class JsMinificationProvider : BaseStringMinificationProvider
    {
        public override string MinifyStringValue(string data)
        {
            try
            {

                if (string.IsNullOrEmpty(data))
                {
                    return "";
                }
                
                // Uses the same default values as Yahoo YUI Compressor
                return Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(data, true, true, false, false, -1, Encoding.UTF8, CultureInfo.InvariantCulture);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("Encountered exception trying minify invalid JavaScript.", e);
            }
            
        }

        public override string GetContentType(string filename)
        {
            return "text/javascript";
        }
    }
}
