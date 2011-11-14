using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Rejuicer.Engine
{
    public static class HashUtilities
    {
        public static byte[] HashArray(this byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(bytes);
            }  
        }

        public static string HashStringValue(this byte[] bytes, bool uppercase = false)
        {
            return bytes.HashArray().ToDecimalString(uppercase);
        }

        public static string ToDecimalString(this byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "D2" : "d2"));

            return result.ToString();
        }
    }
}
