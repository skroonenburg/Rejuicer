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

        public byte[] Minify(byte[] data)
        {
            // Read the data into a string
            var stringValue = data.ReadString();

            return MinifyStringValue(stringValue).AsBytes();
        }

        public byte[] Combine(IEnumerable<byte[]> data)
        {
            var sb = new StringBuilder();
            
            foreach (var value in data)
            {
                sb.Append(value.ReadString());
                sb.AppendLine();
            }

            return sb.ToString().AsBytes();
        }


        public abstract string GetContentType(string filename);
    }
}
