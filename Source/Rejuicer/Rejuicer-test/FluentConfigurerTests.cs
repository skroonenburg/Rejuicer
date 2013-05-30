using System.IO;
using System.Web;
using NUnit.Framework;
using Rejuicer;
using Rejuicer.Engine;
using Rejuicer.Model;
using Rejuicer.Rules;

namespace Rejuicer_test
{
    [TestFixture]
    public class FluentConfigurerTests
    {
        [SetUp]
        public void Setup()
        {
            RejuicerEngine._configurations.Clear();
            
            WildcardMatchFileRule.VirtualPathResolver = new StubVirtualPathResolver();
            SingleFileRule.VirtualPathResolver = new StubVirtualPathResolver();
            PhysicalFileRegister.VirtualPathResolver = new StubVirtualPathResolver();
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
                );
        }

        private static RejuicerConfigurationSource GetModelFor(object configurer)
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
            var configurator = OnRequest.ForJs("~/Scripts/Combined-Test.js")
                                        .Combine
                                        .File("~/Scripts/one.js");
            configurator.Configure();
            var config = GetModelFor(configurator);

            Assert.AreEqual(1, config.Count);
            Assert.AreEqual("~/Scripts/one.js", ((PhysicalFileSource)config[0]).VirtualPath);
        }

        [Test]
        public void FluentConfiguration_Configure_AddsRejuicerModelToConfiguration()
        {
            OnRequest.ForJs("~/Scripts/Combined-Test.js")
                     .Combine
                     .FilesIn("~/Scripts/")
                     .Matching("*.js")
                     .Configure();

            Assert.AreEqual(1, RejuicerEngine._configurations.Count);
        }

        [Test]
        public void FluentConfiguration_File_IncludesWildCardFiles()
        {
            var configurator = OnRequest.ForJs("~/Scripts/Combined-Test.js")
                                        .Combine
                                        .FilesIn("~/Scripts/")
                                        .Matching("*.js");

            configurator.Configure();
            var model = GetModelFor(configurator);

            Assert.AreEqual(1, RejuicerEngine._configurations.Count);
            Assert.AreEqual(3, model.Count);
            Assert.AreEqual("~/Scripts/one.js", ((PhysicalFileSource)model[0]).VirtualPath);
            Assert.AreEqual("~/Scripts/three.js", ((PhysicalFileSource)model[1]).VirtualPath);
            Assert.AreEqual("~/Scripts/two.js", ((PhysicalFileSource)model[2]).VirtualPath);
        }

        [Test]
        public void FluentConfiguration_File_ExcludesSingleFileFromConfig()
        {
            var configurator = OnRequest.ForJs("~/Scripts/Combined-Test.js")
                     .Combine
                     .FilesIn("~/Scripts/")
                     .Excluding("one.js")
                     .Matching("*.js");

            configurator.Configure();
            var model = GetModelFor(configurator);

            Assert.AreEqual(1, RejuicerEngine._configurations.Count);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("~/Scripts/three.js", ((PhysicalFileSource)model[0]).VirtualPath);
            Assert.AreEqual("~/Scripts/two.js", ((PhysicalFileSource)model[1]).VirtualPath);
        }

        [Test]
        public void FluentConfiguration_File_ExcludesMultipleFilesFromConfig()
        {
            var configurator = OnRequest.ForJs("~/Scripts/Combined-Test.js")
                     .Combine
                     .FilesIn("~/Scripts/")
                     .Excluding("one.js")
                     .Excluding("two.js")
                     .Matching("*.js");

            configurator.Configure();
            var model = GetModelFor(configurator);

            Assert.AreEqual(1, RejuicerEngine._configurations.Count);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("~/Scripts/three.js", ((PhysicalFileSource)model[0]).VirtualPath);
        }

        public class StubVirtualPathResolver : IVirtualPathResolver
        {
            public DirectoryInfo ResolveVirtualPathToDirectory(string virtualPath)
            {
                return new DirectoryInfo("./../../Scripts");
            }

            public FileInfo ResolveVirtualPathToFile(string virtualPath)
            {
                if(virtualPath.EndsWith("one.js"))
                    return new FileInfo("./../../Scripts/one.js");

                return null;
            }

            public string ResolveVirtualPath(string virtualPath)
            {
                return null;
            }

            public string GetRelativeUrl(string virtualPath)
            {
                return null;
            }

            public string GetVirtualPathFor(FileInfo file)
            {
                if(file.Name.Equals("one.js") || file.Name.Equals("two.js")|| file.Name.Equals("three.js"))
                    return "~/Scripts/" + file.Name;
                return null;
            }
        }
    }
}
