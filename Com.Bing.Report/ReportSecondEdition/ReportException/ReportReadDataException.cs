using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{
    public class ReportReadDataException:Exception
    {
    }
    public class MutliManagerNullException : ReportReadDataException
    {
        public override string Message
        {
            get
            {
                return "多工程数据管理对象为空!";
            }
        }
    }
    public class InitDesignColumnException : ReportReadDataException
    {
        private Exception err = null;
        public override string Message
        {
            get
            {
                return "在加载报表设计样式时，初始化报表列出错! 详细信息如下：\r\n" + err.Message;
            }
        }
        public InitDesignColumnException(Exception e)
        {
            this.err = e;
        }
    }
}
