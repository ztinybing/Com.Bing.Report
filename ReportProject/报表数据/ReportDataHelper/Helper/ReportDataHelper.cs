using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Com.Bing
{
    public interface IReportData
    {
        string[] GetColumns();
        DataTable GetBusinessData(DataSet ds, DataTable dt);
    }

    public class ReportDataHelper
    {
        /// <summary>
        /// 参数字典
        /// </summary>
        private static ParmDict rptParamDic;
        public static ParmDict RptParamDic
        {
            get { return rptParamDic; }
            set { rptParamDic = value; }
        }


        /// <summary>
        /// 转换为数值
        /// </summary>
        /// <param name="strvalue"></param>
        /// <returns></returns>
        public static double ToDouble(object strvalue)
        {
            double doubleVal = 0;
            if (strvalue != null) double.TryParse(strvalue.ToString(), out doubleVal);
            return doubleVal;
        }
        public static decimal ToDecimal(object strvalue)
        {
            decimal decimalVal = 0;
            if (strvalue != null) decimal.TryParse(strvalue.ToString(), out decimalVal);
            return decimalVal;
        }

        /// <summary>
        /// 取单位前的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 带百分号的字符串转换为数值
        /// </summary>
        /// <param name="strvalue"></param>
        /// <returns></returns>
        public static double PercentageToDouble(object input)
        {
            double value = 0;
            string strInput = input.ToString();
            if (strInput.EndsWith("%") || strInput.EndsWith("％"))
            {
                double.TryParse(strInput.Remove(strInput.Length - 1), out value);
                value /= 100;
            }
            else
            {
                double.TryParse(strInput, out value);
            }

            return value;
        }

        /// <summary>
        /// 判断input是否为数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumber(object input)
        {
            double temp;
            return double.TryParse(input.ToString(), out temp);
        }

        /// <summary>
        /// 改进的四舍五入
        /// </summary>
        /// <param name="val"></param>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static double Round(double val, int dec)
        {
            if (double.IsNaN(val)) return double.NaN;
            double ret = (double)Math.Round((decimal)val, dec, MidpointRounding.AwayFromZero);//采用中国式四舍五入 算法
            while (ret == 0 && val != 0 && dec < 15)
            {
                ret = Math.Round(val, ++dec);
            }
            return ret;
        }
        /// <summary>
        /// type=0 为工程量，不填充
        /// </summary>
        /// <param name="val"></param>
        /// <param name="dec"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Round2(double val, int dec, int type)
        {
            double ret = Math.Round(val, dec);

            while (ret == 0 && val != 0)
            {
                if (dec > 6)
                {
                    dec = 2;
                    break;
                }
                else
                {
                    ret = Math.Round(val, ++dec);
                }
            }
            if (type == 0)
                return ret.ToString();
            else
            {
                string value = ret.ToString("f" + dec.ToString());
                if (string.Equals(rptParamDic["美元输出"], "True", StringComparison.OrdinalIgnoreCase))
                {
                    int endIndex = value.Contains(".") ? value.LastIndexOf(".") : value.Length;
                    for (int i = endIndex - 3; i > 0; i = i - 3)
                    {
                        value = value.Insert(i, ",");
                    }
                }
                return value;
            }
        }
        public static object Round(object input, int dec)
        {
            object value = string.Empty;
            double temp = 0;
            if (double.TryParse(input.ToString(), out temp))
            {
                value = temp.ToString("f" + dec.ToString());
            }
            return value;
        }

        /// <summary>
        /// (列)arrcol里数据为0的单元格替换成string.Empty
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="arrCol"></param>
        public static void ClearZeroCell(DataTable dt, string[] arrCol)
        {
            DataRow[] drsBusiness = dt.Select("rowgroup >=0 and rowgroup <2000");
            foreach (DataRow drBusiness in drsBusiness)
            {
                foreach (string strCol in arrCol)
                {
                    if (dt.Columns.Contains(strCol) && ToDouble(drBusiness[strCol]) == 0.00)
                    {
                        drBusiness[strCol] = string.Empty;
                    }
                }
            }
        }
        /// <summary>
        /// 费用计算式中常用的操作符号 ,可用于进行变量替换{0}CommUsedOperator
        /// </summary>
        public static string CommUsedOperator = @"[\+\-\*/＋－×÷／\(\)（）]";

        /// <summary>
        /// 取费用基础的正则表达式 (@)*xx费率 or @*xx费率
        /// [\(（]?(?<xxx>.*?)[\)）]?[×\*].*费率
        /// (?<=[\(（]).*(?=[）\)]+[\*×].*率)|.*[^)）](?=[\*×].*率)
        /// </summary>
        public static string BaseFeesRegular = @"(?<=[\(（]).*(?=[）\)]+[\*×].*率)|.*[^)）](?=[\*×].*率)|(?<=[\(（]).*(?=[）\)]+[\*×].*系数)|.*[^)）](?=[\*×].*系数)";
        public static string FeesNameRegular = @"[^\*×]+率";

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
                    exp = exp.Replace(key, VarValDict[key]);
                }
            }
            return exp;
        }
  
        /// <summary>
        /// 两数相加
        /// </summary>        
        public static object AddOpt(object objA, object objB)
        {
            if (objA.ToString().Length == 0)
            {
                return objB;
            }
            if (objB.ToString().Length == 0)
            {
                return objA;
            }

            return ToDecimal(objA) + ToDecimal(objB);
        }
        public static object DecOpt(object objA, object objB)
        {
            if (objB.ToString().Length == 0)
            {
                return objA;
            }

            return ToDecimal(objA) - ToDecimal(objB);
        }
        public static object MulOpt(object objA, object objB)
        {
            if (objA.ToString().Length == 0)
            {
                return "";
            }
            if (objB.ToString().Length == 0)
            {
                return "";
            }
            return ToDecimal(objA) * ToDecimal(objB);
        }
        public static object DivOpt(object objA, object objB)
        {
            if (objA.ToString().Length == 0)
            {
                return "";
            }
            decimal d = ToDecimal(objB);
            if (d == 0)
            {
                return "";
            }
            else
            {
                return ToDecimal(objA) / d;
            }
        }

        /// <summary>
        /// 连乘
        /// </summary>        
        public static object Mult(params object[] objs)
        {
            double value = 1;
            foreach (object obj in objs)
            {
                double tmp = 0.0;
                if (double.TryParse(obj == null ? "" : obj.ToString(), out tmp))
                {
                    value *= tmp;
                }
                else
                {
                    return string.Empty;
                }
            }

            return value;
        }
        /// <summary>
        /// 累加
        /// </summary>        
        public static object Sum(params object[] objs)
        {
            double value = 0;
            int maxDecimalPos = 0;//最大小数位数
            bool isAllEmpty = true;//是否全为空，都为空结果为空
            foreach (object obj in objs)
            {
                maxDecimalPos = Math.Max(maxDecimalPos, GetDecimalPos(obj));//最大小数位数
                if (obj != null && !string.IsNullOrEmpty(obj.ToString().Trim())) isAllEmpty = false;
                value += ToDouble(obj);
            }
            if (isAllEmpty) return string.Empty;

            return value.ToString(string.Format("f{0}", maxDecimalPos));
        }
        /// <summary>
        /// 获取小数位数
        /// </summary>
        /// <param name="input">输入值</param>
        /// <returns>小数位数</returns>
        private static int GetDecimalPos(object input)
        {
            if (input == null) return 0;
            Match match = Regex.Match(input.ToString().Trim(), @"\d+\.(\d+)");
            if (!match.Success) return 0;
            return match.Groups[1].Value.Length;
        }

        /// <summary>
        /// 确定工程的费用计算基础，用于定额借用识别
        /// </summary>
        /// <param name="dsProject"></param>
        /// <param name="gc_chandle"></param>
        /// <returns></returns>
        public static string GetFeeBase(DataSet dsProject, string gc_chandle)
        {
            //工程费用表计算基础
            if (!string.IsNullOrEmpty(gc_chandle))
            {
                DataRow[] drs = dsProject.Tables["GcConfig"].Select("gc_chandle = '" + gc_chandle + "' and ConfigID = 'DefaultQuotaID'");
                if (drs.Length > 0)
                {
                    return drs[0]["ConfigValue"].ToString();
                }
            }
            return "";
        }
        public static DataRow AddMacro(DataTable dt, string macroName, string macroValue)
        {
            DataRow dr = dt.NewRow();
            dr["rowGroup"] = -3;
            dr["macroName"] = macroName;
            dr["macroValue"] = macroValue;
            dt.Rows.Add(dr);
            return dr;
        }
        /// <summary>
        /// 根据代数运算式求值
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        private static DataTable dtCompute = new DataTable();
        public static Double Compute(string formula)
        {
            double result = 0;
            try
            {
                formula = Regex.Replace(formula, @"[{｛][^{｛]*[}｝]", "");
                formula = Regex.Replace(formula, "[%％]", "/100");
                string src = "＋－×÷／（）", dst = "+-*//()";
                for (int i = 0; i < src.Length; i++)
                {
                    formula = formula.Replace(src[i], dst[i]);
                }
                double.TryParse(dtCompute.Compute(formula, "").ToString(), out result);
            }
            catch
            {
                result = double.NaN;
            }
            return result;
        }
    }
}
