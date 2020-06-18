using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Com.Bing.Report
{
    public class BodyDataStruct
    {
        private object tag = string.Empty;
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        private static BodyDataStruct empty = new BodyDataStruct("空");
        /// <summary>
        /// 等同于 null ,表格线也不画
        /// </summary>        
        public static BodyDataStruct Empty
        {
            get { return empty; }
        }
        /// <summary>
        /// 本页小计标识符
        /// </summary>
        private bool isFunCell = true;
        public bool IsFunCell
        {
            get { return isFunCell; }
            set { isFunCell = value; }
        }
        string context = "";
        public string Context
        {
            get { return context; }
            set { context = value; }
        }
        RowMergeInfo rowMergeInfo = new RowMergeInfo(1);
        public RowMergeInfo RowMergeInfo
        {
            get { return rowMergeInfo; }
            set { rowMergeInfo = value; }
        }
        ColMergeInfo colMergeInfo = new ColMergeInfo(1);
        public ColMergeInfo ColMergeInfo
        {
            get { return colMergeInfo; }
            set { colMergeInfo = value; }
        }
        public BodyDataStruct() { }
        public BodyDataStruct(string context) { this.context = context; }
        public BodyDataStruct(string contxt, int mergeColCount)
            : this(contxt)
        {

            colMergeInfo = new ColMergeInfo(mergeColCount);
        }
        public BodyDataStruct(string context, int mergeColCount, int mergeRowCount)
            : this(context, mergeColCount)
        {
            rowMergeInfo = new RowMergeInfo(mergeRowCount);
        }
        public override string ToString()
        {
            //return context + "【" + colMergeInfo.MergeCount + "," + rowMergeInfo.RowMergeCount + "】";
            return context;
        }
        public static bool IsEmptyOrNull(object obj)
        {
            if (obj == Empty || obj == null || obj == DBNull.Value)
            {
                return false;
            }
            return true;
        }
    }
    public class RowMergeInfo
    {
        //向下合并的行数
        int rowMergeCount = 0;
        public int RowMergeCount
        {
            get { return rowMergeCount; }
            set { rowMergeCount = value; }
        }
        //合并后上下左右的边框
        BoundaryLine boundaryLine = new BoundaryLine();
        public BoundaryLine BoundaryLine
        {
            get { return boundaryLine; }
            set { boundaryLine = value; }
        }
        public RowMergeInfo(int rowMergeCount)
        {
            this.rowMergeCount = rowMergeCount;
        }
        public RowMergeInfo(int rowMergeCount, bool up, bool down, bool left, bool right)
            : this(rowMergeCount)
        {
            boundaryLine.UpperBoundaryLine = up;
            boundaryLine.LowerBoundaryLine = down;
            boundaryLine.LeftBoundaryLine = left;
            boundaryLine.RightBooundaryLine = right;
        }
    }
    public class ColMergeInfo
    {
        private int mergeCount = 0;
        public int MergeCount
        {
            get { return mergeCount; }
            set { mergeCount = value; }
        }
        //居中 居左的
        private StringAlignment alignment = StringAlignment.Center;
        public StringAlignment Alignment
        {
            get { return alignment; }
            set { alignment = value; }
        }
        private StringAlignment lineAlignment = StringAlignment.Center;
        public StringAlignment LineAlignment
        {
            get { return lineAlignment; }
            set { lineAlignment = value; }
        }
        private Font font = null;
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
        public ColMergeInfo() { }
        public ColMergeInfo(int mergeCount)
        {
            this.mergeCount = mergeCount;
        }
        public ColMergeInfo(int mergeCount, Font font)
            : this(mergeCount)
        {
            this.font = font;
        }
    }
}
