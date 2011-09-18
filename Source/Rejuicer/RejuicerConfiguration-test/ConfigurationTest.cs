using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rejuicer.Configuration;
using System.Configuration;
using Rejuicer;

namespace Rejuicer_test
{
    public class ConfigurationTest
    {
        [Test]
        public void ApplicationConfiguration_WithCurrentAppConfig_PreventPassThroughOnDebugIsOn()
        {
            Assert.IsTrue(RejuicerConfiguration.Current.PreventPassThroughOnDebug.HasValue && RejuicerConfiguration.Current.PreventPassThroughOnDebug.Value);
        }
    }
}
