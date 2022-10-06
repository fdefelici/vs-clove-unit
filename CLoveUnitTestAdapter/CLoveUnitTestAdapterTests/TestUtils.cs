using System;
using System.IO;

namespace CLoveUnitTestAdapterTests
{
    public static class TestUtils
    {
        public static string BinBasePath => AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public static string AbsPath(params string[] parts)
        {
            return PathCombine(BinBasePath, parts);
        }

        public static string PathCombine(string first, params string[] parts)
        {
            string result = first;

            foreach (var part in parts)
            {
                result = Path.Combine(result, part);
            }
            return result;
        }

        public static string MakeDir(string dirPath, params string[] parts)
        {   
            string path = PathCombine(dirPath, parts);
            return Directory.CreateDirectory(path).FullName;
        }

        public static void WriteFile(string filePath, string contents)
        {
            string basePath = Path.GetDirectoryName(filePath);
            MakeDir(basePath);
            File.WriteAllText(filePath, contents);
        }

        public static void DeleteDir(string basePath)
        {
            Directory.Delete(basePath, true); 
        }

        public static void MakeFile(string dirPath, params string[] parts)
        {
            string filePath = PathCombine(dirPath, parts);
            
            string basePath = Path.GetDirectoryName(filePath);
            MakeDir(basePath);
            WriteFile(filePath, "");
        }
    }


}
