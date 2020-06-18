using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Com.Bing
{
    public class DictionaryWithDefault<X, Y> : Dictionary<X, Y>
    {
        Y defaultValue;
        public DictionaryWithDefault(Y defaultValue)
        {
            this.defaultValue = defaultValue;
        }
        public new Y this[X key]
        {
            get
            {
                Y value;
                if (base.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    return defaultValue;
                }
            }
            set
            {
                base[key] = value;
            }
        }
        public void LoadDictKeyValue(DataRow row)
        {
            if (row.Table.Columns.Contains("dict"))
            {
                Dictionary<X, Y> dic = row["dict"] as Dictionary<X, Y>;
                this.Clear();
                //½¡×³ÐÔ
                if (dic == null) { return; }
                foreach (X key in dic.Keys)
                {
                    this[key] = dic[key];
                }
            }
        }
    }
    public class StringDictionaryWithDefault
    {
        
        private Dictionary<string, string> dict;
        public StringDictionaryWithDefault(Dictionary<string, string> dict)
        {
            this.dict = dict;
        }

        public string this[string key]
        {
            get
            {
                string value;
                dict.TryGetValue(key, out value);
                return value ?? string.Empty;
            }
            set
            {
                dict[key] = value;
            }
        }

        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public DataTable DataTable
        {
            get
            {
                DataTable table = new DataTable();
                table.Columns.Add("Key");
                table.Columns.Add("Value");
                foreach(KeyValuePair<string ,string> pair in dict)
                {
                    table.Rows.Add(new object[] { pair.Key, pair.Value });
                }
                return table;
            }
        }
    }
}
