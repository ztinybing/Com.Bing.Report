using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml;
namespace Com.Bing.Report
{
    public class CellStruct
    {
        public static int echoFlag = 0;
        private static CellStruct empty = new CellStruct("空", null, ExcelInfo.Empty);
        public static CellStruct Empty
        {
            get { return CellStruct.empty; }
        }
        protected string context = string.Empty;
        public string Context
        {
            get { return context; }
            set { context = value; }
        }
        protected DrawInfo drawInfo = null;
        public DrawInfo DrawInfo
        {
            get { return drawInfo; }
            set { drawInfo = value; }
        }
        protected ExcelInfo excelInfo = ExcelInfo.Empty;
        public ExcelInfo ExcelInfo
        {
            get { return excelInfo; }
            set { excelInfo = value; }
        }
        public CellStruct()
        { }
        public CellStruct(string context, DrawInfo drawInfo, ExcelInfo excelInfo)
        {
            this.context = context;
            this.drawInfo = drawInfo;
            this.excelInfo = excelInfo;
        }
        public string ToExcelString(Point excelLocaton)
        {
            excelLocaton.Offset(excelInfo.ExcelPoint);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("10");
            sb.AppendLine("1");
            //sb.AppendLine(string.Format("{0},{1},{2},{3}", excelLocaton.X +1, excelLocaton.Y+1,
            //    excelLocaton.X + excelInfo.RowMerge , excelLocaton.Y + excelInfo.ColMerge ));
            sb.AppendLine(string.Format("{0},{1},{2},{3}", excelLocaton.Y + 1, excelLocaton.X + 1
                 , excelLocaton.Y + excelInfo.ColMerge, excelLocaton.X + excelInfo.RowMerge));
            sb.AppendLine("@");
            //去除文本末尾的换行
            string text = context.Replace("\r", "").TrimEnd('\n').Replace("\n", "Chr(13)+Chr(10)");
            sb.AppendLine(text);

            sb.AppendLine(drawInfo.Format.LineAlignment == StringAlignment.Near ? "2" :
                                        (drawInfo.Format.LineAlignment == StringAlignment.Center ? "3" : "4"));
            sb.AppendLine(drawInfo.Format.Alignment == StringAlignment.Near ?
                "3" : (drawInfo.Format.Alignment == StringAlignment.Center ? "2" : "1"));
            sb.AppendLine(drawInfo.Font.FontFamily.Name);

            sb.AppendLine(Convert.ToInt32(drawInfo.Font.Size).ToString());

            sb.AppendLine("1");
            sb.AppendLine(drawInfo.Font.Bold ? "1" : "0");
            sb.AppendLine(drawInfo.Font.Italic ? "1" : "0");
            sb.AppendLine(drawInfo.Font.Italic ? "1" : "0");
            //sb.AppendLine("0");
            //报表发送特殊线处理
            if (drawInfo.BoundaryLine.IsSlash)
                sb.AppendLine("1");
            else if (drawInfo.BoundaryLine.IsBackSlash)
                sb.AppendLine("2");
            else if (drawInfo.BoundaryLine.IsCrossLine)
                sb.AppendLine("3");
            else
                sb.AppendLine("0");

            sb.AppendLine(drawInfo.BoundaryLine.UpperBoundaryLine ? "1" : "0");
            sb.AppendLine(drawInfo.BoundaryLine.LowerBoundaryLine ? "1" : "0");
            sb.AppendLine(drawInfo.BoundaryLine.LeftBoundaryLine ? "1" : "0");
            sb.AppendLine(drawInfo.BoundaryLine.RightBooundaryLine ? "1" : "0");
            sb.AppendLine("-1");
            sb.AppendLine("-1");
            return sb.ToString();
        }
        public override string ToString()
        {
            if (echoFlag == 1)
            {
                return drawInfo.ToString();
            }
            else if (echoFlag == 2)
            {
                return Context + excelInfo.ToString();
            }
            return context;
            //return context + "Alignment:" + drawInfo.Format.Alignment + ",LineAligment:" + drawInfo.Format.LineAlignment;
        }
        internal void SetExcelPoint(int rowIndex, int colIndex)
        {
            excelInfo.SetCurrentPageLocation(rowIndex, colIndex);
        }
        public static bool IsNullOrEmpty(CellStruct tempCell)
        {
            return tempCell == null || tempCell == Empty;
        }
        public virtual CellStruct Clone()
        {
			return new CellStruct(this.context, this.drawInfo.Clone(), this.excelInfo.Clone());
        }
    }
    public class DrawInfo
    {
        Point point = Point.Empty;
        public Point Point
        {
            get { return point; }
            set { point = value; }
        }
        Size size = Size.Empty;
        public Size Size
        {
            get { return size; }
            set { size = value; }
        }
        
