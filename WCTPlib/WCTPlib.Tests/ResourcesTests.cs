using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WCTPlib.Tests
{
    [TestClass]
    public class ResourcesTests
    {
        [TestMethod]
        public void ResourceExists_Existant()
        {
            Assert.IsTrue(Resources.ResourceExists("wctpv1-0.dtd"));
        }

        [TestMethod]
        public void ResourceExists_NonExistant()
        {
            Assert.IsFalse(Resources.ResourceExists("invalid.file"));
        }

        [TestMethod]
        public void GetResourceStream_Existant()
        {
            Assert.IsNotNull(Resources.GetResourceStream("wctpv1-0.dtd"));
        }

        [TestMethod]
        public void GetResourceStream_NonExistant()
        {
            try
            {
                Resources.GetResourceStream("invalid.file");
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
        public void TryGetResourceStream_Existant()
        {
            Stream stream;
            Assert.IsTrue(Resources.TryGetResourceStream("wctpv1-0.dtd", out stream));
            Assert.IsNotNull(stream);
        }

        [TestMethod]
        public void TryGetResourceStream_NonExistant()
        {
            Stream stream;
            Assert.IsFalse(Resources.TryGetResourceStream("invalid.file", out stream));
            Assert.IsNull(stream);
        }
    }
}
