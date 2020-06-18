using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FarPoint.Win.Spread;
using System.Data;

namespace Com.Bing.Report
{
    public class SpecialLineInfo : List<SpecialLine>
    {
        private SheetView sheet;        
        public SpecialLineInfo(SheetView sheet)
        {
            this.sheet = sheet;
            initLineInfo();
        }
        private void initLineInfo()
        {
            for (int rowIndex = 0; rowIndex < sheet.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < sheet.Columns.Count; colIndex++)
                {
                    int lineType =  -1;
                    Cell cell = sheet.Cells[rowIndex, colIndex];
                    if(cell.Tag != null)
                        lineType = Convert.ToInt32(cell.Tag.ToString());
                    if (lineType > 0)
                    {   
                        this.Add(new SpecialLine(cell));
                    }
                }
            }
        }
        public void CalcLinePoint(SpecialLine line,Point ScrollOffset)
        {
            int rowHeaderWidth = 0;
            int columnHeaderHeight = 0;
            for (int i = 0; i < sheet.RowHeader.Columns.Count; i++)
            {
                if (sheet.RowHeader.Columns[i].Visible)
                    rowHeaderWidth += Convert.ToInt32(sheet.RowHeader.Columns[i].Width);
            }
            for (int i = 0; i < sheet.ColumnHeader.RowCount; i++)
            {
                if (sheet.ColumnHeader.Rows[i].Visible)
                   columnHeaderHeight += Convert.ToInt32(sheet.ColumnHeader.Rows[i].Height);
            }

            //单元格位置信息
            int preHeight = columnHeaderHeight;
            for (int rowIndex = 0; rowIndex < line.Cell.Row.Index; rowIndex++)
                if(rowIndex > ScrollOffset.Y - 1 )
                preHeight += Convert.ToInt32(sheet.Rows[rowIndex].Height);

            int preWidth = rowHeaderWidth;
            for (int colIndex = 0; colIndex < line.Cell.Column.Index; colIndex++)
                if (sheet.Columns[colIndex].Visible && colIndex > ScrollOffset.X - 1)
                    preWidth += Convert.ToInt32(sheet.Columns[colIndex].Width);

            line.Location = new Point(preWidth, preHeight);
            line.Size = new Size(GetWidth(line.Cell, ScrollOffset.X - 1), GetHeight(line.Cell, ScrollOffset.Y - 1));
              
        }
        private int GetWidth(Cell cell, int scrollOffset)
        {
            int width = 0;
            for (int i = cell.Column.Index; i < cell.Column.Index + cell.ColumnSpan; i++)
            {
                if (sheet.Columns[i].Visible && i > scrollOffset)
                {
                    width += Convert.ToInt32(sheet.Columns[i].Width);
                }
            }
            return width;
        }
        private int GetHeight(Cell cell, int scrollOffset)
        {
            int height = 0;
            for (int i = cell.Row.Index; i < cell.Row.Index + cell.RowSpan; i++)
            {
                if(i> scrollOffset)
                    height += Convert.ToInt32(sheet.Rows[i].Height);
            }
            return height;
        }
        public void Refresh()
        {
            this.Clear();
            initLineInfo();
        }
        public SpecialLine GetSpecialLine(Cell cell)
        {
            foreach (SpecialLine line in this)
            {
                //这个地方有点奇怪
                //if (line.Cell == cell)
                if(line.Cell.Row.Index == cell.Row.Index &&
                    line.Cell.Column.Index == cell.Column.Index)
                    return line;
            }
            return null;
        }
        public void CreateLineIfNoExist(Cell cell)
        {
            if (GetSpecialLine(cell) == null)
                this.Add(new SpecialLine(cell));
        }   
    }
    public class SpecialLine
    {        
        public int LineType
        {
            get { return Int32.Parse(cell.Tag.ToString()); }
            set { cell.Tag = value; }
        }
        private Point location;

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }
        private Size size;

        public Size Size
        {
            get { return size; }
            set { size = value; }
        }

        //线的表格位置信息.rowIndex colIndex mergeSpan
        private Cell cell;
        public Cell Cell
        {
            get { return cell; }
            set { cell = value; }
        }
        public SpecialLine(Cell cell)
        {
            this.Cell = cell;
        }

    }
}
