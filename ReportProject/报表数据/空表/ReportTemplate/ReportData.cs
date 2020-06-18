using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace Com.Bing
{
    public class ReportData : BaseReportData
    {
        public static DataTable SqliteTable(DataSet ds, string[] s)
        {
            return new ReportData().InitTable(ds, s);
        }

        public override string[] GetColumns()
        {
            return new string[] { "col1", "col2", "col3", "col4", "col5", "col6" };
        }

        public override DataTable GetBusinessData(DataSet ds, DataTable dt)
        {
            AddMacro(dt, "名称", "大法师");
            AddMacro(dt, "单位", "m");
            AddRow(dt, 5);
            return dt;
        }

        private void AddRow(DataTable dt, int rowCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                DataRow row = dt.NewRow();
                row[ROWGROUP] = 1;
                row["col1"] = new string(i.ToString()[0], 2);
                row["col2"] = new string(i.ToString()[0], 3);
                row["col3"] = new string(i.ToString()[0], 4);
                row["col4"] = new string(i.ToString()[0], 5);
                row["col5"] = new string(i.ToString()[0], 6);
                row["col6"] = new string(i.ToString()[0], 7);
                dt.Rows.Add(row);
            }
        }
    }
}

