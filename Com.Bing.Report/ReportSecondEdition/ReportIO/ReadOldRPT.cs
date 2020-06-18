using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Windows.Forms;
using System.Text.RegularExpressions;
using Com.Bing.API;


namespace Com.Bing.Report
{
    /// <summary>
    /// 读取报表信息
    /// </summary>
    public class ReadOldRPT
    {
        //报表设计起始标识
        public const string DESIGNNODE = "[DESIGN]";
        //报表参数起始标识
        public const string PARAMETERNODE = "[PARAMETER]";
        public const string SOURCENODE = "[SOURCE]";

        private StreamReader reader = null;
        private Report report = null;
        private Report Report
        {
            set { report = value; }
        }
        internal List<string> Contextlist = new List<string>();

        public ReadOldRPT(Report report)
        {
            this.report = report;
        }
        /// <summary>
        /// 从报表文件读取Report信息
        /// </summary>
        public void Read()
        {
            ReadUsefulData();
            readToReport();
        }
        private void ReadUsefulData()
        {
            if (report.RptFilePath == "")
            {
                return;
            }
            if (!File.Exists(report.RptFilePath))
            {
                throw new ReportFileNotExistsException(report.RptFilePath);
            }
            this.reader = new StreamReader(report.RptFilePath, Encoding.Default);
            Contextlist.Clear();
            //不读取[DATA][/DATA]、[SQL][/SQL]、[SQLSYNTAX][/SQLSYNTAX]、[DESCRIBE][/DESCRIBE]
            string tempLine = string.Empty;
            bool usedLine = true;

            try
            {
                while (!reader.EndOfStream)
                {
                    tempLine = reader.ReadLine();
                    if (Regex.IsMatch(tempLine, @"\[((DATA)|(SQL)|(SQLSYNTAX)|(DESCRIBE))\]"))
                    {
                        usedLine = false;
                        continue;
                    }
                    if (Regex.IsMatch(tempLine, @"\[\/((DATA)|(SQL)|(SQLSYNTAX)|(DESCRIBE))\]"))
                    {
                        usedLine = true;
                        continue;
                    }
                    if (usedLine)
                    {
                        Contextlist.Add(tempLine);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        private void readToReport()
        {
            //[PARAMETER] [DESIGN]
            bool isPropertyString = false;
            foreach (string context in Contextlist)
            {
                if (context.Trim().Equals(DESIGNNODE))
                {
                    isPropertyString = true;
                    continue;
                }
                if (context.Trim().Equals(PARAMETERNODE))
                {
                    isPropertyString = false;
                    continue;
                }
                if (context.Trim().Equals(SOURCENODE))
                {
                    isPropertyString = false;
                    continue;
                }
                if (isPropertyString && !context.Trim().Equals(string.Empty))
                {
                    addProperty(context);
                }
            }
        }
        private void addProperty(string context)
        {
            //如d0001=xxx
            int spliteIndex = context.IndexOf("=");
            if (spliteIndex <= 0)
            {
                throw new ReportPropertyFormatException(context);
            }
            //段标识
            string key = context.Substring(0, spliteIndex);
            if (key.Length < 3)
            {
                throw new ReportPropertyKeyException(context);
            }
            string val = context.Remove(0, spliteIndex + 1);
            //寻找属性字段
            switch (int.Parse(key.Substring(1, 3)))
            {
                case 1:
                    if (key.Length <= 4)
                    {
                        //报表类型1:自由风格报表 0：网格风格报表                        
                        //d001=0|1 
                        if (val == "3")
                            val = "1";
                        report.RptClass = ReportClass.GetClass(val);
                    }
                    else
                    {
                        //报表整体属性
                        //d001001=报表名称,上边界,下边界,左边界,右边界,纸张方向,行高,横向分页设置,缺省省字体大小,表脚是每页都打印还是只在最后页打印,是否简洁报表样式,表格线宽度
                        report.Attributes.Add(val);
                    }
                    break;
                case 2:
                    //该段无用，直接丢弃
                    break;
                case 3:
                    //报表列相关
                    if (key.Length > 4)
                    {
                        report.Columns.Add(key, val);
                    }
                    break;
                case 4:
                    //Band区相关
                    if (key.Length == 12)
                    {
                        report.Bands.SetBindRowEcho(key, val);
                    }
                    else if (key.Length == 8)
                    {
                        report.Bands.SetBindRowNum(key, val);
                    }
                    break;
                case 5:
                    if (key.Length == 8)
                    {
                        report.Texts.Add(val);
                    }
                    else if (key.Length == 4)
                    {
                        int textCount = 0;
                        if (!int.TryParse(val, out textCount))
                        {
                            throw new TextCountInvalidException();
                        }
                        report.Texts.Capacity = textCount;
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取报表参数信息
        /// </summary>        
        public List<string> RptExInfo()
        {
            int exInfoIndex = 0;

            int parameterNode = Contextlist.IndexOf(PARAMETERNODE);
            int sourceNode = Contextlist.IndexOf(SOURCENODE);
            if (parameterNode == -1)
                exInfoIndex = sourceNode;
            else if (sourceNode == -1)
                exInfoIndex = parameterNode;
            else
                exInfoIndex = parameterNode < sourceNode ? parameterNode : sourceNode;

            if (exInfoIndex > 0)
            {
                return Contextlist.GetRange(exInfoIndex, Contextlist.Count - exInfoIndex);
            }
            else
            {
                return new List<string>();
            }
        }
    }
}
