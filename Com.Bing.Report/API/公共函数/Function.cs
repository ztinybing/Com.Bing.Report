using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Xml;
using DevExpress.XtraEditors;
using System.Drawing;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Management;
using System.Net.Mail;
using Com.Bing.Forms;
using Com.Bing.Business;

//公用函数
namespace Com.Bing.API
{
    public class Function
    {
        /// <summary>
        /// 转换报表交换数据为电子表格
        /// <param name="vbaFile">报表交换数据</param>
        /// <param name="sheetName">电子表格名称</param>
        /// <param name="xlsFile">电子表格文件名称</param>
        /// <returns>0为成功</returns>
        /// </summary>        
        [System.Runtime.InteropServices.DllImport("TransToExcel.dll")]
        internal static extern long TransExcel(string vbaFile, string sheetName, string xlsFile);
        /// <summary>
        /// TransBegin TransExcelPart TransEnd 三个组合使用，进行批量发送
        /// </summary>        
        [System.Runtime.InteropServices.DllImport("TransToExcel.dll")]
        internal static extern long TransBegin(string xlsfile);
        [System.Runtime.InteropServices.DllImport("TransToExcel.dll")]
        internal static extern long TransEnd();
        [System.Runtime.InteropServices.DllImport("TransToExcel.dll")]
        internal static extern long TransExcelPart(string vbaFile, string sheetName);

        //判断网络连接状态，返回true 或 false
        [DllImport("wininet.dll", EntryPoint = "InternetGetConnectedState")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        public static bool InternetGetConnectedState()
        {
            int Desc = 0;
            return InternetGetConnectedState(out Desc, 0);
        }
       
        private static int debugIndex
        {
            get
            {
                return Application.StartupPath.ToLower().IndexOf("\\bin\\debug");
            }
        }

        public static bool Debuging
        {
            get
            {
                return debugIndex >= 0;
            }
        }

        public static bool DebugMode
        {
            get
            {
                return ProfileInt(IniFile, "ModuleLimit", "Debug", 0) == 1;
            }
            set
            {
                SetProfileString(IniFile, "ModuleLimit", "Debug", value ? "1" : "0");
            }
        }

        public static string AppPath
        {
            get
            {
                int idx = debugIndex;
                if (idx >= 0)
                {
                    return Application.StartupPath.Substring(0, idx);
                }
                else
                {
                    return Application.StartupPath;
                }
            }
        }

        public static string MakeFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch { }
            }
            return path;
        }

        public static void BatchCopy(string sourcePath, string targetPath, bool overrideExist)
        {
            Array.ForEach
            (Directory.GetFiles(sourcePath), delegate(string sourceFile)
            {
                string targetFile = sourceFile.Replace(sourcePath, targetPath);
                if (overrideExist || !File.Exists(targetFile))
                {
                    File.Copy(sourceFile, targetFile);
                }
            }
            );
        }

        public static string UserFolder
        {
            get
            {
                return MakeFolder(AppPath + "\\用户数据\\");
            }
        }
        public static string ReportFolder
        {
            get
            {
                return MakeFolder(AppPath + "\\报表数据\\");
            }
        }
        public static string UserReportFolder
        {
            get { return MakeFolder(AppPath + "\\用户数据\\报表数据\\"); }
        }
        public static string VbaFilePath
        {
            get { return ReportFolder + "vba.vba"; }
        }
        public static string XlsTemplate
        {
            get { return ReportFolder + "Com.Bing.Report"; }
        }
        public static string InterfaceFolder
        {
            get
            {
                return MakeFolder(AppPath + "\\接口数据\\");
            }
        }
        public static string SysFolder
        {
            get
            {
                return MakeFolder(AppPath + "\\系统数据\\");
            }
        }
        public static string TempFolder
        {
            get
            {
                return MakeFolder(AppPath + "\\临时数据\\");
            }
        }
        public static string SysDataFolder
        {
            get
            {
                MakeFolder(AppPath + "\\系统数据\\");
                return MakeFolder(AppPath + "\\系统数据\\数据库\\");
            }
        }
        public static string UserDataFolder
        {
            get
            {
                MakeFolder(AppPath + "\\用户数据\\");
                return MakeFolder(AppPath + "\\用户数据\\数据库\\");
            }
        }

        public static string IniFile
        {
            get { return UserFolder + "配置文件.ini"; }
        }

        public static string AreaIniFile(string area)
        {
            return UserFolder + "配置文件" + area + ".ini";
        }

        public static string SysIniFile
        {
            get { return SysFolder + "配置文件.ini"; }
        }

        public static string OemIniFile
        {
            get { return SysFolder + "OEM.ini"; }
        }

        public static string AreaSysIniFile(string area)
        {
            return SysFolder + "配置文件" + area + ".ini";
        }

        public static object Invoke(string FileName, string Namespace, string ClassName, string ProcName, object[] Parameters)
        {
            try
            {
                if (DebugMode)
                {
                    return LoadHelper.InvokeStandard(FileName, Namespace, ClassName, ProcName, Parameters);
                }
                else
                {
                    try
                    {
                        return LoadHelper.Invoke(FileName, Namespace, ClassName, ProcName, Parameters);
                    }
                    catch
                    {
                        return LoadHelper.InvokeStandard(FileName, Namespace, ClassName, ProcName, Parameters);
                    }
                }
            }
            catch (Exception e)
            {
                (new ExceptionDialog(new InnerException(e.GetBaseException().Message, e.GetBaseException()))).ShowDialog();
                return null;
            }
        }

        public static bool IsNumber(string strVal)
        {
            if(strVal.EndsWith("%") || strVal.EndsWith("％"))
            {
                strVal = strVal.Substring(0, strVal.Length - 1);
            }

            double doubleVal = 0;
            return string.IsNullOrEmpty(strVal) || double.TryParse(strVal, out doubleVal);
        }

