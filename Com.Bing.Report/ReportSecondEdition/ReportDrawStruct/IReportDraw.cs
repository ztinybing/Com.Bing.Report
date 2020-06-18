using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Collections;
namespace Com.Bing.Report
{
    public class ReportDrawManager : IDisposable
    {
        private Bitmap bitMapBuffer = null;
        public Bitmap BitMapBuffer
        {
            get
            {
                if (bitMapBuffer == null)
                {
                    bitMapBuffer = new Bitmap(report.TotalWidth + 1, report.TotalHeight);
                }
                return bitMapBuffer;
            }
        }
        private Graphics graphicsBuffer = null;
        public Graphics GraphicsBuffer
        {
            get
            {
                if (graphicsBuffer == null)
                {
                    graphicsBuffer = Graphics.FromImage(BitMapBuffer);
                }
                return graphicsBuffer;
            }
            set { graphicsBuffer = value; }
        }
        ReportDrawStruct WholeData
        {
            get
            {
                return report.WholeDrawStruct;
            }
        }
        Report report = null;
        public ReportDrawManager(Report report)
        {
            this.report = report;
            report.PropertyChanged += new ReportProeprtyChangedHandler(report_PropertyChanged);
        }

        void report_PropertyChanged(object sender, EventArgs e)
        {
            graphicsBuffer = null;
            bitMapBuffer = null;
        }
        public void Draw(Point point, int page)
        {
            GraphicsBuffer.Clear(Color.White);
            if (WholeData != null && WholeData.Invalid)
            {
                Draw(new IGraphics(GraphicsBuffer), point, page);
            }
            //PrintUtil.PrintWatermark(graphicsBuffer, report);
            
        }
        public void Draw(IGraphics g, Point point, int page)
        {
            if (WholeData != null && WholeData.Invalid)
            {
                DataTable pageTable = WholeData.GetPageData(page);
                for (int rowIndex = 0; rowIndex < pageTable.Rows.Count; rowIndex++)
                {
                    foreach (DataColumn column in pageTable.Columns)
                    {
                        g.DrawCellStruct(point, pageTable.Rows[rowIndex][column] as CellStruct);
                    }
                }
            }
        }
        public void Send(StringBuilder vbaContext, bool b)
        {

            if (!WholeData.Invalid)
            {
                return;
            }
            //vba整体设置字串
            vbaContext.AppendLine(WholeData.WholeInfo.ToString());

            SetColumnWidth.EchoRatio = WholeData.ExcelSizeF.Width;
            //vba整体的列设置
            int columnIndex = 1;
            foreach (int key in WholeData.ColumnInfo.Keys)
            {
                SetColumnWidth excelColumnWidth = new SetColumnWidth();
                excelColumnWidth.ColumnIndex = columnIndex;
                excelColumnWidth.ColumnWidth = WholeData.ColumnInfo[key].Width;
                vbaContext.AppendLine(excelColumnWidth.ToString());
                columnIndex++;
            }
            SetRowHeight.EchoRatio = WholeData.ExcelSizeF.Height;
            int rowCount = 1;



            foreach (PageInfo info in WholeData.PageList)
            {
                foreach (int height in info.PageRowHeight)
                {
                    SetRowHeight excelRowHeight = new SetRowHeight();
                    excelRowHeight.RowIndex = rowCount;
                    excelRowHeight.RowHeight = height;
                    vbaContext.AppendLine(excelRowHeight.ToString());
                    rowCount++;
                }
            }

            Point location = Point.Empty;
            CellStruct cellStruct = null;
            //整张页发送
            for (int pageIndex = 1; pageIndex <= WholeData.PageCount; pageIndex++)
            {
                //List<int> list = WholeData.ExBodyHeight;

                DataTable pageTable = WholeData.GetPageData(pageIndex);
                for (int rowIndex = 0; rowIndex < pageTable.Rows.Count; rowIndex++)
                {
                    columnIndex = 0;
                    foreach (DataColumn column in pageTable.Columns)
                    {
                        cellStruct = pageTable.Rows[rowIndex][column] as CellStruct;
                        if (!CellStruct.IsNullOrEmpty(cellStruct))
                        {
                            vbaContext.AppendLine((cellStruct).ToExcelString(
                                location));

                        }
                        columnIndex++;
                    }
                }
                if (WholeData.PageList[pageIndex - 1].IsCrosswisePage && InHorizontalPage(pageIndex, WholeData.PageList))
                {
                    location.Offset(0, pageTable.Columns.Count);
                }
                else
                {
                    location.Y = 0;
                    location.Offset(pageTable.Rows.Count, 0);
                }
                SetPaginationFlag paginationFlag = new SetPaginationFlag();
                paginationFlag.Range = string.Format("1,1,{0},{1}", location.Y, location.X);
                vbaContext.AppendLine(paginationFlag.ToString());
            }
        }
        private bool InHorizontalPage(int pageIndex, List<PageInfo> listPageInfo)
        {
            bool inHorizontalPage = false;
            if (listPageInfo.Count > pageIndex)
            {
                //后一样的横向分页号大于当前页的为同一横向页面中
                if (listPageInfo[pageIndex].HpageIndex > listPageInfo[pageIndex - 1].HpageIndex)
                {
                    inHorizontalPage = true;
                }
            }
            return inHorizontalPage;
        }

