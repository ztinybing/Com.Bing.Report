using System;
using System.Collections.Generic;
using System.Text;
 
namespace Com.Bing.Report
{
    /// <summary>
    /// 
    /// 记录报表的打印样式
    /// 
    /// </summary>
    public class PrintStyle
    {
        //表脚是否最后一页打印
        private bool isPrintInLast;

        public bool IsPrintInLast
        {
            get { return isPrintInLast; }
            set { isPrintInLast = value; }
        }
        private bool isBrifeStyle;

        public bool IsBrifeStyle
        {
            get { return isBrifeStyle; }
            set { isBrifeStyle = value; }
        }
        private bool noDataNoPrint;

        public bool NoDataNoPrint
        {
            get { return noDataNoPrint; }
            set { noDataNoPrint = value; }
        }
        private bool noPrint;

        public bool NoPrint
        {
            get { return noPrint; }
            set { noPrint = value; }
        }
    }
}
