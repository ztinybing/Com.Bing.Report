using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Com.Bing.Report
{
    /// <summary>
    /// 该类不属于报表的属性类，只是用于记录数据表中应该打印在一起行
    /// 因此将该类取名为分组
    /// </summary> 
    public class Groups:List<Group>
    {
        public Groups() { }
        public bool Contains(DataRow row)
        {
            bool isContains = false;
            foreach (Group g in this)
            {
                isContains = g.ContainsRow(row);
            }
            return isContains;
        }
        public int FindPreRowGroup(DataRow row )
        {
            int findIndex = -1;
            foreach (Group g in this)
            {
                //可能会引影效率
                if(g.EndRow.Table.Rows.IndexOf(g.EndRow)+1 == row.Table.Rows.IndexOf(row))
                findIndex++;                
            }
            return findIndex;
        }
        public void Add(DataRow tRow)
        {
            int index = this.FindPreRowGroup(tRow);
            if (index == -1)
            {
                this.Add(new Group(tRow, tRow));
            }
            else
            {
                this[index].EndRow =tRow;
            }
        }
    }
    public class Group
    {
        List<DataRow> groupContainRow = new List<DataRow>();
        private DataRow startRow = null;
        public DataRow StartRow
        {
            get { return startRow; }
            set { startRow = value; AddGroupRow(value); }
        }
        private DataRow endRow = null;
        public DataRow EndRow
        {
            get { return endRow; }
            set { endRow = value; AddGroupRow(value); }
        }

        public Group() { }
        public Group(DataRow startRow ,DataRow endRow)
        {
            this.startRow = startRow;
            this.endRow = endRow;
            AddGroupRow(startRow);
            AddGroupRow(endRow);
        }
        private void AddGroupRow(DataRow row)
        {
            if (!groupContainRow.Contains(row))
            {
                groupContainRow.Add(row);
            }
        }
        public bool ContainsRow(DataRow row)
        {
            return groupContainRow.Contains(row);
        }
    }
}
