using CLoveUnitTestAdapter.Core;
using CLoveUnitTestAdapter.Core.Utils;
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
    public class CloveCacheMgrTest
    {

        private string _cachePath;

        [TestInitialize]
        public void Setup()
        {
            string binBasePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            _cachePath = Path.Combine(binBasePath, "cachedir");
        }

        [TestCleanup]
        public void TearDown()
        {   
            Directory.Delete(_cachePath, true);
        }


        [TestMethod]
        public void CachePathCreatedOnContruction()
        {
            Assert.IsFalse(Directory.Exists(_cachePath));

            _ = CloveCacheMgr.Setup(_cachePath);

            Assert.IsTrue(Directory.Exists(_cachePath));
        }

        [TestMethod]
        public void CreateOneCache()
        {
            CloveCacheMgr cacheMgr =  CloveCacheMgr.Setup(_cachePath);
            Assert.IsFalse(cacheMgr.HasCache("cache1"));

            _ = cacheMgr.CreateCache("cache1");

            Assert.IsTrue(cacheMgr.HasCache("cache1"));
            Assert.IsTrue(File.Exists(Path.Combine(_cachePath, "cache1.cloveunit")));
        }
    }
}
