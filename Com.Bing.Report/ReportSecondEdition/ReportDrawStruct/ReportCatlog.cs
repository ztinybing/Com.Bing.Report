using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Com.Bing.Report
{
    /// <summary>
    /// 处理批量报表目录问题
    /// </summary>
    public class ReportCatlog
    {
        Report report = null;
        DataTable catlogTable = new DataTable();
        public ReportCatlog()
        {
            report = new Report();
            report.ReadRPT("目录.rpt");
            InitTableColumn();
        }
        private void InitTableColumn()
        {
            catlogTable.Columns.Add("rowgroup");
            foreach (ReportColumn column in report.Columns)
            {
                catlogTable.Columns.Add(column.ColumnName);
            }
            catlogTable.Columns.Add("macroName");
            catlogTable.Columns.Add("macroValue");
        }
        internal void Clear()
        {
            catlogTable.Clear();       
        }
        internal void Add(string rptName, int rptPage)
        {
            catlogTable.Rows.Add(new object[] { 1, rptName, rptPage });
        }
        internal void Print(string flag,string xlsFile)
        {
            InitReportData();

            if (flag == "send")
            {
                report.SendToExcelAsPart();
            }
            else
            {
                report.Print();
            }
        }

        private void InitReportData()
        {
            catlogTable.Rows.Add(new object[] { -1 });
            ReportData reportData = new ReportData(report);
            reportData.InitReportData(catlogTable);

            report.InitReportData(reportData);
        }
    }
}
