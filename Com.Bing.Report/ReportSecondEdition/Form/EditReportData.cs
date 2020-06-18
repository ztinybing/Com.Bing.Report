using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FarPoint.Win.Spread.Model;
using FarPoint.Win.Spread;

namespace Com.Bing.Report
{
    public partial class EditReportData : DevExpress.XtraEditors.XtraForm
    {
        private PageInfo pageInfo;
        private DataTable dt;

        public EditReportData(DataTable dt, PageInfo pageInfo)
        {
            InitializeComponent();

            this.pageInfo = pageInfo;
            this.dt = dt;

            InitSheetView();
        }
        private void InitSheetView()
        {
            try
            {
                sheet.ColumnCount = pageInfo.ContainCols.Count;
                sheet.RowCount = dt.Rows.Count;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        CellStruct cellStruct = dt.Rows[i][j] as CellStruct;
                        if (!CellStruct.IsNullOrEmpty(cellStruct))
                        {
                            //文本内容
                            sheet.Cells[i, j].Text = cellStruct.Context;
                            //字体信息
                            sheet.Cells[i, j].Font = cellStruct.DrawInfo.Font;
                            //边框信息
                            FarPoint.Win.IBorder border = new FarPoint.Win.LineBorder(Color.Black, 1,
                                cellStruct.DrawInfo.BoundaryLine.LeftBoundaryLine,
                                cellStruct.DrawInfo.BoundaryLine.UpperBoundaryLine,
                                cellStruct.DrawInfo.BoundaryLine.RightBooundaryLine,
                                cellStruct.DrawInfo.BoundaryLine.LowerBoundaryLine);

                            sheet.Cells[i, j].Border = border;
                            //合并信息
                            sheet.Cells[i, j].RowSpan = cellStruct.ExcelInfo.RowMerge;
                            sheet.Cells[i, j].ColumnSpan = cellStruct.ExcelInfo.ColMerge;

                            //对齐方式
                            sheet.Cells[i, j].HorizontalAlignment = DesignHelper.ConvertStringAlignmentToHAlignment(cellStruct.DrawInfo.Format.LineAlignment);
                            sheet.Cells[i, j].VerticalAlignment = DesignHelper.ConvertStringAlignmentToVAlignment(cellStruct.DrawInfo.Format.Alignment);
                        }
                    }
                    sheet.Rows[i].Height = pageInfo.PageRowHeight[i];
                }
                for (int i = 0; i < pageInfo.ColumnInfo.Length; i++)
                {
                    sheet.Columns[i].Width = pageInfo.ColumnInfo[i].Width;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            CellStruct cellStruct = dt.Rows[e.Row][e.Column] as CellStruct;
            if (!CellStruct.IsNullOrEmpty(cellStruct))
            {
                cellStruct.Context = sheet.Cells[e.Row, e.Column].Text;
                dt.Rows[e.Row][e.Column] = cellStruct;
                tool_Save.Enabled = true;
            }
        }
        //合并单元格
        private void tool_Merge_Click(object sender, EventArgs e)
        {
            CellRange range = sheet.GetSelection(0);
            if (range != null)
            {
                sheet.AddSpanCell(range.Row, range.Column, range.RowCount, range.ColumnCount);
                for (int i = range.Row; i < range.Row + range.RowCount; i++)
                {
                    for (int j = range.Column; j < range.Column + range.ColumnCount; j++)
                    {
                        CellStruct cellStruct = dt.Rows[i][j] as CellStruct;
                        if (i == range.Row && j == range.Column)
                        {
                            cellStruct.ExcelInfo.RowMerge = range.RowCount;
                            cellStruct.ExcelInfo.ColMerge = range.ColumnCount;
                        }
                        else
                        {
                            cellStruct = CellStruct.Empty;
                        }
                        dt.Rows[i][j] = cellStruct;
                    }
                }

                tool_Save.Enabled = true;
            }
        }
        //取消合并单元格
        private void tool_CancelMerge_Click(object sender, EventArgs e)
        {
            CellRange range = sheet.GetSelection(0);
            if (range != null)
            {
                CellStruct orginalCellStrct = dt.Rows[range.Row][range.Column] as CellStruct;
                int x = orginalCellStrct.DrawInfo.Point.X;
                int y = orginalCellStrct.DrawInfo.Point.Y;
                for (int i = range.Row; i < range.Row + range.RowCount; i++)
                {
                    int height = Convert.ToInt32(sheet.Rows[i].Height);
                    for (int j = range.Column; j < range.Column + range.ColumnCount; j++)
                    {
                        sheet.RemoveSpanCell(i, j);
                        //CellStruct cellStruct = dt.Rows[i][j] as CellStruct;
                        CellStruct cellStruct = new CellStruct("", null, null);
                        cellStruct.Context = sheet.Cells[i, j].Text;
                        DrawInfo drawInfo = new DrawInfo();

                        if (sheet.Cells[i, j].Border != null)
                        {
                            drawInfo.BoundaryLine.UpperBoundaryLine = sheet.Cells[i, j].Border.Inset.Top == 1;
                            drawInfo.BoundaryLine.LowerBoundaryLine = sheet.Cells[i, j].Border.Inset.Bottom == 1;
                            drawInfo.BoundaryLine.LeftBoundaryLine = sheet.Cells[i, j].Border.Inset.Left == 1;
                            drawInfo.BoundaryLine.RightBooundaryLine = sheet.Cells[i, j].Border.Inset.Right == 1;
                        }

                        int width = Convert.ToInt32(sheet.Columns[j].Width);
                        drawInfo.Size = new Size(width, height);
                        drawInfo.Point = new Point(x, y);
                        x += width;

                        drawInfo.Format.LineAlignment = DesignHelper.ConvertHAlignmentToStringAlignment(sheet.Cells[i, j].HorizontalAlignment);
                        drawInfo.Format.Alignment = DesignHelper.ConvertVAlignmentToStringAlignment(sheet.Cells[i, j].VerticalAlignment);

                        drawInfo.Font = sheet.Cells[i, j].Font;

                        cellStruct.DrawInfo = drawInfo;
                        
                        cellStruct.ExcelInfo = new ExcelInfo(1, 1);

                        dt.Rows[i][j] = cellStruct;
                    }
                    y += height;
                }
            }
        }
        private void EditReportData_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tool_Save.Enabled && Com.Bing.API.Function.Confirm("确定修改当前报表的数据", "提示"))
            {
            }
        }

        //边框设置
        private void tool_InsertBorder_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                FarPoint.Win.BevelBorder border = new FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Raised, Color.Black, Color.Black, 1, true, true, true, true);
                cell.Border = border;
                List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                foreach (CellStruct cellStruct in list)
                {
                    cellStruct.DrawInfo.BoundaryLine.SetBoundary();
                }
                tool_Save.Enabled = true;
            }
        }

        private void tool_cancelBorder_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                FarPoint.Win.BevelBorder border = new FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Raised, Color.Black, Color.Black, 1, false, false, false, false);
                cell.Border = border;
                List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                foreach (CellStruct cellStruct in list)
                {
                    cellStruct.DrawInfo.BoundaryLine.SetNoBoundary();
                }
                tool_Save.Enabled = true;
            }
        }

        private void tool_borderSet_Click(object sender, EventArgs e)
        {
            using (FarPoint.Win.Spread.Design.BorderEditor borderedit = new FarPoint.Win.Spread.Design.BorderEditor(fpSpread1))
            {
                borderedit.Shown += new EventHandler(borderedit_Shown);
                DesignHelper.LocalizationCHS(borderedit);
                if (borderedit.ShowDialog() == DialogResult.OK)
                {
                    Cell cell = DesignHelper.GetSelectedCell(sheet);
                    if (cell != null)
                    {
                        List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                        foreach (CellStruct cellStruct in list)
                        {
                            if (cell.Border == null)
                            {
                                cellStruct.DrawInfo.BoundaryLine.SetNoBoundary();
                            }
                            else
                            {
                                cellStruct.DrawInfo.BoundaryLine.UpperBoundaryLine = cell.Border.Inset.Top == 1;
                                cellStruct.DrawInfo.BoundaryLine.LowerBoundaryLine = cell.Border.Inset.Bottom == 1;
                                cellStruct.DrawInfo.BoundaryLine.LeftBoundaryLine = cell.Border.Inset.Left == 1;
                                cellStruct.DrawInfo.BoundaryLine.RightBooundaryLine = cell.Border.Inset.Right == 1;
                            }
                        }
                    }
                }
            }
        }
        void borderedit_Shown(object sender, EventArgs e)
        {
            FarPoint.Win.Spread.Design.BorderEditor borderedit = sender as FarPoint.Win.Spread.Design.BorderEditor;

            GroupBox groupBox = (borderedit.Controls["BorderLineGroupBox"] as GroupBox);
            ListView lineList = groupBox.Controls["ListView_Style"] as ListView;
            lineList.Items[6].Selected = true;
        }


        private void tool_FontSet_Click(object sender, EventArgs e)
        {
            using (FontDialog f = new FontDialog())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    Cell cell = DesignHelper.GetSelectedCell(sheet);
                    if (cell != null)
                    {
                        List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                        foreach (CellStruct cellStruct in list)
                        {
                            cellStruct.DrawInfo.Font = f.Font;
                        }
                        cell.Font = f.Font;
                        tool_Save.Enabled = true;
                    }
                }
            }
        }

        private void 左对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                cell.HorizontalAlignment = CellHorizontalAlignment.Left;
                foreach (CellStruct cellStruct in list)
                {
                    cellStruct.DrawInfo.Format.LineAlignment = DesignHelper.ConvertHAlignmentToStringAlignment(CellHorizontalAlignment.Left);
                }
                tool_Save.Enabled = true;
            }
        }

        private void 中对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                cell.HorizontalAlignment = CellHorizontalAlignment.Center;
                foreach (CellStruct cellStruct in list)
                {
                    cellStruct.DrawInfo.Format.LineAlignment = DesignHelper.ConvertHAlignmentToStringAlignment(CellHorizontalAlignment.Center);
                }
                tool_Save.Enabled = true;
            }
        }

        private void 右对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                cell.HorizontalAlignment = CellHorizontalAlignment.Right;
                foreach (CellStruct cellStruct in list)
                {
                    cellStruct.DrawInfo.Format.LineAlignment = DesignHelper.ConvertHAlignmentToStringAlignment(CellHorizontalAlignment.Right);
                }
                tool_Save.Enabled = true;
            }
        }

        private void 上对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                cell.VerticalAlignment = CellVerticalAlignment.Top;
                foreach (CellStruct cellStruct in list)
                {
                    cellStruct.DrawInfo.Format.Alignment = DesignHelper.ConvertVAlignmentToStringAlignment(CellVerticalAlignment.Top);
                }
                tool_Save.Enabled = true;
            }
        }

        private void 中对齐ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                cell.VerticalAlignment = CellVerticalAlignment.Center;
                foreach (CellStruct cellStruct in list)
                {
                    cellStruct.DrawInfo.Format.Alignment = DesignHelper.ConvertVAlignmentToStringAlignment(CellVerticalAlignment.Center);
                }
                tool_Save.Enabled = true;
            }
        }

        private void 下对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                cell.VerticalAlignment = CellVerticalAlignment.Bottom;
                foreach (CellStruct cellStruct in list)
                {
                    cellStruct.DrawInfo.Format.Alignment = DesignHelper.ConvertVAlignmentToStringAlignment(CellVerticalAlignment.Bottom);
                }
                tool_Save.Enabled = true;
            }
        }

        private void tool_Save_Click(object sender, EventArgs e)
        {
            tool_Save.Enabled = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete)
            {
                Cell cell = DesignHelper.GetSelectedCell(sheet);
                if (cell != null)
                {
                    cell.Text = "";
                    List<CellStruct> list = DesignHelper.GetListCellStruct(cell, dt);
                    foreach (CellStruct cellStruct in list)
                    {
                        cellStruct.Context = "";
                    }
                    tool_Save.Enabled = true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void fpSpread1_CellDoubleClick(object sender, CellClickEventArgs e)
        {
            Com.Bing.Forms.InputDialog dlg = new Com.Bing.Forms.InputDialog("编辑文本", fpSpread1.ActiveSheet.ActiveCell.Text, true, true);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                sheet.ActiveCell.Text = dlg.TextResult;
                CellStruct cellStruct = dt.Rows[e.Row][e.Column] as CellStruct;
                if (!CellStruct.IsNullOrEmpty(cellStruct))
                {
                    cellStruct.Context = dlg.TextResult;
                }
                tool_Save.Enabled = true;
            }
        }
    }
}