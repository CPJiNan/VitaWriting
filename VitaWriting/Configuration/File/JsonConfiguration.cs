using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace VitaWriting.Configuration
{
    public class JsonConfiguration : IConfiguration
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        private string _name;
        private string _currentPath;

        public JsonConfiguration(string name, string currentPath)
        {
            _name = name;
            _currentPath = currentPath;
        }

        public object Get(string path)
        {
            return Get(path, null);
        }

        public object Get(string path, object def)
        {
            if (string.IsNullOrWhiteSpace(path))
                return def;

            var keys = path.Split('.');
            var current = _values as IDictionary<string, object>;

            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];

                if (current == null || !current.ContainsKey(key))
                    return def;

                if (i == keys.Length - 1) return current[key];

                current = current[key] as IDictionary<string, object>;
            }

            return def;
        }

        public void Set(string path, object value)
        {
            var keys = path.Split('.');
            var current = _values;

            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];

                if (i == keys.Length - 1)
                {
                    if (current.ContainsKey(key))
                        current[key] = value;
                    else
                        current.Add(key, value);
                }
                else
                {
                    if (!current.ContainsKey(key) || !(current[key] is Dictionary<string, object> nested))
                    {
                        nested = new Dictionary<string, object>();
                        current[key] = nested;
                    }

                    current = nested;
                }
            }
        }

        public HashSet<string> GetKeys(bool deep)
        {
            return new HashSet<string>(_values.Keys);
        }

        public Dictionary<string, object> GetValues(bool deep)
        {
            return new Dictionary<string, object>(_values);
        }

        public bool Contains(string path)
        {
            return _values.ContainsKey(path);
        }

        public bool IsSet(string path)
        {
            return _values.ContainsKey(path) && _values[path] != null;
        }

        public string GetCurrentPath()
        {
            return _currentPath;
        }

        public string GetName()
        {
            return _name;
        }

        public IConfiguration GetRoot()
        {
            return this;
        }

        public string GetString(string path)
        {
            return Get(path) as string;
        }

        public string GetString(string path, string def)
        {
            return Get(path) as string ?? def;
        }

        public bool IsString(string path)
        {
            return Get(path) is string;
        }

        public int GetInt(string path)
        {
            return Convert.ToInt32(Get(path));
        }

        public int GetInt(string path, int def)
        {
            return Get(path) is int i ? i : def;
        }

        public bool IsInt(string path)
        {
            return Get(path) is int;
        }

        public bool GetBoolean(string path)
        {
            return Convert.ToBoolean(Get(path));
        }

        public bool GetBoolean(string path, bool def)
        {
            return Get(path) is bool b ? b : def;
        }

        public bool IsBoolean(string path)
        {
            return Get(path) is bool;
        }

        public double GetDouble(string path)
        {
            return Convert.ToDouble(Get(path));
        }

        public double GetDouble(string path, double def)
        {
            return Get(path) is double d ? d : def;
        }

        public bool IsDouble(string path)
        {
            return Get(path) is double;
        }

        public long GetLong(string path)
        {
            return Convert.ToInt64(Get(path));
        }

        public long GetLong(string path, long def)
        {
            return Get(path) is long l ? l : def;
        }

        public bool IsLong(string path)
        {
            return Get(path) is long;
        }

        public List<T> GetList<T>(string path)
        {
            return Get(path) as List<T> ?? new List<T>();
        }

        public List<T> GetList<T>(string path, List<T> def)
        {
            return Get(path) as List<T> ?? def;
        }

        public bool IsList(string path)
        {
            return Get(path) is IList && Get(path).GetType().IsGenericType;
        }

        public List<string> GetStringList(string path)
        {
            return Get(path) as List<string> ?? new List<string>();
        }

        public List<int> GetIntegerList(string path)
        {
            return Get(path) as List<int> ?? new List<int>();
        }

        public List<bool> GetBooleanList(string path)
        {
            return Get(path) as List<bool> ?? new List<bool>();
        }

        public List<double> GetDoubleList(string path)
        {
            return Get(path) as List<double> ?? new List<double>();
        }

        public List<float> GetFloatList(string path)
        {
            return Get(path) as List<float> ?? new List<float>();
        }

        public List<long> GetLongList(string path)
        {
            return Get(path) as List<long> ?? new List<long>();
        }

        public List<byte> GetByteList(string path)
        {
            return Get(path) as List<byte> ?? new List<byte>();
        }

        public List<char> GetCharacterList(string path)
        {
            return Get(path) as List<char> ?? new List<char>();
        }

        public List<short> GetShortList(string path)
        {
            return Get(path) as List<short> ?? new List<short>();
        }

        public List<Dictionary<string, object>> GetMapList(string path)
        {
            return Get(path) as List<Dictionary<string, object>> ?? new List<Dictionary<string, object>>();
        }

        public static JsonConfiguration LoadConfiguration(FileInfo file)
        {
            if (file == null || !file.Exists) throw new FileNotFoundException("File does not exist.", file?.FullName);

            var config = new JsonConfiguration(file.Name, file.FullName);

            var jsonContent = File.ReadAllText(file.FullName);

            if (string.IsNullOrWhiteSpace(jsonContent)) jsonContent = "{}";

            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);

            foreach (var kvp in deserializedData) config.Set(kvp.Key, kvp.Value);

            return config;
        }

        public void SaveToFile(FileInfo file)
        {
            if (file == null) throw new FileNotFoundException("File does not exist.", file?.FullName);

            var jsonContent = JsonConvert.SerializeObject(_values, Formatting.Indented);

            File.WriteAllText(file.FullName, jsonContent);
        }
    }
}