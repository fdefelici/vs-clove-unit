using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;


namespace CLoveUnitTestAdapter.Core.Utils
{
    public class XFilesystem
    {
        public static void MakeDir(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static void ForEachFileIn(string dirPath, Action<string> action)
        {
            IEnumerable<string> list = Directory.EnumerateFiles(dirPath);
            foreach(string file in list)
            {
                action(file);
            }
        }

        public static List<string> SelectFiles(string dirPath, Predicate<string> filter)
        {
            List<string> files = new List<string>();
            IEnumerable<string> list = Directory.EnumerateFiles(dirPath);
            foreach (string file in list)
            {
                if (filter(file)) files.Add(file);
            }
            return files;
        }

        public static string SelectFile(string dirPath, Predicate<string> filter, bool recursive = false)
        {
            IEnumerable<string> list = Directory.EnumerateFiles(dirPath, "*.*", recursive ?  SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (string file in list)
            {
                if (filter(file)) return file;
            }
            return null;
        }

        public static void MakeFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, "");
            } catch (Exception) {  }
        }

        public static string SelectFileBack(string basePath, Predicate<string> filter)
        {
            DirectoryInfo dir = new DirectoryInfo(basePath);

            IEnumerable<string> files = Directory.EnumerateFiles(dir.FullName, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                if (filter(file)) return file;
            }

            DirectoryInfo parentDir = dir.Parent;
            if (parentDir == null) return null;
            return SelectFileBack(parentDir.FullName, filter);
        }
    }
}
