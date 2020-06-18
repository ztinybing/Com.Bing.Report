using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Com.Bing.API;
using System.IO;
using System.Drawing;
using Com.Bing.Business;
namespace Com.Bing.Report
{
    /// <summary>
    ///需要结合现有的结构做如下实现
    /// </summary>
    public class SqliteData
    {
        private Report report = null;
        public SqliteData(Report report)
        {
            this.report = report;
        }
        public Dictionary<string, string> GetReportArgs(string rptFile)
        {
            Dictionary<string, string> dic = null;
            return dic;
        }
        public ReportData GetData(object projectData)
        {
            return GetData(projectData as DataSet, null);
        }
        public string[] GetRptParam(object projectData, Dictionary<string, object> exParams)
        {
            Dictionary<string, string> rptParams = report.GetParam();
            List<string> sarray = new List<string>();
            foreach (KeyValuePair<string, string> entry in rptParams)
            {
                sarray.Add(string.Format("{0}={1}", entry.Key, entry.Value));
            }

            if (projectData is StorageDataSet)
            {
                StorageDataSet sDataSet = projectData as StorageDataSet;
 
                foreach (KeyValuePair<string, object> entry in sDataSet.Config)
                {
                    sarray.Add(string.Format("{0}={1}", entry.Key, entry.Value));
                }
            }

            if (exParams != null)
            {
                foreach (string key in exParams.Keys)
                {
                    sarray.Add(string.Format("{0}={1}", key, exParams[key]));
                }
            }
            return sarray.ToArray();
        }
        internal ReportData GetData(DataSet projectData, Dictionary<string, object> exParams)
        {
            if (File.Exists(report.DllPath))
            {
                DataTable dataTable = Function.Invoke(report.DllPath, "Com.Bing",
                                                 "ReportData", "SqliteTable", new object[] { projectData, GetRptParam(projectData, exParams) }) as DataTable;												 
                if (report.IsDoc)
                {
					//加入报表名称的宏变量
                    DataRow newRow = dataTable.NewRow();
                    newRow["rowGroup"] = -3;
                    newRow["macroName"] = "报表名称";
                    newRow["macroValue"] = report.DesrcName;
                    dataTable.Rows.Add(newRow);
                }
                report.OriginalData = dataTable;

                ReportData reportData = new ReportData(report);
                reportData.InitReportData(dataTable);
                return reportData;
            }
            return null;
        }
    }
}
