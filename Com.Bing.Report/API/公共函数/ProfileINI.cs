using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;

namespace Com.Bing.API
{
    public static class ProfileINI
    {

        #region 配制文件函数内核接口
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpReturnBytes, int nSize, string lpFileName);
        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpValue, string lpFileName);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(string strsection, string strkey, int ndefault, string strfilename);
        #endregion

        #region 内核接口封装

        /// <summary>
        /// 将[PARAMETER]到[/PARAMETER]标记之间的内容写入新文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="section"></param>
        /// <param name="iniFile"></param>
        public static void WriteSectionToFile(string file, string section, string iniFile)
        {
            WriteSectionToFile(file, section, iniFile, "", "");
        }
        public static void WriteSectionToFile(string file, string section, string iniFile, string oldSectionName, string newSectionName)
        {
            string secBegin = string.Format("[{0}]", section);
            string secEnd = string.Format("[/{0}]", section);
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(file, Encoding.Default))
            {
                string line = sr.ReadLine();
                bool start = false;
                for (int i = 0; line != null; i++)
                {
                    if (line.Trim() == secEnd)
                    {
                        start = false;
                    }
                    if (start)
                    {
                        sb.Append(line.Replace(oldSectionName, newSectionName) + "\r\n");
                    }
                    if (line.Trim() == secBegin)
                    {
                        start = true;
                    }
                    line = sr.ReadLine();
                }
            }

            File.Delete(iniFile);
            File.Create(iniFile).Close();
            using (StreamWriter sw = new StreamWriter(iniFile, false, Encoding.Default, 1024))
            {
                sw.Write(sb);
            }
        }
        /// <summary>
        /// 查找整数据的配置文件
        /// </summary>
        /// <param name="strfilename">文件名</param>
        /// <param name="strsection">节</param>
        /// <param name="strkey">键</param>
        /// <param name="intvalue"></param>
        /// <returns>值</returns>
        /// <example>
        /// INI -->fileName
        /// [modulelimit]  注：modulelimit ->strsection
        /// quota=1 注：quota-> strkey
        /// 返回 1
        /// </example>        
        public static int ProfileInt(string strfilename, string strsection, string strkey, int intvalue)
        {
            return GetPrivateProfileInt(strsection, strkey, intvalue, strfilename);
        }
        /// <summary>
        /// 查找配置文件给定条件的字符串值
        /// </summary>
        /// <param name="strfilename">名件名</param>
        /// <param name="strsection">节</param>
        /// <param name="strkey">键</param>
        /// <param name="strdefault">给定默认值</param>
        /// <returns>值</returns>
        /// <example>
        /// [造价元素关键列]  //造价元素关键列->strsection
        /// ElementFolderList=项目名称,清单编码 // ElementFolderList->键
        /// 返回 项目名称,清单编码
        /// </example>
        public static string ProfileString(string strfilename, string strsection, string strkey, string strdefault)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(strsection, strkey, strdefault, temp, 1024, strfilename);
            return temp.ToString();
        }

        /// <summary>
        /// 获取系统配置文件中的可选值列表 ,注：只能用于系统配置文件
        /// </summary>
        /// <param name="strSection">配置节</param>
        /// <param name="strKey">配置键</param>
        /// <returns>可选值列表</returns>
        /// <example>
        /// 例：[工程量小数位数保留设置]  //注 ：工程量小数位数保留设置 -->strSection
        ///     desc=工程量小数位数保留设置 
        ///     count=8
        ///     1=t、T、吨、千匹,L(3),0,1,2,3,4,5,6 //注:t、T、吨、千匹--> strKey
        ///     2=个、根、处、块、套、榀、对,L(0),0,1,2,3,4,5,6
        ///返回值：(0,1,2,3,4,5,6)
        /// </example>
        public static List<string> ProfileString(string strSection, string strKey)
        {
            List<string> vallist = new List<string>();
            int cnt = ProfileInt(Function.SysIniFile, strSection, "count", 0);
            string strVal;
            for (int i = 1; i <= cnt; i++)
            {
                //strVal = "t、T、吨、千匹,L(3),0,1,2,3,4,5,6"
                strVal = ProfileString(Function.SysIniFile, strSection, i.ToString(), "");
                // 包含 t、T、吨、千匹
                if (strVal.Contains(strKey))
                {
                    //取 0,1,2,3,4,5,6
                    string val = System.Text.RegularExpressions.Regex.Match(strVal, @"(?<=\),).*").Value;
                    //拆分成列表
                    vallist = new List<string>(val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    break;
                }
            }
            return vallist;
        }

        /// <summary>
        /// 获取系统配置文件中的可选值列表 ,注：只能用于例子似配制文件
        /// </summary>
        /// <param name="sysFileIni">系统配置文件名</param>
        /// <param name="section">配置节</param>
        /// <param name="key">配置键</param>
        /// <param name="val">指定值</param>
        /// <returns>可选值列表</returns>
        /// <example>
        /// 例：[工程量小数位数保留设置]  //注 ：工程量小数位数保留设置 -->strSection
        ///     desc=工程量小数位数保留设置 
        ///     count=8
        ///     1=t、T、吨、千匹,L(3),0,1,2,3,4,5,6 //注:t、T、吨、千匹--> strKey
        ///     2=个、根、处、块、套、榀、对,L(0),0,1,2,3,4,5,6
        /// 返回 3
        /// </example>
        public static string ProfileStringSys(string sysFileIni, string section, string key, string val)
        {
            string strVal = ProfileString(sysFileIni, section, key, val);
            if (strVal == val)
            {
                int cnt = ProfileInt(sysFileIni, section, "count", 0);
                for (int i = 1; i <= cnt; i++)
                {
                    //strVal = "t、T、吨、千匹,L(3),0,1,2,3,4,5,6"
                    strVal = ProfileString(sysFileIni, section, i.ToString(), "");
                    // 包含 t、T、吨、千匹
                    if (strVal.Contains(key))
                    {
                        //取 0,1,2,3,4,5,6
                        strVal = System.Text.RegularExpressions.Regex.Match(strVal, @"(?<=\().*(?=\))").Value;
                        break;
                    }
                }
            }
            return strVal;
        }

        public static bool SetProfileString(string strfilename, string strsection, string strkey, string strvalue)
        {
            return WritePrivateProfileString(strsection, strkey, strvalue, strfilename);
        }
        public static bool ClrProfileString(string strfilename, string strsection, string strkey)
        {
            SetProfileString(strfilename, strsection, strkey, "\n\n");

            StreamReader sr = new StreamReader(strfilename, Encoding.Default);
            string content = '\n' + sr.ReadToEnd();
            sr.Close();

            string target = '\n' + strkey + "=\n\n";
            content = content.Replace(target, "\n").TrimStart('\n');
            content = content.Replace("\r\n\r\n", "\r\n");
            content = content.Replace("\n\n", "\n");

            StreamWriter sw = new StreamWriter(strfilename, false, Encoding.Default);
            sw.Write(content);
            sw.Close();

            return true;
        }
        /// <summary>
        /// 获取配制文件中节列表
        /// </summary>
        /// <param name="strfilename">文件名</param>
        /// <returns>节列表</returns>
        public static string[] ProfileSectionNames(string strfilename)
        {

            uint MAX_BUFFER = 32767;
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER);
            uint bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, strfilename);
            if (bytesReturned == 0)
            {
                Marshal.FreeCoTaskMem(pReturnedString);
                return new string[0];
            }
            string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
            Marshal.FreeCoTaskMem(pReturnedString);
            return local.Substring(0, local.Length - 1).Split('\0');


        }

        /// <summary>
        /// 配制文件节的键值对
        /// </summary>
        /// <param name="strfilename">名称名</param>
        /// <param name="strsection">配制节</param>
        /// <returns>键值字典 </returns>
        public static Dictionary<string, string> ProfileSection(string strfilename, string strsection)
        {
            byte[] lpReturnBytes = new byte[32768];
            GetPrivateProfileSection(strsection, lpReturnBytes, 32768, strfilename);
            int len = 0;
            for (int i = 0; i < 32767; i++)
            {
                if (lpReturnBytes[i] == 0)
                {
                    if (lpReturnBytes[i + 1] == 0)
                    {
                        len = i;
                        break;
                    }
                    else
                    {
                        lpReturnBytes[i] = (byte)';';
                    }
                }
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] lpReturnString = System.Text.Encoding.Default.GetString(lpReturnBytes, 0, len).Split(';');
            foreach (string s in lpReturnString)
            {
                string[] pair = s.Split('=');
                if (pair.Length > 1)
                {
                    dict.Add(pair[0], pair[1]);
                }
            }

            return dict;
        }


        #endregion

        #region 用户+系统配制的函数接口
        public static string ProfileStringSysUser(string section, string key)
        {
            string val = ProfileString(Function.IniFile, section, key, "");
            if (string.IsNullOrEmpty(val))
            {
                val = ProfileStringSys(Function.IniFile, section, key, "");
            }
            return val;
        }
        /// <summary>
        /// 获取配置文件中的 键+默认值
        /// </summary>
        /// <param name="strSection">配置节</param>
        /// <returns>键值对</returns>
        /// <example>
        /// SysINI
        /// [strSection]        
        ///count=2
        ///1=Key1,L(3),0,1,2,3,4,5,6
        ///2=Key2,L(0),0,1,2,3,4,5,6
        /// INI
        /// Key2=2
        /// 
        /// 返回 ([Key1,3],[key2,2])
        /// </example>
        public static Dictionary<string, string> ProfileString(string strSection)
        {
            return ProfileDictionary(Function.SysIniFile, Function.IniFile, strSection);
        }
        public static Dictionary<string, string> ProfileDictionary(string sysIniFile, string iniFile)
        {
            return ProfileDictionary(sysIniFile, iniFile, ProfileSectionNames(sysIniFile));
        }
        public static Dictionary<string, string> ProfileDictionary(string sysIniFile, string iniFile, params string[] sections)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string strSection in sections)
            {
                int cnt = ProfileInt(Function.SysIniFile, strSection, "count", 0);
                string strKey, strVal, strConfig;
                for (int i = 1; i <= cnt; i++)
                {
                    strConfig = ProfileString(sysIniFile, strSection, i.ToString(), "");
                    strKey = strConfig.Split(',')[0];
                    //查找用户配制文件
                    strVal = ProfileString(iniFile, strSection, strKey, "");
                    if (strVal == "")
                    {
                        //Key1,L(3),0,1,2,3,4,5,6 匹配括号中的值
                        strVal = Regex.Match(strConfig, @"(?<=\().*(?=\))").Value;
                    }
                    dict[strKey] = strVal;
                }
            }

            return dict;
        }

        #endregion

        #region 用户+ 地区配制+系统配制的函数接口

        /// <summary>
        /// 包含地区信息的接口实例
        /// </summary>
        private static IArea iArea = null;
        public static IArea IArea
        {
            set { iArea = value; }

        }

        public static string AreaSysIniFile
        {
            get
            {
                string areaSysIniFile = string.Empty;
                if (iArea != null)
                {
                    areaSysIniFile = Function.AreaSysIniFile(iArea.Area);
                }
                return areaSysIniFile;
            }
        }
        public static string AreaIniFile
        {
            get
            {
                string areaIniFile = string.Empty;
                if (iArea != null)
                {
                    areaIniFile = Function.AreaIniFile(iArea.Area);
                }
                return areaIniFile;
            }
        }
        private static Dictionary<string, string> getIniFiles()
        {
            //由于AreaIniFile , AreaSysIniFile会动态变化，请不要删除该函数
            //"0" 仅 A=B 的模式，"1" 含配置
            Dictionary<string, string> iniFileList = new Dictionary<string, string>();
            if (iArea != null && !string.IsNullOrEmpty(iArea.Area))
            {
                iniFileList.Add(AreaIniFile, "0");
                iniFileList.Add(AreaSysIniFile, "1");
            }
            iniFileList.Add(Function.IniFile, "0");
            iniFileList.Add(Function.OemIniFile, "1");
            iniFileList.Add(Function.SysIniFile, "1");
            return iniFileList;
        }

        /// <summary>
        /// 给定 配置文件节 键 获取配置文件 值  注：处理可进行交互的配置信息节
        ///    配置文件 优先级
        ///     用户地区配置
        ///     地区配置
        ///     用户系统配置
        ///     系统配置
        /// </summary>
        /// <param name="strSection">配制节</param>
        /// <param name="strKey">配制键</param>
        /// <param name="defval">默认值</param>
        /// <returns>配制文件值</returns>
        public static string ProfileString(string strSection, string strKey, string defval)
        {
            Dictionary<string, string> iniFileList = getIniFiles();
            //查找用户地区配制
            string profileVal = string.Empty;
            foreach (KeyValuePair<string, string> iniFilePair in iniFileList)
            {

                if (iniFilePair.Value == "0")
                {
                    profileVal = ProfileString(iniFilePair.Key, strSection, strKey, "");
                }
                else
                {
                    profileVal = ProfileStringSys(iniFilePair.Key, strSection, strKey, "");
                }

                if (profileVal != string.Empty)
                {
                    break;
                }
            }
            return profileVal;
        }
        /// <summary>
        /// 查找系统配制文件中不进行交互的节
        /// </summary>        
        public static string ProfileStringSysArea(string strSection, string strKey, string defval)
        {
            Dictionary<string, string> iniFileList = getIniFiles();
            //查找用户地区配制
            string profileVal = string.Empty;
            foreach (KeyValuePair<string, string> iniFilePair in iniFileList)
            {
                if (iniFilePair.Value == "1")
                {
                    profileVal = ProfileString(iniFilePair.Key, strSection, strKey, "");
                }

                if (profileVal != string.Empty)
                {
                    break;
                }
            }
            return profileVal;
        }
        /// <summary>
        /// 查找系统配制文件中不进行交互的节
        /// </summary>        
        public static Dictionary<string, string> ProfileSectionsSysArea(string section)
        {
            Dictionary<string, string> areaIniDict = ProfileSection(AreaIniFile, section);
            Dictionary<string, string> sysIniDict = ProfileSection(Function.SysIniFile, section);
            foreach (string key in areaIniDict.Keys)
            {
                if (sysIniDict.ContainsKey(key))
                {
                    sysIniDict[key] = areaIniDict[key];
                }
            }
            return sysIniDict;
        }
        //其它函数接口， 需要时再进行扩展
        #endregion
    }
    public interface IArea
    {
        string Area
        {
            get;
        }
    }
}
