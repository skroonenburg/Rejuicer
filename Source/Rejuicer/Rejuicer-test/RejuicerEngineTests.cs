using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rejuicer;

namespace Rejuicer_test
{
    [TestFixture]
    public class RejuicerEngineTests
    {
        [SetUp]
        public void Setup()
        {
            RejuicerEngine.ClearConfigurations();
        }

        [Test]
        public static void HasConfigurationFor_WithZeroConfigurations_NoMatch()
        {
            Assert.IsFalse(RejuicerEngine.HasConfigurationFor("~/TestFile.css"));
        }

        [Test]
        public static void HasConfigurationFor_WithOneConfigurationNotMatching_NoMatch()
        {
            OnRequest.ForCss("~/TestFile.css")
                        .Compact
                        .Configure();

            Assert.IsFalse(RejuicerEngine.HasConfigurationFor("~/OtherFile.css"));
        }

        [Test]
        public static void HasConfigurationFor_WithOneConfigurationMatching_Matches()
        {
            OnRequest.ForCss("~/TestFile.css")
                        .Compact
                        .Configure();

            Assert.IsTrue(RejuicerEngine.HasConfigurationFor("~/TestFile.css"));
        }

        [Test]
        public static void HasConfigurationFor_WithNoPlaceholderConfigurations_DoesNotMatchPlaceholder()
        {
            OnRequest.ForCss("~/TestFile.css")
                        .Compact
                        .Configure();

            Assert.IsFalse(RejuicerEngine.HasConfigurationFor("~/TestFile{0}.css"));
        }

        [Test]
        public static void HasConfigurationFor_WithOnePlaceholderConfigurationNotMatching_DoesNotMatchPlaceholder()
        {
            OnRequest.ForCss("~/TestFile-{0}.css")
                        .Compact
                        .Configure();

            Assert.IsFalse(RejuicerEngine.HasConfigurationFor("~/OtherFile-{0}.css"));
        }

        [Test]
        public static void HasConfigurationFor_WithOnePlaceholderConfigurationMatching_Matches()
        {
            OnRequest.ForCss("~/TestFile-{0}.css")
                        .Compact
                        .Configure();

            Assert.IsTrue(RejuicerEngine.HasConfigurationFor("~/TestFile-{0}.css"));
        }

        [Test]
        public static void HasConfigurationFor_WithOneLambdaPlaceholderConfigurationMatching_Matches()
        {
            OnRequest.ForCss(x => "~/TestFile-" + x + ".css")
                        .Compact
                        .Configure();

            Assert.IsTrue(RejuicerEngine.HasConfigurationFor("~/TestFile-{0}.css"));
        }

        [Test]
        public static void HasConfigurationFor_WithOneLambdaPlaceholderConfigurationNotMatching_DoesNotMatch()
        {
            OnRequest.ForCss(x => "~/TestFile-" + x + ".css")
                        .Compact
                        .Configure();

            Assert.IsFalse(RejuicerEngine.HasConfigurationFor("~/OtherFile-{0}.css"));
        }

        [Test]
        public static void HasConfigurationFor_WithTwoPlaceholderConfigurationMatching_Matches()
        {
            OnRequest.ForCss("~/File1-{0}.css")
                        .Compact
                        .Configure();

            OnRequest.ForCss("~/File2-{0}.css")
                        .Compact
                        .Configure();

            Assert.IsTrue(RejuicerEngine.HasConfigurationFor("~/File2-{0}.css"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public static void Configure_FileWithTwoPlaceHolders_ThrowsException()
        {
            OnRequest.ForCss("~/File-{0}-{0}.css")
                .Compact.Configure();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public static void Configure_FileWithTwoLambdaPlaceHolders_ThrowsException()
        {
            OnRequest.ForCss(x => "~/File-" + x + "-" + x + ".css")
                .Compact.Configure();
        }
    }
}