        public void Send(StringBuilder vbaContext)
        {
            if (!WholeData.Invalid)
            {
                return;
            }
            //vba整体设置字串
            vbaContext.AppendLine(WholeData.WholeInfo.ToString());

            SetColumnWidth.EchoRatio = WholeData.ExcelSizeF.Width;
            //vba整体的列设置
            int columnIndex = 1;
            foreach (int key in WholeData.ColumnInfo.Keys)
            {
                SetColumnWidth excelColumnWidth = new SetColumnWidth();
                excelColumnWidth.ColumnIndex = columnIndex;
                excelColumnWidth.ColumnWidth = WholeData.ColumnInfo[key].Width;
                vbaContext.AppendLine(excelColumnWidth.ToString());
                columnIndex++;
            }
            SetRowHeight.EchoRatio = WholeData.ExcelSizeF.Height;
            int rowCount = 1;
            foreach (PageInfo info in WholeData.PageList)
            {
                foreach (int width in info.PageRowHeight)
                {
                    SetRowHeight excelRowHeight = new SetRowHeight();
                    excelRowHeight.RowIndex = rowCount;
                    excelRowHeight.RowHeight = width;
                    vbaContext.AppendLine(excelRowHeight.ToString());
                    rowCount++;
                }
            }

            Point location = Point.Empty;
            CellStruct cellStruct = null;
            //整张页发送
            for (int pageIndex = 1; pageIndex <= WholeData.PageCount; pageIndex++)
            {
                DataTable pageTable = WholeData.GetPageData(pageIndex);
                for (int rowIndex = 0; rowIndex < pageTable.Rows.Count; rowIndex++)
                {
                    columnIndex = 0;
                    foreach (DataColumn column in pageTable.Columns)
                    {
                        cellStruct = pageTable.Rows[rowIndex][column] as CellStruct;
                        if (!CellStruct.IsNullOrEmpty(cellStruct))
                        {
                            vbaContext.AppendLine((cellStruct).ToExcelString(
                                location));

                        }
                        columnIndex++;
                    }
                }
                if (WholeData.PageList[pageIndex - 1].IsCrosswisePage && InHorizontalPage(pageIndex, WholeData.PageList))
                {
                    location.Offset(0, pageTable.Columns.Count);
                }
                else
                {
                    location.Y = 0;
                    location.Offset(pageTable.Rows.Count, 0);
                }
                SetPaginationFlag paginationFlag = new SetPaginationFlag();
                paginationFlag.Range = string.Format("1,1,{0},{1}", location.Y, location.X);
                vbaContext.AppendLine(paginationFlag.ToString());
            }
        }

        public void Dispose()
        {
            if (bitMapBuffer != null)
            {
                bitMapBuffer.Dispose();
                bitMapBuffer = null;
            }

            if (graphicsBuffer != null)
            {
                graphicsBuffer.Dispose();
                graphicsBuffer = null;
            }
        }
    }
    /// <summary>
    /// 应编写单元测试测试该模块的正确性
    /// </summary>
    public class IGraphics
    {
        //文本背景色为固定值
        static Brush contextBackGroundColor = null;
        static IGraphics()
        {
            contextBackGroundColor = ConvertUtil.GetBrush("0");
        }
        public Graphics g;
        public IGraphics(Graphics g)
        {
            this.g = g;
        }
        public void DrawCellStruct(CellStruct cellStruct)
        {
            DrawCellStruct(Point.Empty, cellStruct);
        }
        public void DrawCellStruct(Point point, CellStruct cellStruct)
        {
            if (cellStruct != null && cellStruct != CellStruct.Empty)
            {
                // 画一个CELL，分为划线与写入文本内容
                DrawCellLine(point, cellStruct.DrawInfo);
                DrawCellContext(point, cellStruct.Context, cellStruct.DrawInfo);
            }
        }

