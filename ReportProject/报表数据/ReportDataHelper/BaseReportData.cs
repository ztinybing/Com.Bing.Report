using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;

namespace Com.Bing
{
    public abstract class BaseReportData : IReportData
    {
        public const string ROWGROUP = "rowgroup";
        public const string MACRONAME = "macroName";
        public const string MACROVALUE = "macroValue";

        private ParmDict rptParamDic;
        public ParmDict RptParamDic
        {
            get { return rptParamDic; }
        }
        public abstract string[] GetColumns();

        public abstract DataTable GetBusinessData(DataSet ds, DataTable dt);

        /// <summary>
        /// 唯一入口
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="s"></param>
        /// <returns></returns>        
        public DataTable InitTable(DataSet ds, string[] s)
        {
            rptParamDic = ParseRPTParams(s);
            rptParamDic.RecordParm = true;

            ParmDict.AllParamDict.Clear();

            ReportDataHelper.RptParamDic = rptParamDic;

            InitReportParam(rptParamDic);

            //初始化DataTable,添加列
            DataTable dt = InitTableColumn(GetColumns());

            //参数设置
            InitReportParam(dt);

            //获取报表的业务数据            
            return GetBusinessData(ds, dt);
        }

        protected virtual void InitReportParam(ParmDict rptParamDic)
        {

        }

        protected virtual void InitReportParam(DataTable dt)
        {

        }

        #region private functions

        /// <summary>
        /// 获取报表的参数集合
        /// </summary>
        /// <param name="sarray"></param>
        /// <returns></returns>
        private ParmDict ParseRPTParams(string[] sarray)
        {
            ParmDict rptParamsDic = new ParmDict();
            string[] spliteResult = null;
            foreach (string s in sarray)
            {
                spliteResult = s.Split(new char[] { '=' });
                if (spliteResult.Length == 2)
                {
                    //参数先加载rpt参数后加载config 参数  rpt中存在的参数，config不覆盖
                    if (!rptParamsDic.ContainsKey(spliteResult[0]))
                    {
                        rptParamsDic.Add(spliteResult[0], spliteResult[1]);
                    }
                }
            }
            return rptParamsDic;
        }

        /// <summary>
        /// 初始化DataTable的列，第一列为RowGroup,最后加入宏变量列
        /// </summary>
        /// <param name="dataColumn">RPT中d003的列名</param>
        /// <returns></returns>
        private DataTable InitTableColumn(IEnumerable<string> dataColumn)
        {
            DataTable dt = new DataTable();
            //RowGroup 
            dt.Columns.Add(ROWGROUP, typeof(int));

            foreach (string columnName in dataColumn)
            {
                dt.Columns.Add(columnName, typeof(string));
            }

            //宏变量名称 
            dt.Columns.Add(MACRONAME, typeof(string));
            //宏变量值
            dt.Columns.Add(MACROVALUE, typeof(string));
            return dt;
        }
        #endregion

