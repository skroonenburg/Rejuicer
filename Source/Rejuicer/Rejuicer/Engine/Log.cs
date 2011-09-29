using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Rejuicer.Engine
{
    public static class Log
    {
        public static void WriteLine(string format, params object [] parameters)
        {
            Debug.WriteLine(string.Format(format, parameters), "Rejuicer");
        }
    }
}
