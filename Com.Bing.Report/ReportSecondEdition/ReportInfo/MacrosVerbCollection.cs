using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using KeyType = System.Collections.Generic.KeyValuePair<string, int>;
using System.Drawing;
namespace Com.Bing.Report
{
    public class MacrosVerbCollection : Dictionary<KeyType,string>
	{   
        /// <summary>
        /// 初始化宏变理所在页
        /// </summary>
        /// <param name="key">
        /// 形如：A(@@)B
        /// </param>
        /// <param name="pageNum">
        /// 宏变量在数据页中的分页数
        /// </param>
        /// <param name="val">
        /// 宏变量值
        /// </param>
        public void Add(int pageNum, string key , string val)
        {    
            if (this.ContainsKey(key) == 0)
            {
                pageNum = 1;
            }
            this[new KeyType(key, pageNum)] = val;
        }
        public void Add(string key, string val)
        {
            this[new KeyType(key, 1)] = val;
        }
		public int ContainsKey(string macrosVerbName)
		{
			int index = 0;
			foreach ( KeyValuePair<string, int> item in this.Keys )
			{
				if ( item.Key == macrosVerbName )
					index++;
			}
			return index;
		}
        //以页码和变量名查找宏变量时，总是往前找
		public string this[string macrosVerbName, int pageNum]
		{
			get
			{
				int index = this.ContainsKey( macrosVerbName );
                //是否包含这个宏变量
				if ( index >= 0 )
				{
                    if (pageNum == 0)
                    {
                        pageNum = 1;
                    }
                    //找宏变量时，总是往前靠2011/3/4
                    for (int i = pageNum; i >=0; i-- )
                    {
                        foreach (KeyType key in this.Keys)
                        {
                            if (key.Key == macrosVerbName && key.Value == i)
                            {
                                return this[key];
                            }
                        }                        
                    }
				}
                return "";
			}
			set
			{
				this[new KeyValuePair<string, int>( macrosVerbName, 1 )] = value;
			}
		}
        
        //public Dictionary<string,string> GetPageMacrs(int index)
        //{
        //    Dictionary<string, string> macrsDict = new Dictionary<string, string>();            
        //    foreach (KeyType key in this.Keys)
        //    {
        //        if (!macrsDict.ContainsKey(key.Key))
        //        {
        //            macrsDict[key.Key] = this[key.Key, index];
        //        }
        //    }
        //    return macrsDict;
        //}
        Dictionary<int, Dictionary<string, string>> pageIndexMacrsCache = new Dictionary<int, Dictionary<string, string>>();
        public Dictionary<string, string> GetPageMacrs(int index)
        {
            Dictionary<string, string> macrsDict = null;
            if (!pageIndexMacrsCache.TryGetValue(index, out macrsDict))
            {
                macrsDict = new Dictionary<string, string>();
                if (index >1 )
                {
                    Dictionary<string, string> prePageMacrs = GetPageMacrs(index-1);
                    foreach (KeyValuePair<string, string> entry in prePageMacrs)
                    {
                        macrsDict[entry.Key] = entry.Value;
                    }
                }
                foreach (KeyType key in this.Keys)
                {
                    if (key.Value == index)
                    {
                        macrsDict[key.Key] = this[key];
                    }
                }
                pageIndexMacrsCache[index] = macrsDict;
            }
            return macrsDict;
        }
        
        /// <summary>
        /// 查看宏变量是否在多页中存在
        /// </summary>        
        public bool HasMorePages(List<string> list)
        {
            bool hasMore = false;
            foreach (string macrosVerbName in list)
            {
                int count = 0;
                foreach (KeyType key in this.Keys)
                {
                    if (key.Key == macrosVerbName)
                    {
                        count++;
                        if (count > 1)
                        {
                            hasMore = true;
                            break;
                        }
                    }
                }
                if (hasMore) { break; }
            }
            return hasMore;
        }
        internal MacrosVerbCollection SubVerbCollection(List<string> list)
        {
            MacrosVerbCollection subVerb = new MacrosVerbCollection();
            foreach (KeyType entry in this.Keys)
            {
                if (list.Contains(entry.Key))
                {
                    subVerb[entry] = this[entry];
                }
            }
            return subVerb;
        }
    }
}
