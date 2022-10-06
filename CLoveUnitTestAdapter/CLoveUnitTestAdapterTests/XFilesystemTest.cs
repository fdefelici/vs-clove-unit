using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLoveUnitTestAdapter.Core;
using CLoveUnitTestAdapter.Core.Utils;
using System.IO;

namespace CLoveUnitTestAdapterTests
{
    [TestClass]
    public class XFilesystemTest
    {
        [TestMethod]
        public void SelectFileBackWithFileFound()
        {
            string rootPath = TestUtils.AbsPath("xfilesystem");
            TestUtils.MakeDir(rootPath);
            TestUtils.MakeFile(rootPath, "level1", "level2", "level3", "file31.txt");
            TestUtils.MakeFile(rootPath, "level1", "level2", "level3", "file32.txt");
            TestUtils.MakeFile(rootPath, "level1", "level2", "file21.txt");
            TestUtils.MakeFile(rootPath, "level1", "level2", "file22.txt");
            TestUtils.MakeFile(rootPath, "level1", "file11.txt");
            TestUtils.MakeFile(rootPath, "level1", "file12.txt");
            TestUtils.MakeFile(rootPath, "mysol.sln");
            TestUtils.MakeFile(rootPath, "file01.txt");

            string solPath = XFilesystem.SelectFileBack(rootPath, file => Path.GetExtension(file) == ".sln");

            string expectedSlnFile = Path.Combine(rootPath, "mysol.sln");
            Assert.AreEqual(expectedSlnFile, solPath);  

            TestUtils.DeleteDir(rootPath);
        }

        [TestMethod]
        public void SelectFileBackWithFileNotFound()
        {
            string rootPath = TestUtils.AbsPath("xfilesystem");
            TestUtils.MakeDir(rootPath);
            TestUtils.MakeFile(rootPath, "level1", "level2", "level3", "file31.txt");
            TestUtils.MakeFile(rootPath, "level1", "level2", "level3", "file32.txt");
            TestUtils.MakeFile(rootPath, "level1", "level2", "file21.txt");
            TestUtils.MakeFile(rootPath, "level1", "level2", "file22.txt");
            TestUtils.MakeFile(rootPath, "level1", "file11.txt");
            TestUtils.MakeFile(rootPath, "level1", "file12.txt");
            TestUtils.MakeFile(rootPath, "mysol.sln");
            TestUtils.MakeFile(rootPath, "file01.txt");

            string solPath = XFilesystem.SelectFileBack(rootPath, file => Path.GetExtension(file) == ".UNEXISTENT");
            Assert.IsNull(solPath);

            TestUtils.DeleteDir(rootPath);
        }
    }
}
