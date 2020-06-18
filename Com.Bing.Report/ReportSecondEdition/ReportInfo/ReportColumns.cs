using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{
    public class ReportColumns : List<ReportColumn>
    {
        private int columnCount = 0;
        public int ColumnCount
        {
            get { return columnCount; }
            set { columnCount = value; }
        }
        private Report report = null;
        public ReportColumns(Report report)
        {
            this.report = report;
        }
        //重载名称索引器
        public ReportColumn this[string colName]
        {
            get
            {
                return Find(colName);
            }
        }
        public ReportColumn Find(string colName)
        {
            if (speedBuffer.ContainsKey(colName))
            {
                return speedBuffer[colName].Key;
            }
            return null;
        }

        public int FindIndex(string colName)
        {
            if (speedBuffer.ContainsKey(colName))
            {
                return speedBuffer[colName].Value;
            }
            else
            {
                return -1;
            }
        }


        public int FindIndex(ReportColumn col)
        {
            ReportColumn c;
            for (int i = 0; i < this.Count; i++)
            {
                c = this[i];
                if (c.ColumnName == col.ColumnName)
                    return i + 1;
            }
            return 0;
        }
        private Dictionary<string, KeyValuePair<ReportColumn, int>> speedBuffer =
                new Dictionary<string, KeyValuePair<ReportColumn, int>>();

        public new void Add(ReportColumn col)
        {

            base.Add(col);
            if (speedBuffer.ContainsKey(col.ColumnName))
            {
                //RecordError LB
            }
            speedBuffer[col.ColumnName] = new KeyValuePair<ReportColumn, int>(col, this.Count - 1);

        }
        public void SetColumnName(ReportColumn col, string newName)
        {
            string oldColumnName = col.ColumnName;
            base[base.IndexOf(col)].ColumnName = newName;
            if (speedBuffer.ContainsKey(oldColumnName))
            {
                speedBuffer.Remove(oldColumnName);
                speedBuffer[col.ColumnName] = new KeyValuePair<ReportColumn, int>(col, this.Count - 1);
            }
        }
        public new void Insert(int index, ReportColumn col)
        {
            base.Insert(index, col);
            if (speedBuffer.ContainsKey(col.ColumnName))
            {
                //RecordError LB
            }
            speedBuffer[col.ColumnName] = new KeyValuePair<ReportColumn, int>(col, this.Count - 1);
        }
        /// <summary>
        /// 通过报表列信息加添加报表列
        /// </summary>        
        /// <example>
        /// 列1 名称,宽度,列属性（属性取值为FF 字体、FN 大小、B 加粗、I 斜体、U 下划、
        ///S 中划、FC 前景色、BC 背景色、HA 横对齐、VA 纵对齐、上边框线、下边框线、左边框
        ///线、右边框线、自动换行、无数据不打印、固定不打印、自动调整宽度、对角线、X 偏移、
        ///列中文含义）
        /// d0030001=col1,65,,,,,,,,16777215,1,2,1,1,1,1,0,0,0,1,0,0
        /// </example>
        public void Add(string key, string columnInfo)
        {
            string[] tempArray = columnInfo.Split(',');
            if (tempArray.Length < 2)
            {
                throw new ColumnInfoLostException(key, columnInfo);
            }
            ReportColumn column = new ReportColumn(tempArray[0].Trim());
            column.ColumnWidth = int.Parse(tempArray[1]);
            int lenght = tempArray.Length - 2;
            string[] columnPropertyArray = new string[TextAttibute.PROPERTYCOUNT];

            for (int i = 0; i < columnPropertyArray.Length; i++)
            {
                if (lenght > i)
                {
                    //+2 去掉列的名称及线宽信息
                    columnPropertyArray[i] = tempArray[i + 2];
                }
                else
                {
                    //属性值缺失的用空字串补齐
                    columnPropertyArray[i] = "";
                }
            }
            column.Attibutes.Add(columnPropertyArray, report);
            Add(column);
        }
        public int IndexOf(string columnName)
        {
            int findIndex = -1;
            for (int index = 0; index < Count; index++)
            {
                if (this[index].ColumnName == columnName)
                {
                    findIndex = index;
                    break;
                }
            }
            return findIndex;
        }
        public new void Clear()
        {
            base.Clear();
            speedBuffer.Clear();
        }
        public override string ToString()
        {
            StringBuilder columnSection = new StringBuilder();
            columnSection.Append("d003=");
            columnSection.Append(Count);
            columnSection.AppendLine();
            for (int index = 0; index < Count; index++)
            {
                columnSection.Append(string.Format("d003{0:D4}=", index + 1));
                columnSection.AppendLine(this[index].ToString());
            }
            return columnSection.ToString();
        }
        public int[] GetColumnsWidth(int[] usedColumns)
        {
            List<int> tempWidths = new List<int>();
            int index = 1;
            foreach (ReportColumn column in this)
            {
                if (usedColumns == null || usedColumns[index] != 0)
                {
                    tempWidths.Add(column.ColumnWidth);
                }
                index++;
            }
            return tempWidths.ToArray();
        }


    }
}