        public void DrawCellLine(Point point, DrawInfo cellDrawInfo)
        {
            //表格内部线宽设固定值
            //Pen p = new Pen(Brushes.Black, 1.0f); 
            Pen p = new Pen(cellDrawInfo.Brush, 1.0f);
            BoundaryLine boundaryLine = cellDrawInfo.BoundaryLine;
            int x = cellDrawInfo.Point.X + point.X;
            int y = cellDrawInfo.Point.Y + point.Y;
            int height = cellDrawInfo.Size.Height;
            int width = cellDrawInfo.Size.Width;

            if (boundaryLine.LeftBoundaryLine)
            {
                g.DrawLine(p, x, y, x, y + height);
#if DrawReportBoundaraLineSize
                if (updateBorderPoint)
                {
                    borderLine.UpdateLeftUpPoint(x, y);
                }
#endif
            }
            if (boundaryLine.RightBooundaryLine)
            {
                g.DrawLine(p, x + width, y, x + width, y + height);
#if DrawReportBoundaraLineSize
                if (updateBorderPoint)
                {
                    borderLine.UpdateRightDownPoint(x + width, y + height);
                }
#endif
            }

            if (boundaryLine.UpperBoundaryLine)
                g.DrawLine(p, x, y, x + width, y);
            if (boundaryLine.LowerBoundaryLine)
                g.DrawLine(p, x, y + height, x + width, y + height);

            //特殊线，正、反、十字线
            if (boundaryLine.IsSlash)
                g.DrawLine(p, x, y + height, x + width, y);
            if (boundaryLine.IsBackSlash)
                g.DrawLine(p, x, y, x + width, y + height);

            if (boundaryLine.IsCrossLine)
            {
                g.DrawLine(p, x, y, x + width, y + height);
                g.DrawLine(p, x, y + height, x + width, y);
            }
        }
        public void DrawCellContext(Point point, string context, DrawInfo cellDrawInfo)
        {
            //Rectangle stringRange = new Rectangle(cellDrawInfo.Point, cellDrawInfo.Size);
            Rectangle stringRange = new Rectangle(cellDrawInfo.Point, cellDrawInfo.Size);
            stringRange.Offset(point.X, point.Y);
            //cjl 20120604 自动换行时向右偏移，会多出半个字符的距离，打印位置偏离，在最后的字符中都加入/r/n，在stringrange中处理半个字符的问题
            //根据文本的对齐方式画位置，因为后面会加入\r\n会多出空格的距离
            //switch (cellDrawInfo.Format.LineAlignment)
            //{
            //    case StringAlignment.Near:
            //        stringRange.Offset(point.X, point.Y);
            //        break;
            //    case StringAlignment.Center:
            //        stringRange.Offset(point.X + 4, point.Y);
            //        break;
            //    case StringAlignment.Far:
            //        stringRange.Offset(point.X + 7, point.Y);
            //        break;
            //}
            //stringRange.Offset(point.X, point.Y);
            //+1是因为线占一个宽度，当然线占多个单位宽时暂不考虑！
            //+halfCharWdith 是因为MeasureString总是会在右边和下面添加了一部分空白
            int halfCharHeight = cellDrawInfo.Font.Height / 4 + 1;
            stringRange.Y = stringRange.Y + halfCharHeight;
            stringRange.Height = stringRange.Height - halfCharHeight;

            int halfCharWidth = Convert.ToInt32(cellDrawInfo.Font.Size / 8);
            stringRange.X = stringRange.X + halfCharWidth;
            //不去掉字符两边的空格
            StringFormat sf = new StringFormat();

            sf.FormatFlags = cellDrawInfo.Format.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;

            sf.Alignment = cellDrawInfo.Format.LineAlignment;
            sf.LineAlignment = cellDrawInfo.Format.Alignment;

            //因使用MeasureTrailingSpaces标识，它会将字符后的回车换行算进行，导致中对齐的时候最行一行            
            //向右偏移。故在每一个无回车换行处加一个回车换行符
            //bool hasEndChar = context.EndsWith("\r\n") || context.EndsWith("\n") || context.EndsWith("\r");
            //if (!hasEndChar)
            //{
            //    context += "\r\n";
            //}
            context = Com.Bing.API.Function.GenUpperMark(context); //上标转换问题



            g.DrawString(context, cellDrawInfo.Font, Brushes.Black/* contextBackGroundColor*/, stringRange, sf);
            //g.DrawString(context, cellDrawInfo.Font, Brushes.Black/* contextBackGroundColor*/, stringRange.X, stringRange.Y, sf);
        }
    }
}
