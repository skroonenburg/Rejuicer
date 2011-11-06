using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rejuicer;
using Rejuicer.Engine;
using Rejuicer.Engine.Transformers;
using Rejuicer.Model;

namespace Rejuicer_test
{
    [TestFixture]
    public class DefaultCssTransformerTest
    {
        private string TransformCss(string css, Func<string, string> replacement)
        {
            return new DefaultCssTransformer(new StubVirtualPathResolver(replacement)).TransformFile(new PhysicalFileSource(ResourceType.Css, "~/test.css", "test.css", Mode.Minify), css.AsBytes()).ReadString();
        }

        [Test]
        public void TransformFile_IncludingZeroUrls_NoChange()
        {
            var sourceCss = ".cssClass { display: none; }";
            var transformedCss = TransformCss(sourceCss, x => "TEST");

            Assert.AreEqual(transformedCss, sourceCss);
        }

        [Test]
        public void TransformFile_IncludingOneUrl_IsReplaced()
        {
            var sourceCss = ".cssClass { background-image: url(~/Content/MyImage.png); }";
            var expectedCss = ".cssClass { background-image: url(TEST-URL); }";
            var transformedCss = TransformCss(sourceCss, x => "TEST-URL");

            Assert.AreEqual(expectedCss, transformedCss);
        }

        [Test]
        public void TransformFile_IncludingOneUrl_IsReadAndReplaced()
        {
            var sourceCss = ".cssClass { background-image: url(~/Content/MyImage.png); }";
            var expectedCss = ".cssClass { background-image: url(~/CONTENT/MYIMAGE.PNG); }";
            var transformedCss = TransformCss(sourceCss, x => x.ToUpperInvariant());

            Assert.AreEqual(expectedCss, transformedCss);
        }

        [Test]
        public void TransformFile_IncludingTwoUrls_AreReadAndReplaced()
        {
            var sourceCss = @".cssClass { background-image: url(~/Content/MyImage.png); }
                              .cssClass2 { background-image: url(~/Content/OtherImage.png); }";
            var expectedCss = @".cssClass { background-image: url(~/CONTENT/MYIMAGE.PNG); }
                              .cssClass2 { background-image: url(~/CONTENT/OTHERIMAGE.PNG); }";

            var transformedCss = TransformCss(sourceCss, x => x.ToUpperInvariant());

            Assert.AreEqual(expectedCss, transformedCss);
        }

        private class StubVirtualPathResolver : IVirtualPathResolver
        {
            private readonly Func<string, string> _replacement;

            public StubVirtualPathResolver(Func<string, string> replacement)
            {
                _replacement = replacement;
            }

            public DirectoryInfo ResolveVirtualPathToDirectory(string virtualPath)
            {
                throw new NotImplementedException();
            }

            public FileInfo ResolveVirtualPathToFile(string virtualPath)
            {
                throw new NotImplementedException();
            }

            public string ResolveVirtualPath(string virtualPath)
            {
                throw new NotImplementedException();
            }

            public string GetRelativeUrl(string virtualPath)
            {
                return _replacement(virtualPath);
            }

            public string GetVirtualPathFor(FileInfo file)
            {
                throw new NotImplementedException();
            }
        }
    }
}
