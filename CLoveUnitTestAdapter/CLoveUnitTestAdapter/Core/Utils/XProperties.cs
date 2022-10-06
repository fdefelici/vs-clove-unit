using System;
using System.Collections.Generic;
using System.IO;

namespace CLoveUnitTestAdapter.Core
{ 
    public class XProperties
    {
        private List<Tuple<string, string>> propsList;
        private Dictionary<string, string> propsMap;

        public int Count { get { return propsList.Count; } }

        public XProperties()
        {
            propsList = new List<Tuple<string, string>>();
            propsMap = new Dictionary<string, string>();
        }

        public void ForEach(Action<string, string> action)
        {
            propsList.ForEach((tuple) => action(tuple.Item1, tuple.Item2));
        }

        public void Set(string key, string value)
        {
            if (propsMap.ContainsKey(key))
            {
                propsMap[key] = value;
                
                for(int i = 0; i < propsList.Count; i++)
                {
                    Tuple<string, string> tuple = propsList[i];
                    if (tuple.Item1 == key)
                    {
                        propsList[i] = new Tuple<string, string>(key, value);
                        return;
                    }
                }


            } else
            {
                propsMap.Add(key, value);
                propsList.Add(new Tuple<string, string>(key, value));
            }

        }

        public void Set(string key, int value)
        {
            string valueStr = $"{value}";
            Set(key, valueStr);
        }

        public void Set(string key, bool value)
        {
            string valueStr = $"{value}";
            Set(key, valueStr);
        }

        public void SetAll(XProperties props)
        {
            props.ForEach(Set);
        }

        public bool Has(string key)
        {
            return propsMap.ContainsKey(key);
        }

        public string Get(string key, string defaultValue = null)
        {
            if (propsMap.TryGetValue(key, out string value)) return value;
            else return defaultValue;
        }

        public int Get(string key, int defaultValue)
        {
            if (!Has(key)) return defaultValue;
            string valueStr = Get(key);

            if (!int.TryParse(valueStr, out int value))
            {
                return defaultValue;
            }
            return value;
        }

        public bool Get(string key, bool defaultValue)
        {
            if (!Has(key)) return defaultValue;
            string valueStr = Get(key);

            if (!bool.TryParse(valueStr, out bool value))
            {
                return defaultValue;
            }
            return value;
        }

        public void Clear()
        {
            propsList.Clear();
            propsMap.Clear();
        }

        public static XProperties FromFile(string propFilePath)
        {
            XProperties props = new XProperties();
            if (!File.Exists(propFilePath)) return props;
            
            string[] lines = File.ReadAllLines(propFilePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                string[] prop = line.Split('=');
                if (prop.Length != 2) continue;
                props.Set(prop[0].Trim(), prop[1].Trim());
            }

            return props;
        }

        public static void ToFile(XProperties props, string filePath)
        {
            StreamWriter stream = new StreamWriter(filePath, false);
            props.ForEach((name, value) => {
                stream.WriteLine($"{name}={value}");
            });
            stream.Flush();
            stream.Close();
        }
    }

}
