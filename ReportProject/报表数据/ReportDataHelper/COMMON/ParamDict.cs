using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing
{
    /// <summary>
    /// 参数字典，不会触发无参数异常
    /// </summary>
    public class ParmDict : Dictionary<string, string>
    {
        private static Dictionary<string, string> allParamDict = new Dictionary<string, string>();
        public static Dictionary<string, string> AllParamDict
        {
            get { return allParamDict; }
        }
        private bool recordParam = false;
        public bool RecordParm
        {
            get { return recordParam; }
            set { recordParam = value; }
        }
        public new string this[string name]
        {
            get
            {
                if (RecordParm && !allParamDict.ContainsKey(name) && !name.StartsWith("宏变量_"))
                {
                    string key = base.ContainsKey(name) ? string.Format("{0}={1}", name, base[name]) : name;
                    allParamDict[key] = "";
                }
                if (this.ContainsKey(name))
                {
                    return base[name];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                base[name] = value;
            }
        }
    }
}
