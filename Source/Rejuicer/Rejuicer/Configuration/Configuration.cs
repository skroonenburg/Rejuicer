using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rejuicer.Configuration
{
    /// <summary>
    /// A strongly typed wrapper for accessing the XML configuration of grouper versions, filters and limits.
    /// </summary>
    public class RejuicerConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        public static RejuicerConfiguration Current
        {
            get
            {
                return (RejuicerConfiguration) ConfigurationManager.GetSection("rejuicer");
            }
        }

        [ConfigurationProperty("PreventPassThroughOnDebug", IsRequired = false)]
        public bool? PreventPassThroughOnDebug
        {
            get { return (bool?)this["PreventPassThroughOnDebug"]; }
        }

    }
}
