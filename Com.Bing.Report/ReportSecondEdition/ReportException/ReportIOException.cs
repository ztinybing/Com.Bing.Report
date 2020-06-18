using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{
    public class ReportIOException : Exception
    { }
    public class FormulaWrongException : ReportIOException
    {
        string errContext = string.Empty;
        public override string Message
        {
            get
            {
                return string.Format("报表Text的公式格式不正确，期望格式为$fun:函数名:列名$ ,而实际值为:{0}", errContext);
            }
        }
        public FormulaWrongException(string err)
        {
            this.errContext = err;
        }
    }
    public class ReportFileNotExistsException : ReportIOException
    {
        string filePath = "";
        public override string Message
        {
            get
            {
                return string.Format("报表文件不存在!\r\n{0}",filePath);
            }
        }
        public ReportFileNotExistsException(string filePath)
        {
            this.filePath = filePath;
        }
    }
    public class ReportPropertyFormatException : ReportIOException
    {
        string errContext = string.Empty;
        public override string Message
        {
            get
            {
                return string.Format("报表属性格式不正确!\r\n{0}",errContext);
            }
        }
        public ReportPropertyFormatException(string err)
        {
            this.errContext = err;
        }
    }
    public class ReportPropertyKeyException : ReportIOException
    {
        string errContext = string.Empty;
        public override string Message
        {
            get
            {
                return string.Format("报表属性键长度不正确!\r\n{0}",errContext);
            }
        }
        public ReportPropertyKeyException(string err)
        {
            errContext = err;
        }

    }
    public class ReportTypeIDInvalidException : ReportIOException
    {
        string errContext = string.Empty;
        public override string Message
        {
            get
            {
                return string.Format("报表类型值无效!\r\n{0}",errContext);
            }
        }
        public ReportTypeIDInvalidException(string err)
        {
            errContext = err;
        }
    }
    public class ReportWholePropertyNoLengthException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "报表整体属性缺失！";
            }
        }
    }
    public class ReportMarginInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "报表边距数字不正确！";
            }
        }
    }
    public class ReportGrainDirectionInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "报表纸张方向设置数值出错！";
            }
        }
    }
    public class ReportRowMultiHeigthInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "报表行高倍数数值无效!";
            }
        }
    }
    public class ReportLandscapePagingInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "横向分页固定列设置出错!";
            }
        }
    }
    public class FontSizeInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "字体大小设置出错!";
            }
        }
    }
    public class ReportLineWidthInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "表格线宽设置无效!";
            }
        }
    }
    public class ColumnInfoLostException : ReportIOException
    {
        string info = "报表列信息缺失";
        public override string Message
        {
            get
            {
                string message = string.Empty;
                if (Data.Count == 0)
                {
                    message = info;
                }
                else
                {
                    foreach (object key in Data.Keys)
                    {
                        message += string.Format("{2},相关信息:\r\n{0}={1}", key, Data[key], info);
                    }
                }
                return message;
            }
        }
        public ColumnInfoLostException(string key, string value)
        {
            this.Data.Add(key, value);
        }
    }
    public class ReportBandEchoRowInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "Band区行显示设置无效!";
            }
        }
    }
    public class ReportEchoColumnCountInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "列显示行数设置无效!";
            }
        }
    }
    public class TextCountInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "TEXT Count设置无效!";
            }
        }
    }
    public class ReportBandNumInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "Band区行数设置无效！";
            }
        }
    }
    public class TextInfoLostException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "文本(TEXT)信息缺失";
            }
        }
    }
    public class TextBandIndexInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "文本所在Band区信息无效!";
            }
        }
    }
    public class ReportColorValInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "字体颜色信息无效!";
            }
        }
    }
    public class TextLocationInvalidException : ReportIOException
    {
        public override string Message
        {
            get
            {
                return "文本位置信息无效!";
            }
        }
    }
}
