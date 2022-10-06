using System.IO;

namespace CLoveUnitTestAdapter.Core
{
    public class XBinaryFile
    {
        public static bool ContainsString(string binaryPath, string str)
        {
            string contents = File.ReadAllText(binaryPath);
            return contents.Contains(str);
        }

        public XBinaryFile(string filePath)
        {
            FilePath = filePath;
            FileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
        }

        public bool Exists()
        {
            return File.Exists(FilePath);
        }

        public bool Contains(string text)
        {
            return ContainsString(FilePath, text);
        }

        public string FilePath { get; internal set; }
        public string FileNameNoExt { get; internal set; }
    }
}