        #region protected functions
        /// <summary>
        /// 实现xml导入到datatable 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public DataTable ImportFromXml(string filePath)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filePath);
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(new XmlTextReader(new StringReader(xmldoc.InnerXml)));
                return ds.Tables[0];
            }
            catch
            {
                return new System.Data.DataTable();
            }
        }

        /// <summary>
        /// 计算part占total的百分比
        /// </summary>
        /// <param name="part">分子</param>
        /// <param name="total">分母</param>
        /// <returns></returns>
        public object PercentRatio(object part, object total)
        {
            double partVal = ToDouble(part);
            double totalVal = ToDouble(total);

            if (partVal == 0.0 || totalVal == 0.0)
            {
                return "";
            }

            string value = ((decimal)(partVal / totalVal)).ToString("P");

            return value == "0.00%" ? "" : value;
        }
        #endregion



        #region 实例公共方法
        /// <summary>
        /// 根据参数名，获取一个参数值，如果没有则返回一个期望的默认值
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="defvalue">期望的默认值</param>
        /// <returns></returns>
        public string GetRptParameter(string name, string defValue)
        {
            if (RptParamDic.ContainsKey(name))
            {
                return RptParamDic[name];
            }
            else
            {
                return defValue;
            }
        }
        /// <summary>
        /// 根据参数名，获取一个参数值，如果没有则返回一个期望的默认值
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="defvalue">期望的默认值</param>
        /// <returns></returns>
        public bool GetRptParameter(string name, bool defValue)
        {
            if (RptParamDic.ContainsKey(name))
            {
                return RptParamDic[name].Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return defValue;
            }
        }
        #endregion

        #region Static functions

        //保留小数的位数
        public static object SetPrecision(object inputStr, object num)
        {
            if (inputStr.Equals(""))
            {
                return inputStr;
            }
            if (Convert.ToDouble(inputStr.ToString()) == 0)
            {
                return "";
            }
            return Round(ToDouble(inputStr), (int)num);
        }

        #region 在数据表中插入分页、分隔行、宏等
        /// <summary>
        /// 插入分割行
        /// </summary>
        /// <param name="dt"></param>
        public static void InsertSplitingRow(DataTable dt)
        {
            DataRow pagingRow = dt.NewRow();
            pagingRow[ROWGROUP] = -2;
            dt.Rows.Add(pagingRow);
        }

        /// <summary>
        /// 插入分页行
        /// </summary>
        /// <param name="dt"></param>
        public static void InsertPagingRow(DataTable dt)
        {
            DataRow pagingRow = dt.NewRow();
            pagingRow[ROWGROUP] = -1;
            dt.Rows.Add(pagingRow);
        }

        /// <summary>
        /// 添加宏变量
        /// </summary>
        /// <param name="dt">目标表</param>
        /// <param name="macroName">宏变量名</param>
        /// <param name="macroValue">宏变量值</param>
        public static void AddMacro(DataTable dt, string macroName, string macroValue)
        {
            ReportDataHelper.AddMacro(dt, macroName, macroValue);
        }
        /// <summary>
        /// datatable 数值比对
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string DoubleCompareInDataTable(object obj, string col)
        {
            return DoubleCompareInDataTable(obj, col, "=");
        }
        public static string DoubleCompareInDataTable(object obj, string col, string oper)
        {
            return string.Format("convert('0'+{1},'System.Double') {2} {0}", ToDouble(obj), col, oper);
        }
        private List<object> GetSelectNodeList(DataTable dtConfig)
        {
            List<object> selectedNodes = new List<object>();
            DataRow[] drsCheckedGc = dtConfig.Select("keyname= 'checked' and keyval='true'");
            foreach (DataRow dr in drsCheckedGc)
            {
                selectedNodes.Add(dr["gc_chandle"]);
            }
            return selectedNodes;
        }
        #endregion

        public static string[] zhMark = new string[] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
        /// <summary>
        /// 只能取得中文件报表数据一至 九十九
        /// </summary>        
        public static string GetZHMark(int index)
        {
            int tens = index / 10;
            int units = index % 10;
            string zh = "";
            if (tens > 1 && tens < 10)
            {
                zh += zhMark[tens - 1];
            }
            if (tens > 0)
            {
                zh += zhMark[9];
            }
            if (units > 0)
            {
                zh += zhMark[units - 1];
            }

            return zh;
        }
        /// <summary>
        /// 返回一个值是否在数组里面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsInArray<T>(T[] array, T val)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (val.Equals(array[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 转换为数值
        /// </summary>
        /// <param name="strvalue"></param>
        /// <returns></returns>
        public static double ToDouble(object strvalue)
        {
            return ReportDataHelper.ToDouble(strvalue);
        }

        /// <summary>
        /// 中国式四舍五入一个数值
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="dec">取舍时所保留的小数位数</param>
        /// <returns></returns>
        public static object Round(object input, int dec)
        {
            double value = ReportDataHelper.Round(ToDouble(input), dec);
            return value.ToString("F" + dec);
        }
        /// <summary>
        /// 中国式四舍五入一个数值对象
        /// </summary>
        /// <param name="input">数值对象</param>
        /// <param name="dec">取舍时所保留小数位数</param>
        /// <returns></returns>
        public static double Round(double input, int dec)
        {
            return ReportDataHelper.Round(input, dec);
        }

        /// <summary>
        /// 获取数字的小数位数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetDigit(string str)
        {
            if (str.Contains("."))
                return str.Length - str.LastIndexOf('.');
            else
                return -1;
        }

        /// <summary>
        /// 根据代数运算式求值
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public static Double Compute(string formula)
        {
            return ReportDataHelper.Compute(formula);
        }
        /// <summary>
        /// 两数相加
        /// </summary>        
        public static object AddOpt(object objA, object objB)
        {
            return ReportDataHelper.AddOpt(objA, objB);
        }
        public static object DecOpt(object objA, object objB)
        {
            return ReportDataHelper.DecOpt(objA, objB);
        }
        public static object MulOpt(object objA, object objB)
        {
            return ReportDataHelper.MulOpt(objA, objB);
        }
        public static object DivOpt(object objA, object objB)
        {
            return ReportDataHelper.DivOpt(objA, objB);
        }
        /// <summary>
        /// 连乘
        /// </summary>        
        public static object Mutli(params object[] objs)
        {
            return Mult(objs);
        }
        /// <summary>
        /// 连乘
        /// </summary>        
        public static object Mult(params object[] objs)
        {
            return ReportDataHelper.Mult(objs);
        }
        /// <summary>
        /// 累加
        /// </summary>        
        public static object Sum(params object[] objs)
        {
            return ReportDataHelper.Sum(objs);
        }
        /// <summary>
        /// 从属性字典中获取指定键值对应的值，如果没有该键，返回空
        /// </summary>
        /// <param name="dicProperty"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string TryGetValue(Dictionary<string, string> dicProperty, string key)
        {
            string val = string.Empty;
            dicProperty.TryGetValue(key, out val);
            return val ?? string.Empty;
        }

        //判断是否为零
        public static bool IsZero(object obj)
        {
            return (ToDouble(obj) == 0.0);
        }

        public static bool IsNumber(object valueObje)
        {
            if (valueObje is double ||
                valueObje is Single ||
                valueObje is long ||
                valueObje is int)
            {
                return true;
            }

            double doubleVal = 0;
            return double.TryParse(valueObje.ToString(), out doubleVal);
        }

        public static bool StartWith(string source, params string[] words)
        {
            foreach (string word in words)
            {
                if (source.StartsWith(word))
                {
                    return true;
                }
            }
            return false;
        }
        public static string TrimStart(string source, params string[] trimStrings)
        {
            string result = source;

            while (StartWith(result, trimStrings))
            {
                foreach (string trimString in trimStrings)
                {
                    if (!string.IsNullOrEmpty(trimString) && result.StartsWith(trimString))
                    {
                        result = result.Substring(trimString.Length);
                    }
                }
            }
            return result;
        }
        // 移植PB的金额转换代码不含零版
        public static string DecToChina(decimal number, int cntype)
        {
            if (number < 0)
            {
                return "负" + PriceConveter.DecToChina(number * -1, cntype);
            }
            return PriceConveter.DecToChina(number, cntype);
        }
        //川发改政策〔2009〕1313号
        public static string DecToChina2(decimal number, int cntype)
        {
            return PriceConveter.DecToChina2(number, cntype);
        }
        //川发改政策〔2009〕1049号
        public static string DecToChina3(decimal number, int cntype)
        {
            return PriceConveter.DecToChina3(number, cntype);
        }

        public static List<string> GetAllParam()
        {
            List<string> list = new List<string>();
            list.AddRange(ParmDict.AllParamDict.Keys);
            list.Sort();
            return list;
        }
        #endregion

    }
}
