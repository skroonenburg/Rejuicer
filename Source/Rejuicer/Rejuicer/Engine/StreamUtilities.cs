using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rejuicer.Engine
{
    public static class StreamUtilities
    {
        public static Stream Clone(this Stream input)
        {
            var output = new MemoryStream((int)input.Length);

            var buffer = new byte[1024];
            input.Seek(0, SeekOrigin.Begin);

            int read = 0;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }

            output.Flush();
            output.Seek(0, SeekOrigin.Begin);

            return output;
        }

        public static Stream AsStream(this string input)
        {
            var outputStream = new MemoryStream(input.Length * sizeof(char));

            var streamWriter = new StreamWriter(outputStream);
            streamWriter.Write(input);
            streamWriter.Flush();

            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }

        public static string ReadString(this Stream input)
        {
            return new StreamReader(input).ReadToEnd();
        }
    }
}
