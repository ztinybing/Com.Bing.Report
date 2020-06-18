using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization;
using Com.Bing.Report;

namespace Com.Bing.Business
{
    [Serializable]
    public class StorageDataSet : DataSet
    {
        public StorageDataSet()
            : base()
        { }
        public StorageDataSet(SerializationInfo info, StreamingContext ctxt):base(info,ctxt)
        {

        }
        public StorageDataSet(SerializationInfo info, StreamingContext ctxt, bool construct)
            : base(info, ctxt, construct)
        {
 
        }
        [NonSerialized]
        private Dictionary<string, object> config = new Dictionary<string, object>();
        public Dictionary<string, object> Config
        {
            get
            {
                return config;
            }
        }
        [NonSerialized]
        private bool modifyCheck = false;
        public bool ModifyCheck
        {
            set
            {
                modifyCheck = value;
                if (modifyCheck)
                {
                    foreach (DataTable dt in this.Tables)
                    {
                        dt.TableNewRow += dt_Event;
                        dt.RowChanged += dt_Event;
                        dt.RowDeleted += dt_Event;
                    }
                }
                else
                {
                    foreach (DataTable dt in this.Tables)
                    {
                        dt.TableNewRow -= dt_Event;
                        dt.RowChanged -= dt_Event;
                        dt.RowDeleted -= dt_Event;
                    }
                }
            }
        }

        [NonSerialized]
        public EventHandler<EventArgs> Modified = null;
        void dt_Event(object sender, EventArgs e)
        {
            isModified = true;
            if (Modified != null)
            {
                Modified(sender, e);
            }
        }
        [NonSerialized]
        private bool isModified = false;
        public  bool IsModified
        {
            set
            {
                if (value == false)
                {
                    isModified = value;
                }
            }
            get
            {
                return isModified;
            }
        }
        /*
        /// <summary>
        /// 以当前数据预览特定报表
        /// </summary>        
        public void PreviewRpt(string rptName, string rptFile, Dictionary<string, object> rptParams)
        {
            ReportCenter reportCenter =  GetReportCenter(rptName, rptFile, ref rptParams);
            PreviewReport previewReport = new PreviewReport(reportCenter);
            previewReport.Show();
        }

        private ReportCenter GetReportCenter(string rptName, string rptFile, ref Dictionary<string, object> rptParams)
        {
            ReportCenter reportCenter = new ReportCenter(rptName, PrintReportControl.GetRptFilePathBy(rptFile));
            //定制reportCenter的相关参数 当前勾选工程等问题
            if (rptParams == null)
            {
                rptParams = new Dictionary<string, object>();
                rptParams.Add("忽略工程节点勾选", "True");
            }
            reportCenter.TempRptParams = rptParams;
            reportCenter.ReportArg = this;
            return reportCenter;
        }

        public void SendRpt(string rptName, string rptFile, Dictionary<string, object> rptParams)
        {
            ReportCenter reportCenter = GetReportCenter(rptName, rptFile, ref rptParams);
            string xlsFilePath = string.Empty;
            if (reportCenter.ShowSaveFile(ref xlsFilePath)== System.Windows.Forms.DialogResult.OK)
            {
                reportCenter.SendToExcel(xlsFilePath);
            }
        }
        */
    }
}
