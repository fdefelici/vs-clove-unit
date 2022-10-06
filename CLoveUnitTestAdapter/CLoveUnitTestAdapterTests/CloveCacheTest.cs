using CLoveUnitTestAdapter.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLoveUnitTestAdapterTests
{
    [TestClass]
    public class CloveCacheTest
    {

        private string _cachePath;

        [TestInitialize]
        public void Setup()
        {
            string binBasePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            _cachePath = Path.Combine(binBasePath, "mycache.txt");
        }

        [TestCleanup]
        public void TearDown()
        {
            File.Delete(_cachePath);
        }


        [TestMethod]
        public void CacheFileCreatedOnContruction()
        {
            CloveCache cache = new CloveCache(_cachePath);
            Assert.IsTrue(cache.Exists());
        }

        [TestMethod]
        public void WriteTwoProps()
        {
            CloveCache cache = new CloveCache(_cachePath);
            cache.WriteProp("mybool", true);
            cache.WriteProp("myint", 10);

            string expected = $"mybool=True{Environment.NewLine}" +
                              $"myint=10{Environment.NewLine}";
            string actual = File.ReadAllText(_cachePath);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadTwoExistentsProps()
        {
            string contents = $"mybool=True{Environment.NewLine}" +
                              $"myint=10{Environment.NewLine}";
            File.WriteAllText(_cachePath, contents);

            CloveCache cache = new CloveCache(_cachePath);
            Assert.AreEqual(true, cache.ReadProp("mybool", false));
            Assert.AreEqual(10, cache.ReadProp("myint", 0));
        }
    }
}
