using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rejuicer.Engine
{
    public static class StreamUtilities
    {

        public static byte[] CloneBytes(this byte[] input)
        {
            return (byte[])input.Clone();
        }

        public static Byte[] AsBytes(this string input)
        {
            return System.Text.Encoding.UTF8.GetBytes(input);
        }

        public static string ReadString(this byte[] input)
        {
            return Encoding.UTF8.GetString(input);
        }
    }
}