        public static double ToDouble(string strValue)
        {
            double doubleVal = 0;
            double ratio = 1;
            if (strValue != null && (strValue.EndsWith("%") || strValue.EndsWith("％")))
            {
                strValue = strValue.TrimEnd(new char[] { '%', '％' });
                ratio = 0.01;
            }
            double.TryParse(strValue, out doubleVal);
            return doubleVal * ratio;
        }

        public static bool IsEmpty(string strvalue)
        {
            return (strvalue == "" || strvalue == null);
        }
        public static string MyTrim(string strvalue)
        {
            if (strvalue.Trim() == "" || strvalue == null)
                return "";
            return strvalue.Trim();
        }
        public static bool IsRun(string exe)
        {
            Process[] allProcess = Process.GetProcesses();
            foreach (Process p in allProcess)
            {

                if (p.ProcessName.ToLower() + ".exe" == exe.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        #region 配制文件相关函数
        public static string ProfileString(string strfilename, string strsection, string strkey, string strdefault)
        {
            return ProfileINI.ProfileString(strfilename, strsection, strkey, strdefault);            
        }
        public static bool SetProfileString(string strfilename, string strsection, string strkey, string strvalue)
        {
            return ProfileINI.SetProfileString(strfilename, strsection, strkey, strvalue);
        }
        public static bool ClrProfileString(string strfilename, string strsection, string strkey)
        {
            return ProfileINI.ClrProfileString(strfilename, strsection, strkey);
        }
        //获取配置字典
        public static Dictionary<string, string> ProfileDictionary(string sysIniFile, string iniFile)
        {
            return ProfileINI.ProfileDictionary(sysIniFile, iniFile);
        }
        public static Dictionary<string, string> ProfileDictionary(string sysIniFile, string iniFile, string[] sections)
        {
            return ProfileINI.ProfileDictionary(sysIniFile, iniFile, sections);
        }
        public static Dictionary<string, string> ProfileSection(string strfilename, string strsection)
        {
            return ProfileINI.ProfileSection(strfilename, strsection);
        }
        /// <summary>
        /// 获取用户配制文件信息(如无用户配制信息,则获取系统配制信息)
        /// </summary>
        public static Dictionary<string, string> ProfileString(string strSection)
        {
            return ProfileINI.ProfileString(strSection);
        }
        /// <summary>
        /// 获取配制节点可以选的默认值 
        /// 注：仅限于类型为L的默认值
        /// </summary>
        /// <param name="strSection">节点名</param>
        /// <param name="strItem">条目名</param>        
        public static List<string> ProfileString(string strSection, string strItem)
        {
            return ProfileINI.ProfileString(strSection, strItem);
        }        
        public static int ProfileInt(string strfilename, string strsection, string strkey, int intvalue)
        {
            return ProfileINI.ProfileInt(strfilename, strsection, strkey, intvalue);
        }
        public static string[] ProfileSectionNames(string strfilename)
        {
            return ProfileINI.ProfileSectionNames(strfilename);
        }       
        /// <summary>
        /// 将[PARAMETER]到[/PARAMETER]标记之间的内容写入新文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="section"></param>
        /// <param name="iniFile"></param>
        public static void WriteSectionToFile(string file, string section, string iniFile)
        {
            ProfileINI.WriteSectionToFile(file, section, iniFile);            
        }
        public static void WriteSectionToFile(string file, string section, string iniFile, string oldSectionName, string newSectionName)
        {
            ProfileINI.WriteSectionToFile(file, section, iniFile, oldSectionName, newSectionName);
        }
        public static string ProfileString(string strSection, string strKey, string deftval)
        {
            return ProfileINI.ProfileString(strSection, strKey, deftval);
        }
        #endregion
        /// <summary>
        /// 去掉文件中的特殊字符
        /// </summary>
        /// <param name="fileName">可能含有特殊字符的文件名</param>
        /// <returns>净化后的文件名</returns>
        public static string ReplaceSpecialCharWithBlank(string fileName)
        {
            List<char> invalidFileNameCharList = new List<char>(Path.GetInvalidFileNameChars());

            StringBuilder strB = new StringBuilder();
            for (int i = 0; i < fileName.Length; i++)
            {
                if (invalidFileNameCharList.Contains(fileName[i]))
                {
                    strB.Append(' ');
                }
                else
                {
                    strB.Append(fileName[i]);
                }
            }
            return strB.ToString();
        }
  


        public static string Round(string value, int jd, bool real)
        {
            if (real)
            {
                string tempValue = ToDouble(value).ToString("f" + jd.ToString());
                if (ToDouble(tempValue) == 0.0)
                    return "";
                else
                    return tempValue;
            }
            else
            {
                return Round(value, jd);
            }
        }

        public static string Round(string value, int jd)
        {
            string empty = "";
            value = ToDouble(value).ToString("f" + jd.ToString());
            //科学记数法不进行小数位补齐
            if(!Regex.IsMatch(value,@"[\+\-]?[\d]+([\.][\d]*)?([Ee][+-]?[\d]+)?"))
            {
                if (value.Trim() == "") value = "";
                else if (value.IndexOf(".") < 0)
                {
                    //没有小数点就进行小数点+位数补齐
                    value = value + "." + empty.PadRight(jd, '0');

                }
                else
                {
                    //小数位不够补齐
                    value = value + empty.PadRight(jd - (value.Length - value.IndexOf(".") - 1), '0');

                }
                if (value.Length > 0)
                {
                    //如果小数点前面没有数据，则补全一个零
                    if (value.Substring(0, 1) == ".")
                        value = "0" + value;
                }
            }
            return value;
        }

        public static string GetNewGCL(string strGCL, string strDW)
        {
            string strLeftNumber;

            strLeftNumber = GetLeftNumber(strDW);

            if (ToDouble(strLeftNumber) == 0) strLeftNumber = "1";
            string ret = MyTrim((ToDouble(strGCL) / ToDouble(strLeftNumber)).ToString());
            if (ToDouble(ret) == 0)
                return "";
            else
                return ret;
        }
        public static string GetNewGCL(string strOldGCL, string strOldDW, string strNewDW)
        {
            string strOldDWWordLeft;
            string strOldDWWordRight;
            string strNewDWWordLeft;
            string strNewDWWordRight;

            strOldDWWordLeft = GetLeftNumber(strOldDW);
            if (strOldDWWordLeft.Length < strOldDW.Length)
                strOldDWWordRight = strOldDW.Substring(strOldDWWordLeft.Length);
            else
                strOldDWWordRight = "";

            strNewDWWordLeft = GetLeftNumber(strNewDW);
            if (strNewDWWordLeft.Length < strNewDW.Length)
                strNewDWWordRight = strNewDW.Substring(strNewDWWordLeft.Length);
            else
                strNewDWWordRight = "";

            if (strOldDWWordRight.Trim() == strNewDWWordRight.Trim())
            {
                if (ToDouble(strOldDWWordLeft) == 0) strOldDWWordLeft = "1";
                if (ToDouble(strNewDWWordLeft) == 0) strNewDWWordLeft = "1";
                string ret = MyTrim((ToDouble(strOldGCL) * ToDouble(strOldDWWordLeft) / ToDouble(strNewDWWordLeft)).ToString());
                if (ToDouble(ret) == 0)
                    return "";
                else
                    return ret;
            }

            return "";
        }
        private static string GetLeftNumber(string strExp)
        {
            for (int i = 0; i < strExp.Length; i++)
            {
                if ((strExp[i] < '0' || strExp[i] > '9') && strExp[i] != '.')
                {
                    return strExp.Substring(0, i);
                }
            }
            return "";
        }
        private static string GetRightNumber(string strExp)
        {
            return strExp.Replace(GetLeftNumber(strExp), "");
        }

        //全角转为半角        
        public static string DBCToSBC(string oldstr)
        {
            return DBCToSBC(oldstr, "");
        }

        //全角转为半角        
        public static string DBCToSBC(string oldstr, string range)
        {
            char[] c = oldstr.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (range == "" || range.IndexOf(c[i]) > 0)
                {
                    if (c[i] == 12288)
                    {
                        c[i] = (char)32;
                        continue;
                    }
                    if (c[i] > 65280 && c[i] < 65375)
                    {
                        c[i] = (char)(c[i] - 65248);
                    }
                }
            }
            return new string(c);
        }

        //半角转为全角        
        public static string SBCToDBC(string oldstr)
        {
            return SBCToDBC(oldstr, "");
        }

        //半角转为全角        
        public static string SBCToDBC(string oldstr, string range)
        {
            char[] c = oldstr.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (range == "" || range.IndexOf(c[i]) > 0)
                {
                    if (c[i] == 32)
                    {
                        // 表示空格    
                        c[i] = (char)12288;
                        continue;
                    }
                    if (c[i] < 127 && c[i] > 32)
                    {
                        c[i] = (char)(c[i] + 65248);
                    }
                }
            }
            return new string(c);
        }

        //字符串形式设置对象属性
        public static void SetObjectPorperty(object target, string propertyKey, object propertyValue)
        {
            Type type = target.GetType();
            PropertyInfo proInfo;
            if (Regex.IsMatch(propertyKey, @"\."))
            {
                try
                {
                    string fisrtLayerKey = Regex.Match(propertyKey, @"^[^\.]+").Value;
                    proInfo = type.GetProperty(fisrtLayerKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    if (proInfo == null)
                        proInfo = type.GetProperty(fisrtLayerKey);
                    target = proInfo.GetValue(target, null);
                    SetObjectPorperty(target, propertyKey.Replace(fisrtLayerKey + ".", ""), propertyValue);
                }
                catch (Exception e)
                {
                    throw new Exception(" 中间层属性出错" + e.Message);
                }
            }
            else
            {
                try
                {
                    proInfo = type.GetProperty(propertyKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    if (proInfo == null)
                        proInfo = type.GetProperty(propertyKey);
                    proInfo.SetValue(target, propertyValue, null);
                }
                catch (Exception e)
                {
                    throw new Exception("最后一层属性出错 " + e.Message);
                }
            }
        }

        //字符串形式获取对象属性
        public static object GetObjectProperty(object target, string propertyKey)
        {
            Type type = target.GetType();
            PropertyInfo proInfo;
            if (Regex.IsMatch(propertyKey, @"\."))
            {
                try
                {
                    string fisrtLayerKey = Regex.Match(propertyKey, @"^[^\.]+").Value;
                    proInfo = type.GetProperty(fisrtLayerKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    if (proInfo == null)
                        proInfo = type.GetProperty(fisrtLayerKey);
                    target = proInfo.GetValue(target, null);
                    return GetObjectProperty(target, propertyKey.Replace(fisrtLayerKey + ".", ""));
                }
                catch (Exception e)
                {
                    throw new Exception(" 中间层属性出错" + e.Message);
                }
            }
            else
            {
                try
                {
                    proInfo = type.GetProperty(propertyKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    if (proInfo == null)
                        proInfo = type.GetProperty(propertyKey);
                    return proInfo.GetValue(target, null);
                }
                catch (Exception e)
                {
                    throw new Exception("最后一层属性出错 " + e.Message);
                }
            }
        }

        //统一的确认函数
        public static DialogResult Confirm(string message, string title, MessageBoxButtons buttons, MessageBoxDefaultButton defButton)
        {
            Control preFocusControl = Function.GetFoucsControl();
            DialogResult result = XtraMessageBox.Show(BaseDialog.GetActiveForm(), message, title, buttons, MessageBoxIcon.Question, defButton);

            if (preFocusControl != null)
            {
                preFocusControl.Focus();
            }
            return result;
        }

        public static bool Confirm(string message, string title, bool firstButton)
        {
            MessageBoxDefaultButton defButton = MessageBoxDefaultButton.Button2;
            if (firstButton)
            {
                defButton = MessageBoxDefaultButton.Button1;
            }
            return Confirm(message, title, MessageBoxButtons.OKCancel, defButton) == DialogResult.OK;
        }

        public static DialogResult Confirm(string message, string title, MessageBoxDefaultButton defButton)
        {
            return Confirm(message, title, MessageBoxButtons.YesNoCancel, defButton);
        }

        public static bool Confirm(string message, string title)
        {
            return Confirm(message, title, false);
        }

        //统一的提示函数
        public static void Alert(string message, string title)
        {
            if (Application.OpenForms.Count > 0)
            {
                XtraMessageBox.Show(BaseDialog.GetActiveForm(), message, title);
            }
            else
            {
                XtraMessageBox.Show(message, title);
            }
        }

        public static Dictionary<string, string> Array2Dict(string[] sarray, char split)
        {
            Dictionary<string, string> paramsDic = new Dictionary<string, string>();
            string[] spliteResult = null;
            foreach (string s in sarray)
            {
                spliteResult = s.Split(split);
                if (spliteResult.Length >= 2)
                {
                    StringBuilder sb = new StringBuilder(spliteResult[1]);
                    for (int i = 2; i < spliteResult.Length; i++)
                    {
                        sb.Append(split);
                        sb.Append(spliteResult[i]);
                    }
                    paramsDic.Add(spliteResult[0], sb.ToString());
                }
                else
                {

                }
            }
            return paramsDic;
        }

        #region 单位换算
        /// <summary>
        /// 新旧单位换算，得出新的工程量
        /// </summary>
        /// <param name="oldBill">老的工程量</param>
        /// <param name="oldDw">老单位</param>
        /// <param name="newDW">新单位</param>
        /// <returns>换算之后的工程量</returns>
        public static string UnitTrans(string oldBill, string oldDw, string newDW)
        {
            string newBill = "0";
            if (oldBill != "" && IsNumber(oldBill))
            {
                if (Convert.ToDecimal(oldBill) > 0)
                {

                    decimal para = compare(oldDw, newDW);
                    if (para == 0)
                    {
                        para = compare(uniform(oldDw), uniform(newDW));
                    }
                    if (para > 0) newBill = Convert.ToString(Convert.ToDecimal(oldBill) * para);
                }
            }
            if (ToDouble(newBill) == 0) newBill = "";
            return newBill;
        }
        /// <summary>
        /// 比较清单和定额的单位是否相同
        /// </summary>
        /// <param name="qddw">清单单位</param>
        /// <param name="dedw">定额单位</param>
        /// <returns>单位的比值，不同单位返回0</returns>
        private static decimal compare(string qddw, string dedw)
        {
            qddw = normal(qddw);
            dedw = normal(dedw);
            if (qddw == dedw)
            {
                return 1;
            }
            else
            {
                if (Regex.IsMatch(qddw, @"[(]|[)]|\.\D+|/") || Regex.IsMatch(dedw, @"[(]|[)]|\.\D+|/"))
                {
                    return 0;
                }
                else
                {
                    decimal qdnum = 1, denum = 1;
                    string qdname = "", dename = "";
                    string reg = @"[\u4e00-\u9fa5]+\d+|[\u4e00-\u9fa5]+|[A-Za-z]+\d+|[A-Za-z]+";
                    if (Regex.IsMatch(qddw, reg)) qdname = Regex.Match(qddw, reg).Value.ToString();
                    if (Regex.IsMatch(dedw, reg)) dename = Regex.Match(dedw, reg).Value.ToString();
                    if (qdname == dename)
                    {
                        qdnum = BeforeValueDecimal(qddw);
                        denum = BeforeValueDecimal(dedw);
                        return qdnum / denum;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
        /// <summary>
        /// 统一单位 国标
        /// </summary>
        /// <param name="unit">原始单位</param>
        /// <returns>国标单位</returns>
        private static string uniform(string unit)
        {
            decimal num = 1;
            StringBuilder dw = new StringBuilder();

            if (Regex.IsMatch(unit, "公里"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("公里", "km");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "十"))
            {
                num = BeforeValueDecimal(unit);
                dw.Remove(0, dw.Length);
                dw.Append(Convert.ToString(num * 10));
                dw.Append(Regex.Match(unit, @"(?<=十)\w+"));
                unit = dw.ToString();
            }
            if (Regex.IsMatch(unit, "百"))
            {
                num = BeforeValueDecimal(unit);
                dw.Remove(0, dw.Length);
                dw.Append(Convert.ToString(num * 100));
                dw.Append(Regex.Match(unit, @"(?<=百)\w+"));
                unit = dw.ToString();
            }
            if ((Regex.IsMatch(unit, "千") || Regex.IsMatch(unit, "k")) && !(Regex.IsMatch(unit, "千克") || Regex.IsMatch(unit, "kg")))
            {
                num = BeforeValueDecimal(unit);
                dw.Remove(0, dw.Length);
                dw.Append(Convert.ToString(num * 1000));
                dw.Append(Regex.Match(unit, @"(?<=千)\w+|(?<=k)\w+"));
                unit = dw.ToString();
            }
            if (Regex.IsMatch(unit, "万"))
            {
                num = BeforeValueDecimal(unit);
                dw.Remove(0, dw.Length);
                dw.Append(Convert.ToString(num * 10000));
                dw.Append(Regex.Match(unit, @"(?<=万)\w+"));
                unit = dw.ToString();
            }
            if (Regex.IsMatch(unit, "吨"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("吨", "t");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "平方米"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("平方米", "m2");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "立方米"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("立方米", "m3");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "平方厘米"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("平方厘米", "cm2");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "立方厘米"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("立方厘米", "cm3");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "厘米"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("厘米", "cm");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "米"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("米", "m");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "毫升"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("毫升", "ml");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "升"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("升", "l");
                dw.Append(unit);
            }
            if (Regex.IsMatch(unit, "t") || Regex.IsMatch(unit, "T"))
            {
                num = BeforeValueDecimal(unit);
                dw.Remove(0, dw.Length);
                dw.Append(Convert.ToString(num * 1000));
                dw.Append("kg");
            }
            if (Regex.IsMatch(unit, "千克") || Regex.IsMatch(unit, "公斤"))
            {
                dw.Remove(0, dw.Length);
                unit = unit.Replace("千克", "kg");
                unit = unit.Replace("公斤", "kg");
                dw.Append(unit);
            }
            if ((Regex.IsMatch(unit, "克") || Regex.IsMatch(unit, "g")) && !Regex.IsMatch(unit, "kg"))
            {
                num = BeforeValueDecimal(unit);
                dw.Remove(0, dw.Length);
                dw.Append(Convert.ToString(num / 1000));
                dw.Append("kg");
            }
            if (dw.Length > 0)
            {
                return dw.ToString();
            }
            else
            {
                return unit;
            }
        }
        /// <summary>
        /// 获取单位前面的数字，无数字的默认为1
        /// </summary>
        /// <param name="str">单位</param>
        /// <returns>单位前的数字</returns>
        public static string BeforeValueString(string str)
        {
            string reg = @"^\d+[\.]\d+(?=[\u4e00-\u9fa5]*|[A-Za-z]*)|^\d+(?=[\u4e00-\u9fa5]*|[A-Za-z]*)";
            Match match = Regex.Match(str, reg);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "1";
            }
        }
        public static string AfterValueString(string str)
        {
            string reg = @"^\d+[\.]\d+(?=[\u4e00-\u9fa5]*|[A-Za-z]*)|^\d+(?=[\u4e00-\u9fa5]*|[A-Za-z]*)";
            Match match = Regex.Match(str, reg);
            if (match.Success)
            {
                return str.Substring(match.Value.Length);
            }
            else
            {
                return str;
            }
        }
        public static decimal BeforeValueDecimal(string str)
        {
            return Convert.ToDecimal(BeforeValueString(str));
        }
        public static double BeforeValueDouble(string str)
        {
            return Convert.ToDouble(BeforeValueString(str));
        }
        /// <summary>
        /// 统一全半角
        /// </summary>
        /// <param name="unit">单位</param>
        /// <returns></returns>
        private static string normal(string unit)
        {
            unit = unit.Trim();
            unit = unit.ToLower();
            unit = DBCToSBC(unit);
            return unit;
        }
        #endregion


        #region 数据行颜色存取
        public static Dictionary<string, Color> ColorDict = new Dictionary<string, Color>();
        /// <summary>
        /// 数据行颜色存取
        /// </summary>
        private static void setColor(string section, string colorType, string defColor)
        {
            string colorStr = ProfileString(IniFile, section, colorType, defColor);
            if (colorStr == "255,255,255")//白色表示不取色
            {
                colorStr = "";
            }
            ColorDict[colorType] = String2Color(colorStr);
        }

        public static Color String2Color(string colorStr)
        {
            //系统配置用的;号 用户配置用的,号
            colorStr = colorStr.Replace(";", ",");

            //按RGB取色或者名称取色
            if (colorStr.Split(',').Length == 3)
            {
                return Color.FromArgb(Convert.ToInt32(colorStr.Split(',')[0]), Convert.ToInt32(colorStr.Split(',')[1]), Convert.ToInt32(colorStr.Split(',')[2]));
            }
            else
            {
                return Color.FromName(colorStr);
            }
        }

        public static void InitColorDict(string[] section)
        {
            //zinger:20101126 清除冗余代码
            string defColor;
            string defKey;
            int cnt;

            for (int iCnt = 0; iCnt < section.Length; iCnt++)
            {
                cnt = ProfileInt(SysIniFile, section[iCnt], "count", 0);
                for (int i = 1; i <= cnt; i++)
                {
                    defColor = ProfileString(SysIniFile, section[iCnt], i.ToString(), "").Split(',')[1].Substring(2).TrimEnd(')');
                    defKey = ProfileString(SysIniFile, section[iCnt], i.ToString(), "").Split(',')[0];
                    setColor(section[iCnt], defKey, defColor);
                }
            }
        }
        #endregion

        #region 热键支持
        //如果函数执行成功，返回值不为0。        
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(
            IntPtr hWnd,                //要定义热键的窗口的句柄            
            int id,                     //定义热键ID（不能与其它ID重复）                       
            KeyModifiers km,            //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效            
            Keys vk                     //定义热键的内容            
            );
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄            
            int id                      //要取消热键的ID            
            );

        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）        
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }
        private static int iHotKeyCnt = 0;
        private static Dictionary<int, HotKeyFuncDele> dictHotKey = new Dictionary<int, HotKeyFuncDele>();
        public delegate void HotKeyFuncDele(Object sender, EventArgs e);
        private static Control objHotKey = null;
        public static void SetControl(Control obj)
        {
            objHotKey = obj;
        }
        public static void RegisterHotKey(HotKeyFuncDele dele, KeyModifiers km, Keys vk)
        {
            if (objHotKey != null)
            {
                iHotKeyCnt++;
                if (Debuging)
                {
                    km |= KeyModifiers.Shift;
                }
                RegisterHotKey(objHotKey.Handle, iHotKeyCnt, km, vk);
                dictHotKey[iHotKeyCnt] = dele;
            }
        }
        public static void UnregisterHotKeys()
        {
            if (objHotKey != null)
            {
                foreach (KeyValuePair<int, HotKeyFuncDele> item in dictHotKey)
                {
                    UnregisterHotKey(objHotKey.Handle, item.Key);
                }
                dictHotKey.Clear();
                iHotKeyCnt = 0;
            }
        }
        public static void HotKeyWndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;    //按快捷键     
            int wParam = m.WParam.ToInt32();
            if (m.Msg == WM_HOTKEY)
            {
                if (dictHotKey.ContainsKey(wParam))
                {
                    dictHotKey[wParam](objHotKey, new EventArgs());
                }
            }
        }
        #endregion

        #region 产品自启动键值
        public static void SetProductKey(string product)
        {
            try
            {
                RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + product, true);
                regkey.SetValue("ins_path", AppPath);
            }
            catch
            {
            }
        }
        #endregion

        /// <summary>
        /// 通用编码简写规则解释
        /// </summary>
        /// <param name="code">简写编码</param>
        /// <param name="preCode">上一个正确格式编码</param>
        /// <returns>正确格式的编码</returns>
        public static string CodeBriefRule(string code, string preCode)
        {
            //数字-数字格式
            code = code.ToUpper();
            if (preCode.Contains("-"))
            {
                if (!code.Contains("-") && !Regex.IsMatch(code, ".+[A-Z|a-z]", RegexOptions.None))
                {
                    code = preCode.Substring(0, preCode.IndexOf('-') + 1) + code;
                }
            }
            else if(!code.Contains("-"))
            {
                code = code.PadLeft(2, '0');
                int len = preCode.Length;
                if (len > 2)
                {
                    if (Regex.IsMatch(code, ".+[A-Z|a-z]", RegexOptions.None))
                    {
                        code = code.Substring(0, 2) + code.Substring(2).PadLeft(len - 2, '0');
                    }
                    else
                    {
                        code = preCode.Substring(0, 2) + code.PadLeft(len - 2, '0');
                    }
                }
            }
            return code;
        }

        static string _T(string s)
        {
            return s;
        }

        public static string HZ2PY(string hz)
        {
            if (hz == null)
            {
                hz = "";
            }
            byte[] sHZ = System.Text.Encoding.Default.GetBytes(hz);

            byte ucHigh, ucLow;
            int nCode;
            string sLT = "V";
            string ret = "";
            for (int i = 0; i < sHZ.Length; )
            {
                if (sHZ[i] < 0x80)
                {
                    ret = ret + (char)sHZ[i];
                    i++;
                    continue;
                }

                ucHigh = sHZ[i];
                ucLow = sHZ[i + 1];
                i += 2;
                if (ucHigh < 0xa1 || ucLow < 0xa1)
                {
                    continue;
                }
                else
                {
                    nCode = (ucHigh - 0xa0) * 100 + ucLow - 0xa0;
                }

                if (nCode >= 1601 && nCode < 1637) sLT = "A";
                if (nCode >= 1637 && nCode < 1833) sLT = "B";
                if (nCode >= 1833 && nCode < 2078) sLT = "C";
                if (nCode >= 2078 && nCode < 2274) sLT = "D";
                if (nCode >= 2274 && nCode < 2302) sLT = "E";
                if (nCode >= 2302 && nCode < 2433) sLT = "F";
                if (nCode >= 2433 && nCode < 2594) sLT = "G";
                if (nCode >= 2594 && nCode < 2787) sLT = "H";
                if (nCode >= 2787 && nCode < 3106) sLT = "J";
                if (nCode >= 3106 && nCode < 3212) sLT = "K";
                if (nCode >= 3212 && nCode < 3472) sLT = "L";
                if (nCode >= 3472 && nCode < 3635) sLT = "M";
                if (nCode >= 3635 && nCode < 3722) sLT = "N";
                if (nCode >= 3722 && nCode < 3730) sLT = "O";
                if (nCode >= 3730 && nCode < 3858) sLT = "P";
                if (nCode >= 3858 && nCode < 4027) sLT = "Q";
                if (nCode >= 4027 && nCode < 4086) sLT = "R";
                if (nCode >= 4086 && nCode < 4390) sLT = "S";
                if (nCode >= 4390 && nCode < 4558) sLT = "T";
                if (nCode >= 4558 && nCode < 4684) sLT = "W";
                if (nCode >= 4684 && nCode < 4925) sLT = "X";
                if (nCode >= 4925 && nCode < 5249) sLT = "Y";
                if (nCode >= 5249 && nCode < 5590) sLT = "Z";
                if (nCode >= 1601 && nCode < 5590)
                {
                    ret = ret + sLT;
                }
            }
            return ret;
        }

        public static void AutoFileVersion()
        {
            string file = AppPath + "\\Properties\\AssemblyInfo.cs";
            if (!File.Exists(file))
            {
                file = AppPath + "\\..\\Properties\\AssemblyInfo.cs";
            }
            if (File.Exists(file))
            {
                FileInfo info = new FileInfo(file);
                if ((info.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    info.Attributes = info.Attributes ^ FileAttributes.ReadOnly;
                }
                System.IO.StreamReader sr = new System.IO.StreamReader(file, System.Text.Encoding.Default);
                string content = sr.ReadToEnd();
                sr.Close();
                int start = content.IndexOf("AssemblyVersion(\"") + ("AssemblyVersion(\"").Length;
                int end = content.IndexOf("\")", start);
                if (start >= 0 && end >= 0)
                {
                    string s = content.Substring(start, end - start);
                    string t = s.Substring(0, s.LastIndexOf('.') + 1) + DateTime.Now.DayOfYear.ToString().PadLeft(3, '0');
                    if (s != t)
                    {
                        content = content.Substring(0, start) + t + content.Substring(end);
                        
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(file, false, System.Text.Encoding.Default);
                        sw.Write(content);
                        sw.Close();
                    }
                }
            }
        }

        public static void AutoUpdate()
        {
            AutoUpdate("auto");
        }

        public static void AutoUpdate(string arg)
        {
            if (!IsRun("autoupdate.exe"))
            {
                string curFile = AppPath + "\\AutoUpdate.EXE";
                string tmpFile = TempFolder + "autoupdate.exe";
                if (File.Exists(tmpFile))
                {
                    if (File.Exists(curFile))
                    {
                        File.Delete(curFile);
                    }
                    File.Move(tmpFile, curFile);
                }
                ////无升级程序也不影响主程序运行， 但记录升级程序未运行的情詋
                if (!File.Exists(curFile))
                {
                    //Log : No Update Program!
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Process.Start(curFile, arg);
                    }
                    catch/*(Exception e)*/
                    {
                        //Log : Update Program Occur Unexceptions!
                        //call Stact and Exception Message.
                    }
                }
            }
            else if (arg == "")//无参数，表示非启动调用
            {
                Function.Alert("升级程序已经运行", "提示");
            }
        }

        /// <summary>
        /// 错误记录
        /// </summary>
        public static void RecordError(string msg)
        {
            StreamWriter writer = new StreamWriter(TempFolder + "RecordError.txt", true);
            writer.WriteLine();
            writer.WriteLine(DateTime.Now.ToString());
            StackFrame frame = new StackTrace().GetFrame(1);
            //writer.WriteLine(frame.GetFileName());
            //writer.WriteLine(frame.GetFileLineNumber());
            writer.WriteLine(frame.GetMethod().ToString());
            writer.WriteLine(msg);
            writer.Close();
        }
        public static void RecordErrorPlain(string msg)
        {
            StreamWriter writer = new StreamWriter(TempFolder + "RecordError.txt", true);
            writer.WriteLine(msg);
            writer.Close();
        }
        /// <summary>
        /// 字节数组转换成字符串
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static string Dec2Str(byte[] dec)
        {
            string str = "";
            for (int i = 0; i < dec.Length; i++)
            {
                str += Convert.ToString(dec[i], 10).PadLeft(3, '0');
            }
            return str;
        }

        /// <summary>
        /// 字符串转换成字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] Str2Dec(string str)
        {
            byte[] dec = new byte[str.Length / 3];
            for (int i = 0; i < dec.Length; i++)
            {
                dec[i] = Convert.ToByte(str.Substring(i * 3, 3));
            }
            return dec;
        }

        /// <summary>
        /// 按填充色绘制图片
        /// </summary>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Bitmap DrawColorBitmap(Size size, Color color)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(new SolidBrush(color), 0, 0, size.Width, size.Height);
            g.Save();
            g.Dispose();
            return bmp;
        }

        /// <summary>
        /// 求最大公串
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static string GetLargePublicString(string A, string B)
        {
            string shortString = A.Length > B.Length ? B : A;//取较短者; 
            string longString = A.Length > B.Length ? A : B;//取较长者; 
            for (int i = shortString.Length; i > 0; i--)//长度递减 
            {
                for (int j = 0; j <= shortString.Length - i; j++)//位置递增 
                {
                    if (longString.IndexOf(shortString.Substring(j, i)) != -1)
                    {
                        return shortString.Substring(j, i);
                    }
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder"></param>
        /// <param name="overWrite"></param>
        public static void CopyFolder(string sourceFolder, string destFolder, bool overWrite)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }
            if (!Directory.Exists(sourceFolder)) return;
            string[] files = Directory.GetFiles(sourceFolder);
            for (int i = 0; i < files.Length; i++)
            {
                string[] childfile = files[i].Split('\\');
                string destFile = destFolder + @"\" + childfile[childfile.Length - 1];
                if (File.Exists(destFile) && !overWrite) continue;
                File.Copy(files[i], destFile, true);
            }
            string[] dirs = Directory.GetDirectories(sourceFolder);
            for (int i = 0; i < dirs.Length; i++)
            {
                string[] childdir = dirs[i].Split('\\');
                CopyFolder(dirs[i], destFolder + @"\" + childdir[childdir.Length - 1], overWrite);
            }
        }
        /// <summary>
        /// 获取打印机列表
        /// </summary>
        /// <returns></returns>
        public static StringCollection GetPrintersCollection()
        {
            StringCollection printerNameCollection = new StringCollection();
            string searchQuery = "SELECT   *   FROM   Win32_Printer ";
            ManagementObjectSearcher searchPrinters =
                        new ManagementObjectSearcher(searchQuery);
            ManagementObjectCollection printerCollection = searchPrinters.Get();
            foreach (ManagementObject printer in printerCollection)
            {
                printerNameCollection.Add(printer.Properties["Name"].Value.ToString());
            }
            return printerNameCollection;
        }
        /// <summary>
        /// 费用计算式中常用的操作符号 ,可用于进行变量替换{0}CommUsedOperator
        /// </summary>
        public static string CommUsedOperator = @"[+-/\(\)＋－×÷／（）\*]";
        // [\(（]?(?<xxx>.*?)[\)）]?[×\*].*费率
        // (?<=[\(（]).*(?=[）\)]+[\*×].*率)|.*[^)）](?=[\*×].*率)
        /// <summary>
        /// 取费用基础的正则表达式 (@)*xx费率 or @*xx费率
        /// </summary>
        public static string BaseFeesRegular = @"(?<=[\(（]).*(?=[）\)]+[\*×].*率)|.*[^)）](?=[\*×].*率)|(?<=[\(（]).*(?=[）\)]+[\*×].*系数)|.*[^)）](?=[\*×].*系数)";
        /// <summary>
        /// 获取费用计算公式的中的变量或常量
        /// </summary>
        /// <param name="exp">计算公式</param>        
        public static List<string> GetExpressionVar(string exp)
        {
            List<string> varList = new List<string>();
            string[] varArray = Regex.Split(exp, CommUsedOperator);
            foreach (string var in varArray)
            {
                if (!string.IsNullOrEmpty(var))
                {
                    varList.Add(var);
                }
            }
            return varList;
        }

        /// <summary>
        /// 以变量字典替换费用计算公式中的变量
        /// </summary>
        /// <param name="exp">计算公式</param>
        /// <param name="VarValDict">变量=值字典</param>
        /// <returns>替换的公式表达式</returns>
        public static string ReplaceExpressionVar(string exp, Dictionary<string, string> VarValDict)
        {
            List<string> varList = GetExpressionVar(exp);
            foreach (string key in VarValDict.Keys)
            {
                if (varList.Contains(key))
                {
                    exp = Regex.Replace(exp, string.Format("{0}(?={1})", key, CommUsedOperator), VarValDict[key]);
                }
            }
            return exp;
        }
        /// <summary>
        /// 从费用表达式中获取费用基础
        /// </summary>
        public static string GetFeeCalcBase(string exp)
        {
            return Regex.Match(exp, BaseFeesRegular).Value;
        }
        public static Control GetFoucsControl()
        {
            Control focusControl = null;
            IntPtr focusHandle = Win32API.GetFocus();
            if (focusHandle != IntPtr.Zero)
            {
                focusControl = Control.FromHandle(focusHandle);                
            }
            Control tempControl = focusControl;
            while (tempControl != null && tempControl.Parent != null)
            {
                tempControl =  tempControl.Parent;
                if (tempControl is DevExpress.XtraTreeList.TreeList)
                {
                    focusControl = tempControl;
                    break;
                }
                
            }
            return focusControl;
        }

        public static void SendMail(string body)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add("2217338754@qq.com");
            msg.From = new MailAddress("2217338754@qq.com", "kfb", System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = "程序错误";//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = body;//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = false;//是否是HTML邮件    
            msg.Priority = MailPriority.High;//邮件优先级    

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("2217338754@qq.com", "20110505kfb");
            //在zj.com注册的邮箱和密码
            client.Host = "smtp.qq.com";
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState);
            }
            catch// (System.Net.Mail.SmtpException ex)
            {
                //MessageBox.Show(ex.Message, "发送邮件出错");
            }
        }

        private static DataTable dtCompute = new DataTable();
        public static Double Compute(string formula)
        {
            double result = 0;
            string ret = "";
            try
            {
                ret = dtCompute.Compute(formula, "").ToString();
                double.TryParse(ret, out result);
            }
            catch
            {
                result = double.NaN;
            }
            return result;
        }

        public static string GetShortPathName(string longPathName)
        {
            StringBuilder builder = new StringBuilder(65535);
            Win32API.GetShortPathName(longPathName, builder, 65535);
            return builder.ToString();
        }
        public static void RegisterEnterButton(Control control, IButtonControl btn)
        {
            control.KeyUp += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btn.PerformClick();
                }
            };
        }

        private const int GWL_STYLE = -16;
        private const int WS_DISABLED = 0x8000000;
        public static void SetControlEnabled(Control c, bool enabled)
        {
            if (c != null)
            {
                if (enabled)
                { Win32API.SetWindowLong(c.Handle, GWL_STYLE, (~WS_DISABLED) & Win32API.GetWindowLong(c.Handle, GWL_STYLE)); }
                else
                { Win32API.SetWindowLong(c.Handle, GWL_STYLE, WS_DISABLED + Win32API.GetWindowLong(c.Handle, GWL_STYLE)); }
            }
        }

        public static string RoundString(string val, int digits)
        {
            if (string.IsNullOrEmpty(val))
            {
                return "";
            }
            else
            {
                int pos = val.IndexOf('.');
                if (pos < 0)
                {
                    return string.Concat(val, '.', new string('0', digits));
                }
                int realLength = digits + pos + 1;
                char[] chars = val.PadRight(realLength + 1, '0').ToCharArray();
                bool upDigit = chars[realLength] > '4';
                for (int i = realLength - 1; upDigit && i >= -1; i--)
                {
                    if (i == -1)
                    {
                        return string.Concat('1', new string(chars, 0, realLength));
                    }
                    if (chars[i] == '.')
                    {
                        continue;
                    }
                    if (chars[i] == '9')
                    {
                        chars[i] = '0';
                    }
                    else
                    {
                        chars[i]++;
                        break;
                    }
                }
                return new string(chars, 0, realLength);
            }
        }

        public static string GetFileHeadInfo(string file, int length)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                return "";
            }
            BinaryReader reader = new BinaryReader(fs);
            byte[] bytes = new byte[length];
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            bytes = reader.ReadBytes(length);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b > 15 ? Convert.ToString(b, 16) : '0' + Convert.ToString(b, 16));
            }
            fs.Close();
            reader.Close();
            return sb.ToString();
        }

        private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        private const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();
        [DllImport("Kernel32.dll")]
        private static extern int FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, [Out]StringBuilder lpBuffer, uint nSize, IntPtr arguments);
        public static string GetLastErrorMessage()
        {
            uint dwFlags = FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;
            StringBuilder lpBuffer = new StringBuilder(260);
            uint error = GetLastError();
            FormatMessage(dwFlags, IntPtr.Zero, error, 0, lpBuffer, 260, IntPtr.Zero);
            return error.ToString() + ":" + lpBuffer.ToString();
        }
        public static bool UpperFlag = false;        
        public static string UPPERMARKREGEX = @"m[23２３]";
        /// <summary>
        /// M2 M3 上标化处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GenUpperMark(string context)
        {
            if (!UpperFlag)
            {
                return context;
            }

            if (Regex.IsMatch(context, UPPERMARKREGEX, RegexOptions.IgnoreCase))
            {
                context = Regex.Replace(context, UPPERMARKREGEX ,UpperMark, RegexOptions.IgnoreCase);
            }
            return context;
        }

        /// <summary>
        /// '' 不为空字符
        /// m2、m3、m２、m３ 换成上标
        /// </summary>
        /// <param name="match"></param>        
        private static string UpperMark(Match match)
        {
            string replaceStr = match.Value;
            switch (match.Value)
            {
                case"m2":
                case "m２":
                case "M2":
                case "M２":
                    //不为空字符
                    replaceStr = "";
                    break;
                case "m3":
                case "m３":
                case "M3":
                case "M３":
                    //不为空字符
                    replaceStr = "";
                    break;
            }
            return replaceStr;
        }
    }
}