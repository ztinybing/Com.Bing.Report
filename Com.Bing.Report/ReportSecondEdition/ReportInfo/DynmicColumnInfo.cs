using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Com.Bing.API;

namespace Com.Bing.Report
{
    public class DynmicColumnInfo
    {
        private DynmicColumns dynmicColumns;
        public DynmicColumns DynmicColumns
        {
            get { return dynmicColumns; }
        }

        private Report currReport;
        public Report CurrReport
        {
            get { return currReport; }
        }
        private ReportData data;
        public DynmicColumnInfo(Report report, DataTable dt, ReportData data)
        {
            this.data = data;
            this.currReport = report;

            InitDynmicColumn(report, dt, data);
            RemoveOriginalDynmicColumn(currReport);
        }

        //记录共享列信息
        private void InitDynmicColumn(Report report, DataTable dt, ReportData data)
        {
            dynmicColumns = new DynmicColumns(report);

            if (dynmicColumns.Count > 0)
            {
                currReport = new Report(report.DesrcName, report.DesrcLevel);
                currReport.RptFilePath = report.RptFilePath;
                currReport.ReadRPT();
                currReport.Data = data;
                currReport.DataValid = true;
                int index = currReport.Columns.IndexOf(dynmicColumns[dynmicColumns.Count - 1].ColName);
                for (int i = dt.Columns.Count - 1; i >= 0; i--)
                {
                    string[] strs = dt.Columns[i].ColumnName.Split('_');
                    if (strs.Length == 2 && Function.IsNumber(strs[1]))
                    {
                        string name = strs[0];

                        DynmicColumn dynmicColumn = dynmicColumns.FindDynmicColumn(name);
                        if (dynmicColumn.Visible)
                        {
                            ReportColumn newReportColumn = report.Columns[name].Clone();
                            newReportColumn.ColumnName = dt.Columns[i].ColumnName;
                            //index = report.Columns.IndexOf(name);
                            currReport.Columns.Insert(index + 1, newReportColumn);
                        }
                    }
                }
            }
        }

        //移除原始的共享列
        private void RemoveOriginalDynmicColumn(Report report)
        {
            //记录不显示的共享列个数
            for (int i = report.Columns.Count - 1; i >= 0; i--)
            {
                if (report.Columns[i].Attibutes.DynamicColumn)
                {
                    string[] strs = report.Columns[i].ColumnName.Split('_');
                    if (strs.Length != 2)
                        report.Columns.RemoveAt(i);
                }
            }
        }

        //获取当前共享列之前隐藏的共享列个数
        public int GetBeforeNotVisibleCount(DynmicColumns dynmicColumns, string columnName)
        {
            int count = 0;
            DynmicColumn dynmicColumn = dynmicColumns.FindDynmicColumn(columnName);
            if (dynmicColumn != null)
            {
                int endIndex = dynmicColumns.IndexOf(dynmicColumn);
                for (int i = 0; i < endIndex; i++)
                {
                    if (!dynmicColumns[i].Visible)
                        count++;
                }
            }
            return count;
        }
        //获取在合并范围内隐藏的共享列个数
        public int GetNotVistbleCount(DynmicColumns dynmicColumns, string columnName, int columnSpan)
        {
            int count = 0;
            DynmicColumn dynmicColumn = dynmicColumns.FindDynmicColumn(columnName);
            if (dynmicColumn != null)
            {
                int startIndex = dynmicColumns.IndexOf(dynmicColumn);
                for (int i = startIndex; i < startIndex + columnSpan; i++)
                {
                    //防止索引超出范围
                    if (dynmicColumns.Count > i && !dynmicColumns[i].Visible)
                        count++;
                }
            }
            return count;

        }
    }

    public class DynmicColumn
    {
        private bool visible;
        public bool Visible
        {
            get { return visible; }
        }
        private string colName;
        public string ColName
        {
            get { return colName; }
        }
        public DynmicColumn(string colName, bool visible)
        {
            this.visible = visible;
            this.colName = colName;
        }
    }
    public class DynmicColumns : List<DynmicColumn>
    {
        //共享列总数
        private int totalColumnCount;
        public int TotalColumnCount
        {
            get { return totalColumnCount; }
        }
        //显示的共享列数
        private int visibleColumnCount;
        public int VisibleColumnCount
        {
            get { return visibleColumnCount; }
        }
        //不显示的共享列数
        public int NotVisibleColumnCount
        {
            get { return totalColumnCount - visibleColumnCount; }
        }

        public DynmicColumns(Report report)
        {
            InitDynmicColumn(report);
        }

        private void InitDynmicColumn(Report report)
        {
            foreach (ReportColumn column in report.Columns)
            {
                //处理含有共享列的情况
                if (column.Attibutes.DynamicColumn)
                {
                    //visible 包含无数据不打印，和固定不打印，共享列无数据不打印推后处理
                    DynmicColumn dynmicColumn = new DynmicColumn(column.ColumnName, !column.Attibutes.PrintStyle.NoPrint);
                    this.Add(dynmicColumn);
                    totalColumnCount++;
                    if (dynmicColumn.Visible)
                        visibleColumnCount++;
                }
            }
        }

        public DynmicColumn FindDynmicColumn(string colName)
        {
            foreach (DynmicColumn dynmicColumn in this)
            {
                if (dynmicColumn.ColName == colName)
                {
                    return dynmicColumn;
                }
            }
            return null;
        }
    }
}
