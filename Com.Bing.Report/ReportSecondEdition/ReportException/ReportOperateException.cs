using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{
    public class ReportOperateException:Exception
    {
        public override string Message
        {
            get
            {
                return info + base.Message;
            }
        }
        protected string info = "";
        public ReportOperateException(string err)
        {
            this.info = err;
        }
    }

    public class SendToExcelException:ReportOperateException
    {
        public SendToExcelException(string err)
            : base("电子表格组件调用失败!\r\n" +err)
        {

        }
    }
    public class ExcelTemplateLostException : ReportOperateException
    {
        public ExcelTemplateLostException():base("电子表格的模板文件缺失!")
        {

        }
    }
}
