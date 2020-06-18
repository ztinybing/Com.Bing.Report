using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{
    public class ReportColumn
    {
        private string columnName = "";
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }
        public bool ColumnVisible
        {
            get { return !attibutes.PrintStyle.NoPrint && !attibutes.PrintStyle.NoDataNoPrint; }
        }
        //设计宽度
        private int columnWidth = 0;
        public int ColumnWidth
        {
            get { return columnWidth; }
            set { columnWidth = value; }
        }
        //实际宽度
        private TextAttibute attibutes = new TextAttibute();
        public TextAttibute Attibutes
        {
            get { return attibutes; }
            set { attibutes = value; }
        }
        public ReportColumn() { }
        public ReportColumn(string columnName)
        {
            this.columnName = columnName;
        }
        internal ReportColumn Clone()
        {
            ReportColumn column = new ReportColumn(columnName);
            column.ColumnWidth = columnWidth;
            column.Attibutes = attibutes.Clone();
            return column;
        }
        public override string ToString()
        {
            StringBuilder columnStr = new StringBuilder();
            columnStr.Append(columnName);
            columnStr.Append(",");
            columnStr.Append(columnWidth);
            columnStr.Append(",");
            columnStr.Append(attibutes.ToString());
            return columnStr.ToString();
        }
    }

}