        BoundaryLine boundaryLine = new BoundaryLine();
        public BoundaryLine BoundaryLine
        {
            get { return boundaryLine; }
            set { boundaryLine = value; }
        }
        Rectangle DrawRect
        {
            get { return new Rectangle(point, size); }
        }
        Font font = null;

        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
        Brush brush = Brushes.Black;
        public Brush Brush
        {
            get { return brush; }
            set { brush = value; }
        }
        StringFormat format = new StringFormat();

        public StringFormat Format
        {
            get { return format; }
            set { format = value; }
        }
        public DrawInfo()
        {
        }
        public DrawInfo(Font font, Brush brush, StringFormat format)
        {
            this.font = font;
            this.brush = brush;
            this.format = format;
        }
        internal DrawInfo Clone()
        {
            DrawInfo info = new DrawInfo();
            info.Font = this.Font.Clone() as Font;
            info.Format = this.Format.Clone() as StringFormat;
            info.brush = this.brush.Clone() as Brush;
            info.BoundaryLine = this.BoundaryLine.Clone();

            
            return info;
        }
    }
    public class ExcelInfo
    {

        static ExcelInfo empty = new ExcelInfo(0, 0);
        public static ExcelInfo Empty
        {
            get { return empty; }
        }
        Point excelPoint;
        public Point ExcelPoint
        {
            get { return excelPoint; }

        }
        int rowMerge;
        public int RowMerge
        {
            get { return rowMerge; }
            set { rowMerge = value; }
        }
        int colMerge;
        public int ColMerge
        {
            get { return colMerge; }
            set { colMerge = value; }
        }
        public ExcelInfo(int rowMerge, int colMerge)
        {
            this.rowMerge = rowMerge;
            this.colMerge = colMerge;
            excelPoint = Point.Empty;
        }
        internal void SetCurrentPageLocation(int rowIndex, int colIndex)
        {
            excelPoint = new Point(rowIndex, colIndex);
        }
        public ExcelInfo Clone()
        {
            return new ExcelInfo(this.rowMerge, colMerge);
        }
        public override string ToString()
        {
            return string.Format("( {0},{1}) RM:{2} CM:{3}", excelPoint.X, excelPoint.Y, rowMerge, colMerge);
        }
    }

    public class FunCellStruct : CellStruct
    {
        public FunCellStruct(string context, DrawInfo drawInfo, ExcelInfo excelInfo)
            :
            base(context, drawInfo, excelInfo)
        {
        }
        public override CellStruct Clone()
        {
			return new FunCellStruct(this.context, this.drawInfo.Clone(), this.excelInfo.Clone());
        }
    }
    public class MacrsCellStruct : CellStruct
    {
        public MacrsCellStruct(string context, DrawInfo drawInfo, ExcelInfo excelInfo)
            : base(context, drawInfo, excelInfo)
        {
        }

        public CellStruct ReplaceMacrs(Dictionary<string, string> list)
        {

            string replaceContext = this.Context;
            MatchCollection matches = Regex.Matches(Context, @"\[\w+\]");
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                string key = Regex.Match(m.Value, @"(?<=\[)\w+(?=\])").Value;
                string val = "";
                list.TryGetValue(key, out val);
                replaceContext = replaceContext.Replace(m.Value, val);
            }

            CellStruct newCellStruct = this.Clone();
            newCellStruct.Context = replaceContext;
            return newCellStruct;

        }

        public override CellStruct Clone()
        {
			return new MacrsCellStruct(this.context, this.drawInfo.Clone(), this.excelInfo.Clone());
        }
    }
}

