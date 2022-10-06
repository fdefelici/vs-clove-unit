using System.IO;
using System.Text;

namespace CLoveUnitTestAdapter.Core
{
    public class CloveCache
    {
        public string CachePath { get; internal set; }

        private XProperties _properties;

        public CloveCache(string cacheFilePath)
        {
            CachePath = cacheFilePath;
            if (!Exists())
            {
                File.WriteAllText(CachePath, "");
                _properties = new XProperties();
            }
            else 
            { 
                _properties = XProperties.FromFile(cacheFilePath);
            }
        }

        public bool Exists()
        {
            return File.Exists(CachePath);
        }

        public void OverWrite(XProperties props)
        {
            _properties.Clear();
            _properties.SetAll(props);
            XProperties.ToFile(_properties, CachePath);
        }

        public bool ReadProp(string name, bool defaultValue)
        {
            return _properties.Get(name, defaultValue);
        }

        public int ReadProp(string name, int defaultValue)
        {
            return _properties.Get(name, defaultValue);
        }

        public void WriteProp(string name, bool value)
        {
            _properties.Set(name, value);
            XProperties.ToFile(_properties, CachePath);
        }

        public void WriteProp(string name, int value)
        {
            _properties.Set(name, value);
            XProperties.ToFile(_properties, CachePath);
        }

        public string AsString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine($"Cache Path: {CachePath}");
            buffer.AppendLine($"Cache Props: {_properties.Count}");
            return buffer.ToString();
        }
    }
}
