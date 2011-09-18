using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Rejuicer.Engine
{
    public abstract class BaseStringMinificationProvider : IMinificationProvider
    {
        public abstract string MinifyStringValue(string data);

        public Stream Minify(Stream data)
        {
            // Read the data into a string
            var stringValue = data.ReadString();
            var culture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                var minifiedValue = MinifyStringValue(stringValue);

                return minifiedValue.AsStream();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        public Stream Combine(IEnumerable<Stream> data)
        {
            var combinedStream = new MemoryStream();

            var streamWriter = new StreamWriter(combinedStream);
            
            foreach (var value in data)
            {
                streamWriter.Write(value.ReadString());
                streamWriter.WriteLine();
            }

            streamWriter.Flush();

            return combinedStream;
        }


        public abstract string GetContentType(string filename);
    }
}
