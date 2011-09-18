using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rejuicer;
using Rejuicer.Model;

namespace Rejuicer_test
{
    public class FluentConfigurerTests
    {
        [SetUp]
        public void Setup()
        {
            RejuicerEngine._configurations.Clear();
        }

        private RejuicerConfigurationSource GetModelFor(object configurer)
        {
            return ((CompactorConfigurer)configurer)._config;
        }

        [Test]
        public void FluentConfiguration_ConfigureCssResourceType_SetsConfigCorrectly()
        {
            var config = GetModelFor(OnRequest.ForCss("~/Css/Combined-Test.css"));

            Assert.AreEqual(ResourceType.Css, config.ResourceType);
        }

        [Test]
        public void FluentConfiguration_ConfigureJsResourceType_SetsConfigCorrectly()
        {
            var config = GetModelFor(OnRequest.ForJs("~/Scripts/Combined-Test.js"));

            Assert.AreEqual(ResourceType.Js, config.ResourceType);
        }

        [Test]
        public void FluentConfiguration_ConfigureCombine_ConfigModeIsCombine()
        {
            var config = GetModelFor(OnRequest.ForJs("~/Scripts/Combined-Test.js").Combine);

            Assert.AreEqual(Mode.Combine, config.Mode);
        }

        [Test]
        public void FluentConfiguration_ConfigureCompact_ConfigModeIsCompact()
        {
            var config = GetModelFor(OnRequest.ForJs("~/Scripts/Combined-Test.js").Compact);

            Assert.AreEqual(Mode.Minify, config.Mode);
        }

        [Test]
        public void FluentConfiguration_File_AddsFileToConfig()
        {
            var config = GetModelFor(OnRequest.ForJs("~/Scripts/Combined-Test.js").Combine
                    .File("~/Scripts/myfile.js"));

            Assert.AreEqual(1, config.Count);
            Assert.AreEqual("~/Scripts/myfile.js", ((PhysicalFileSource)config[0]).VirtualPath);
        }

        [Test]
        public void FluentConfiguration_Configure_AddsRejuicerModelToConfiguration()
        {
            OnRequest.ForJs("~/Scripts/Combined-Test.js").Combine
                    .FilesIn("~/Scripts/").Matching("*.js").Configure();

            Assert.AreEqual(1, RejuicerEngine._configurations.Count);
        }
    }
}
