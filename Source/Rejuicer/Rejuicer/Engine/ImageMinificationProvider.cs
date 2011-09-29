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
        public byte[] Minify(byte[] data)
        {
            // Perform no minification on image data
            return data;
        }

        public byte[] Combine(IEnumerable<byte[]> data)
        {
            return data.FirstOrDefault();
        }

        public string GetContentType(string filename)
        {
            switch (Path.GetExtension(filename).ToUpperInvariant())
            {
                case "PNG":
                    return "image/png";
                case "GIF":
                    return "image/gif";
                case "JPG":
                case "JPEG":
                    return "image/jpg";
                default:
                    return "image";
            }
        }
    }
}
