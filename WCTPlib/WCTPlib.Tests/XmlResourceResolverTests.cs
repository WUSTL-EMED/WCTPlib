using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WCTPlib.Tests
{
    [TestClass]
    public class XmlResourceResolverTests
    {
        [TestMethod]
        public void ExplicitDTD_v1r0_Existant()
        {
            var resolver = new XmlResourceResolver("wctpv1-0.dtd");
            Assert.IsNotNull(resolver);

            var stream = resolver.GetEntity(null, null, null) as Stream;
            Assert.IsNotNull(stream);

            using (var reader = new StreamReader(stream))
            {
                var dtd = reader.ReadToEnd();
                Assert.IsTrue(dtd.Contains("http://wctp.arch.com/DTD/wctpv1-0.dtd"));
            }
        }

        [TestMethod]
        public void ExplicitDTD_v1r1_Existant()
        {
            var resolver = new XmlResourceResolver("wctp-dtd-v1r1.dtd");
            Assert.IsNotNull(resolver);

            var stream = resolver.GetEntity(null, null, null) as Stream;
            Assert.IsNotNull(stream);

            using (var reader = new StreamReader(stream))
            {
                var dtd = reader.ReadToEnd();
                Assert.IsTrue(dtd.Contains("http://dtd.wctp.org/wctp-dtd-v1r1.dtd"));
            }
        }

        [TestMethod]
        public void ExplicitDTD_v1r2_Existant()
        {
            var resolver = new XmlResourceResolver("wctp-dtd-v1r2.dtd");
            Assert.IsNotNull(resolver);

            var stream = resolver.GetEntity(null, null, null) as Stream;
            Assert.IsNotNull(stream);

            using (var reader = new StreamReader(stream))
            {
                var dtd = reader.ReadToEnd();
                Assert.IsTrue(dtd.Contains("http://www.wctp.org/wctp-dtd-v1r2.dtd"));
            }
        }

        [TestMethod]
        public void ExplicitDTD_v1r3_Existant()
        {
            var resolver = new XmlResourceResolver("wctp-dtd-v1r3.dtd");
            Assert.IsNotNull(resolver);

            var stream = resolver.GetEntity(null, null, null) as Stream;
            Assert.IsNotNull(stream);

            using (var reader = new StreamReader(stream))
            {
                var dtd = reader.ReadToEnd();
                Assert.IsTrue(dtd.Contains("http://dtd.wctp.org/wctp-dtd-v1r3.dtd"));
            }
        }

        [TestMethod]
        public void ExplicitDTD_NonExistant()
        {
            var resolver = new XmlResourceResolver("invalid.file");
            Assert.IsNotNull(resolver);

            try
            {
                var stream = resolver.GetEntity(null, null, null) as Stream;
                Assert.Fail("Expected exception not thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.ParamName, "resource");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [TestMethod]
        public void ParsedUriDTD_Existant()
        {
            var resolver = new XmlResourceResolver();
            Assert.IsNotNull(resolver);

            var stream = resolver.GetEntity(new Uri("http://example.com/wctpv1-0.dtd"), null, null) as Stream;
            Assert.IsNotNull(stream);

            using (var reader = new StreamReader(stream))
            {
                var dtd = reader.ReadToEnd();
                Assert.IsTrue(dtd.Contains("http://wctp.arch.com/DTD/wctpv1-0.dtd"));
            }
        }

        [TestMethod]
        public void ParsedUriDTD_NonExistant()
        {
            var resolver = new XmlResourceResolver();
            Assert.IsNotNull(resolver);

            var stream = resolver.GetEntity(new Uri("http://example.com/invalid.file"), null, null) as Stream;
            Assert.IsNull(stream);
        }

        [TestMethod]
        public void ParsedUriDTD_NoFile()
        {
            var resolver = new XmlResourceResolver();
            Assert.IsNotNull(resolver);

            var stream = resolver.GetEntity(new Uri("http://example.com/"), null, null) as Stream;
            Assert.IsNull(stream);
        }
    }
}
