using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Rejuicer.Engine
{
    public class ImageMinificationProvider : IMinificationProvider
    {
        public Stream Minify(Stream data)
        {
            // Perform no minification on image data
            return data;
        }

        public Stream Combine(IEnumerable<Stream> data)
        {
            var output = new MemoryStream((int)data.Sum(x => x.Length));

            var buffer = new byte[1024];
            foreach (var value in data)
            {
                value.Seek(0, SeekOrigin.Begin);

                int read = 0;
                while ((read = value.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                }

                output.Flush();
            }

            output.Seek(0, SeekOrigin.Begin);

            return output;
        }

        public string GetContentType(string filename)
        {
            switch (Path.GetExtension(filename).ToUpperInvariant())
            {
                case "PNG":
                    return "image/png";

                default:
                    return "image";
            }
        }
    }
}
