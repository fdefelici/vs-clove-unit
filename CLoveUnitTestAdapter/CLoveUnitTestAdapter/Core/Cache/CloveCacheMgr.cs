using CLoveUnitTestAdapter.Core.Utils;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CLoveUnitTestAdapter.Core
{
    public class CloveCacheMgr
    {
        public string CacheRootPath { get; internal set; }

        private CloveCacheMgr(string dirPath)
        {
            CacheRootPath = dirPath;
            XFilesystem.MakeDir(CacheRootPath);

            _caches = new Dictionary<string, CloveCache>();

            List<string> files =
               XFilesystem.SelectFiles(dirPath, (filePath) => filePath.EndsWith(CloveConfig.CacheFileNameSuffix));
            files.ForEach((file) => {
                string binName = Path.GetFileNameWithoutExtension(file);
                _caches.Add(binName, new CloveCache(file));
            });

        }

        public static CloveCacheMgr Setup(string dirPath)
        {
            return new CloveCacheMgr(dirPath);
        }

        public bool HasCache(string cacheId)
        {
            return _caches.ContainsKey(cacheId);
        }

        public CloveCache CreateCache(string cacheId)
        {
            string fileName = $"{cacheId}{CloveConfig.CacheFileNameSuffix}";
            string filePath = Path.Combine(CacheRootPath, fileName);
            CloveCache cache = new CloveCache(filePath);
            _caches.Add(cacheId, cache);
            return cache;
        }

        public CloveCache LoadCache(string cacheId)
        {
            if (_caches.TryGetValue(cacheId, out CloveCache value)) return value;
            return null;
        }

        public string AsString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine($"Cache Base Path: {CacheRootPath}");
            buffer.AppendLine($"Entries: {_caches.Count}");
            foreach(var cache in _caches)
            {
                buffer.AppendLine(cache.Value.AsString());
            }

            return buffer.ToString();
        }

        private Dictionary<string, CloveCache> _caches;
    }
}