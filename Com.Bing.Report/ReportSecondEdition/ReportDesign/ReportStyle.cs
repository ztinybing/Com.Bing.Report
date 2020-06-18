using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using FarPoint.Win.Spread.Model;
using FarPoint.Win.Spread;
using Com.Bing.API;
namespace Com.Bing.Report
{
    public partial class ReportStyle : DevExpress.XtraEditors.XtraForm
    {
        private int startHeadIndex;
        private int startTitileIndex;
        private int startBodyIndex;
        private int startPageIndex;
        private int startFootIndex;
        private DataTable dtSpan = new DataTable();
        private bool showNoDataColumn = false;
        private bool isDragingCol = false;
        //记录特殊线位置
        private SpecialLineInfo specialLineInfo;
        //记录行显隐勾选状态
        private Dictionary<Cell, bool> dicCheckState = new Dictionary<Cell, bool>();

        private Report report;
        public ReportStyle(Report report)
        {
            this.report = report;
            dtSpan.Columns.Add("minX", typeof(int));
            dtSpan.Columns.Add("minY", typeof(int));
            dtSpan.Columns.Add("maxX", typeof(int));
            dtSpan.Columns.Add("maxY", typeof(int));

            InitializeComponent();

            InputMap im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);
            im.Put(new Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.StartEditing);



            InitReportStyle();
            fpSpread1.HorizontalScrollBar.Scroll += new ScrollEventHandler(HorizontalScrollBar_Scroll);
            fpSpread1.VerticalScrollBar.Scroll += new ScrollEventHandler(VerticalScrollBar_Scroll);
            fpSpread1.HorizontalScrollBar.KeyUp += new KeyEventHandler(HorizontalScrollBar_KeyUp);
            fpSpread1.VerticalScrollBar.KeyUp += new KeyEventHandler(VerticalScrollBar_KeyUp);
        }
        private void InitReportStyle()
        {
            //初始化表头信息
            InitColumnHeader();
            //添加分隔符
            InsertSeparateRow();

            InitStartIndex();
            //填充文本内容
            InitContent();
            //初始化行显隐
            InitVisiable();

            sheet.ColumnHeader.Rows[1].Visible = Function.DebugMode;
            showOriginalData.Visible = Function.DebugMode;


            //初始化合并信息
            specialLineInfo = new SpecialLineInfo(sheet);
        }

        private void InitStartIndex()
        {
            startHeadIndex = 1;
            startTitileIndex = sheet.GetRowFromTag(sheet.Rows[0], "表头").Index + 1;
            startBodyIndex = sheet.GetRowFromTag(sheet.Rows[0], "表体").Index + 1;
            startPageIndex = sheet.GetRowFromTag(sheet.Rows[0], "页汇").Index + 1;
            startFootIndex = sheet.GetRowFromTag(sheet.Rows[0], "表脚").Index + 1;
        }

        private void InitColumnHeader()
        {
            sheet.ColumnHeader.RowCount = 2;//有两行表头，第一行是外部中文名称，第二行是内部名称
            sheet.ColumnCount = report.Columns.Count;
            for (int i = 0; i < sheet.ColumnCount; i++)
            {
                sheet.ColumnHeader.Cells[0, i].Text = report.Columns[i].Attibutes.NameZh_cn;
                sheet.ColumnHeader.Cells[1, i].Text = report.Columns[i].ColumnName;
                sheet.Columns[i].Width = report.Columns[i].ColumnWidth;
                if (!showNoDataColumn && report.Columns[i].Attibutes.PrintStyle.NoPrint)
                {
                    sheet.Columns[i].Visible = false;
                }
                else
                {
                    sheet.Columns[i].Visible = true;
                }
            }

            sheet.Columns.Add(sheet.ColumnCount, 1);//加入一列控制显隐，RowGroup
            sheet.ColumnHeader.Cells[0, sheet.ColumnCount - 1].Text = "行显隐";
            sheet.ColumnHeader.Cells[1, sheet.ColumnCount - 1].Text = "RowGroup";
            sheet.Columns[sheet.ColumnCount - 1].HorizontalAlignment = CellHorizontalAlignment.Center;
        }

        private void InsertSeparateRow()
        {
            int rowIndex = 0;
            InsertSeparateRow(rowIndex, "标题");
            rowIndex += report.Bands.GetCount(Bands.HeadBandID) + 1;

            InsertSeparateRow(rowIndex, "表头");
            rowIndex += report.Bands.GetCount(Bands.TitleBandID) + 1;
            InsertSeparateRow(rowIndex, "表体");
            rowIndex += report.Bands.GetCount(Bands.BodyBandID) + 1;
            InsertSeparateRow(rowIndex, "页汇");
            rowIndex += report.Bands.GetCount(Bands.PageBandID) + 1;
            InsertSeparateRow(rowIndex, "表脚");
            rowIndex += report.Bands.GetCount(Bands.RootBandID) + 1;

            sheet.Rows.Count = rowIndex;
        }
        private void InsertSeparateRow(int index, string text)
        {
            sheet.Cells[index, 0].Text = text;
            sheet.Cells[index, 0].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheet.AddSpanCell(index, 0, 1, report.Columns.Count + 1);
            sheet.Rows[index].BackColor = Color.LightBlue;
            sheet.Rows[index].Locked = true;
            sheet.Rows[index].Tag = text;
        }



        private void InitContent()
        {
            int index = 0;
            foreach (Text text in report.Texts)
            {
                switch (text.BandIndex)
                {
                    case 1:
                        index = startHeadIndex;
                        break;
                    case 2:
                        index = startTitileIndex;
                        break;
                    case 3:
                        index = startBodyIndex;
                        break;
                    case 4:
                        index = startPageIndex;
                        break;
                    case 5:
                        index = startFootIndex;
                        break;
                }
                InitContent(text, index);
            }
        }
        private void InitContent(Text text, int index)
        {
            try
            {

                FarPoint.Win.IBorder border = new FarPoint.Win.LineBorder(Color.Black, 1, text.Attribute.BoundaryLine.LeftBoundaryLine, text.Attribute.BoundaryLine.UpperBoundaryLine, text.Attribute.BoundaryLine.RightBooundaryLine, text.Attribute.BoundaryLine.LowerBoundaryLine);

                int X = text.OriginalLocation.Y1 - 1 + index;
                int Y = text.OriginalLocation.X1 - 1;



                SizeF sizef = report.GraphicsBuffer.MeasureString(text.CellText, text.Attribute.Font);
                if (sizef.Height > sheet.Rows[X].Height && text.RowSpan == 1)
                {
                    sheet.Rows[X].Height = sizef.Height;
                }
                sheet.Cells[X, Y].Text = text.CellText;
                sheet.AddSpanCell(X, Y, text.RowSpan, text.ColumnSpan);

                sheet.Cells[X, Y].Font = text.Attribute.Font;
                sheet.Cells[X, Y].Border = border;
                sheet.Cells[X, Y].HorizontalAlignment = DesignHelper.ConvertStringAlignmentToHAlignment(text.Attribute.Valign.LineAlignment);
                sheet.Cells[X, Y].VerticalAlignment = DesignHelper.ConvertStringAlignmentToVAlignment(text.Attribute.Valign.Alignment);

                sheet.Cells[X, Y].Tag = text.Attribute.Diagonal;

            }
            catch
            {
                throw new CellLocationInvalidExcption(text.ToString());
            }
        }

        private void InitVisiable()
        {
            InitVisiable(startHeadIndex, report.Bands.BandDict[Bands.HeadBandID].BandCount, report.Bands.BandDict[Bands.HeadBandID].EchoRowDict);
            InitVisiable(startTitileIndex, report.Bands.BandDict[Bands.TitleBandID].BandCount, report.Bands.BandDict[Bands.TitleBandID].EchoRowDict);
            InitVisiable(startPageIndex, report.Bands.BandDict[Bands.PageBandID].BandCount, report.Bands.BandDict[Bands.PageBandID].EchoRowDict);
            InitVisiable(startFootIndex, report.Bands.BandDict[Bands.RootBandID].BandCount, report.Bands.BandDict[Bands.RootBandID].EchoRowDict);
        }
        private void InitVisiable(int index, int rowCount, Dictionary<int, bool> dicVisiableRow)
        {
            for (int i = index; i < rowCount + index; i++)
            {
                sheet.Cells[i, sheet.ColumnCount - 1].CellType = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                if (!dicVisiableRow.ContainsKey(i - index + 1))
                {
                    sheet.Cells[i, sheet.ColumnCount - 1].Text = "True";
                    dicCheckState.Add(sheet.Cells[i, sheet.ColumnCount - 1], true);
                }
            }
        }

        #region 按钮事件处理
        //保存
        private void btn_save_Click(object sender, EventArgs e)
        {
            Save();
        }
        //合并单元格
        private void Merge(object sender, EventArgs e)
        {
            CellRange cellRange = sheet.GetSelection(0);
            if (cellRange != null && cellRange.Row >= 0 && cellRange.Column >= 0)
            {
                for (int i = cellRange.Row; i < cellRange.Row + cellRange.RowCount; i++)
                {
                    if (sheet.Rows[i].Tag != null)
                    {
                        Function.Alert("选中区域有间隔行，不能合并", "提示");
                        return;
                    }
                }
                sheet.AddSpanCell(cellRange.Row, cellRange.Column, cellRange.RowCount, cellRange.ColumnCount);
                //合并更改，重新记录合并信息
                //处理 被合并单元格有斜线的情况
                specialLineInfo.Refresh();
                btn_save.Enabled = true;
            }
        }
        //取消合并单元格
        private void CancalMerge(object sender, EventArgs e)
        {
            CellRange cellRange = sheet.GetSelection(0);
            if (cellRange != null)
            {
                for (int i = cellRange.Row; i < cellRange.Row + cellRange.RowCount; i++)
                {
                    if (sheet.Rows[i].Tag != null)
                    {
                        Function.Alert("选中区域有间隔行，不能取消合并", "提示");
                        return;
                    }
                }
                for (int i = cellRange.Row; i < cellRange.Row + cellRange.RowCount; i++)
                {
                    for (int j = cellRange.Column; j < cellRange.Column + cellRange.ColumnCount; j++)
                    {
                        sheet.RemoveSpanCell(i, j);
                    }
                }
                btn_save.Enabled = true;
            }
        }
        //插入行
        private void InsertRow(object sender, EventArgs e)
        {
            CellRange ran = sheet.GetSelection(0);
            if (ran != null && ran.Row >= 0)
            {
                sheet.AddRows(ran.Row + 1, 1);
                InitStartIndex();
                if (ran.Row < startBodyIndex - 1 || ran.Row + 1 >= startPageIndex)
                {
                    sheet.Cells[ran.Row + 1, sheet.ColumnCount - 1].CellType = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                    sheet.Cells[ran.Row + 1, sheet.ColumnCount - 1].Text = "True";
                }
                btn_save.Enabled = true;
            }
        }
        //删除行
        private void DeleteRow(object sender, EventArgs e)
        {
            CellRange ran = sheet.GetSelection(0);
            if (ran != null)
            {
                for (int i = ran.Row; i < ran.Row + ran.RowCount; i++)
                {
                    if (sheet.Rows[i].Tag != null)
                    {
                        Function.Alert("不能删除间隔行", "提示");
                        return;
                    }
                }
                sheet.Rows.Remove(ran.Row, ran.RowCount);
                InitStartIndex();
                //行列更改更改，重新记录合并信息
                specialLineInfo.Refresh();
                btn_save.Enabled = true;
            }
        }
        //插入列
        private void InsertCol(object sender, EventArgs e)
        {
            using (ColumnNameInputDlg dlg = new ColumnNameInputDlg("", "", report.OriginalData.Columns))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (DesignHelper.ContainColumnName(report, dlg.IColumnText, ""))
                    {
                        Function.Alert("已经存在当前内部名称，插入列失败", "提示");
                        return;
                    }
                    ReportColumn newColumn = new ReportColumn(dlg.IColumnText);
                    newColumn.Attibutes.NameZh_cn = dlg.EColumnText;
                    int columnIndex = sheet.ActiveColumnIndex;
                    sheet.Columns.Add(columnIndex, 1);
                    sheet.ColumnHeader.Cells[0, columnIndex].Text = newColumn.Attibutes.NameZh_cn;
                    sheet.ColumnHeader.Cells[1, columnIndex].Text = newColumn.ColumnName;
                    newColumn.ColumnWidth = Convert.ToInt32(sheet.Columns[columnIndex].Width);
                    report.Columns.Insert(columnIndex, newColumn);
                    btn_save.Enabled = true;
                }
            }
        }
        //删除列
        private void DeleteCol(object sender, EventArgs e)
        {
            CellRange ran = sheet.GetSelection(0);
            for (int i = ran.Column + ran.ColumnCount - 1; i >= ran.Column; i--)
            {
                sheet.Columns.Remove(i, 1);
                report.Columns.RemoveAt(i);
            }
            //行列更改更改，重新记录合并信息
            specialLineInfo.Refresh(); ;
            btn_save.Enabled = true;
        }
        //字体设置
        private void FontSet(object sender, EventArgs e)
        {
            using (FontDialog f = new FontDialog())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    Cell cell = DesignHelper.GetSelectedCell(sheet);
                    if (cell != null)
                    {
                        cell.Font = f.Font;
                        btn_save.Enabled = true;
                    }
                }
            }
        }
        //添加边框
        private void FillBorder(object sender, EventArgs e)
        {
            CellRange range = sheet.GetSelection(0);
            if (range != null)
            {
                FarPoint.Win.BevelBorder border = new FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Raised, Color.Black, Color.Black, 1, true, true, true, true);
                for (int i = range.Row; i < range.Row + range.RowCount; i++)
                {
                    for (int j = range.Column; j < range.Column + range.ColumnCount; j++)
                    {
                        sheet.Cells[i, j].Border = border;
                    }
                }
                btn_save.Enabled = true;
            }
        }
        //取消边框
        private void CancalBorder(object sender, EventArgs e)
        {
            CellRange range = sheet.GetSelection(0);
            if (range != null)
            {
                FarPoint.Win.BevelBorder border = new FarPoint.Win.BevelBorder(FarPoint.Win.BevelBorderType.Raised, Color.Black, Color.Black, 1, false, false, false, false);
                for (int i = range.Row; i < range.Row + range.RowCount; i++)
                {
                    for (int j = range.Column; j < range.Column + range.ColumnCount; j++)
                    {
                        sheet.Cells[i, j].Border = border;
                    }
                }
                btn_save.Enabled = true;
            }
        }


        private void 正斜线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sheet.ActiveCell.Tag = 1;
            this.specialLineInfo.CreateLineIfNoExist(sheet.ActiveCell);
            btn_save.Enabled = true;
        }

        private void 反斜线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sheet.ActiveCell.Tag = 2;
            this.specialLineInfo.CreateLineIfNoExist(sheet.ActiveCell);
            btn_save.Enabled = true;
        }

        private void 十字线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sheet.ActiveCell.Tag = 3;
            this.specialLineInfo.CreateLineIfNoExist(sheet.ActiveCell);
            btn_save.Enabled = true;
        }
        private void 清除特殊线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpecialLine line = specialLineInfo.GetSpecialLine(sheet.ActiveCell);
            if (line != null)
            {
                sheet.ActiveCell.Tag = null;
                specialLineInfo.Remove(line);
                btn_save.Enabled = true;
            }
        }
        #endregion

        private void fpSpread1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int maxRowCount;
            int maxColumnCount;
            getRange(e.Range.Row, e.Range.Column, e.Range.RowCount, e.Range.ColumnCount, out maxRowCount, out maxColumnCount);
            ISheetSelectionModel sel = (ISheetSelectionModel)sheet.Models.Selection;
            sel.ClearSelection();
            sel.SetSelection(e.Range.Row, e.Range.Column, maxRowCount, maxColumnCount);
        }
        private void getRange(int row, int column, int rowCount, int columnCount, out int maxRowCount, out int maxColumnCount)
        {
            maxRowCount = rowCount;
            maxColumnCount = columnCount;
            bool reCalc = false;

            for (int j = column; j <= column + columnCount - 1; j++)
            {
                int rCount = 0;
                for (int i = row; i <= row + rowCount - 1; i++)
                {
                    rCount += sheet.Cells[i, j].RowSpan;
                    i += sheet.Cells[i, j].RowSpan - 1;
                }
                if (rCount > maxRowCount)
                {
                    maxRowCount = rCount;
                    reCalc = true;
                }
            }

            for (int i = row; i <= row + rowCount - 1; i++)
            {
                int cCount = 0;
                for (int j = column; j <= column + columnCount - 1; j++)
                {
                    cCount += sheet.Cells[i, j].ColumnSpan;
                    j += sheet.Cells[i, j].ColumnSpan - 1;
                }
                if (cCount > maxColumnCount)
                {
                    maxColumnCount = cCount;
                    reCalc = true;
                }
            }

            if (reCalc)
            {
                getRange(row, column, maxRowCount, maxColumnCount, out maxRowCount, out maxColumnCount);
            }
        }
        private void ReportStyle_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (KeyValuePair<Cell, bool> pair in dicCheckState)
            {
                Cell cell = pair.Key;
                bool checkState = true;
                if (cell.Text.ToLower() == "false")
                    checkState = false;
                if (checkState != pair.Value)
                {
                    btn_save.Enabled = true;
                }
            }
        }
        private void ReportStyle_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (btn_save.Enabled)
            {
                if (Function.Confirm("是否保存修改信息", "提示"))
                {
                    Save();
                }
            }
        }
        private void Save()
        {

            List<ReportColumn> listReportColumn = new List<ReportColumn>();
            foreach (Column col in sheet.Columns)
            {
                if (col.Label.ToLower() == "rowgroup")
                    continue;
                ReportColumn reportColumn = report.Columns[col.Label];
                listReportColumn.Add(reportColumn);
            }
            report.Columns.Clear();
            foreach (ReportColumn reportColumn in listReportColumn)
            {
                report.Columns.Add(reportColumn);
            }

            report.Texts.Clear();
            report.Bands.ClearEchoDic();
            int index = 0;
            int bandIndex = 0;
            int count = 0;
            for (int i = 0; i < sheet.RowCount; i++)
            {
                if (sheet.Rows[i].Tag != null)
                {
                    count = 0;
                    switch (sheet.Rows[i].Tag.ToString())
                    {
                        case "标题":
                            bandIndex = 1;
                            index = startHeadIndex;
                            break;
                        case "表头":
                            bandIndex = 2;
                            index = startTitileIndex;
                            break;
                        case "表体":
                            bandIndex = 3;
                            index = startBodyIndex;
                            break;
                        case "页汇":
                            bandIndex = 4;
                            index = startPageIndex;
                            break;
                        case "表脚":
                            bandIndex = 5;
                            index = startFootIndex;
                            break;
                    }
                    //初始化
                    report.Bands.SetBindRowNum(bandIndex, 0);
                    continue;
                }
                count++;
                if (sheet.Cells[i, sheet.ColumnCount - 1].Text != "True")
                {
                    report.Bands.SetBindRowEcho(bandIndex, i - index + 1, false);
                }
                report.Bands.SetBindRowNum(bandIndex, count);

                for (int j = 0; j < sheet.ColumnCount; j++)
                {
                    if (sheet.Cells[i, j].CellType is FarPoint.Win.Spread.CellType.CheckBoxCellType)
                    {
                        continue;
                    }
                    DataRow[] rows = dtSpan.Select(string.Format("{0} >= minY and {0} <= maxY and {1} >= minX and {1} <= maxX", i, j));
                    if (rows.Length == 0)
                    {
                        //不在合并单元格中
                        Text text = new Text();
                        text.BandIndex = bandIndex;
                        text.Context = sheet.Cells[i, j].Text;
                        text.Location.X1 = j + 1;
                        text.Location.Y1 = i + 1 - index;
                        text.Location.X2 = j + sheet.Cells[i, j].ColumnSpan;
                        text.Location.Y2 = i + sheet.Cells[i, j].RowSpan - index;

                        SpecialLine line = specialLineInfo.GetSpecialLine(sheet.Cells[i, j]);
                        if (line != null)
                            text.Attribute.Diagonal = line.LineType;

                        DesignHelper.SetTextPropertys(sheet.Cells[i, j], text);

                        report.Texts.Add(text);
                        DataRow row = dtSpan.NewRow();
                        row["minX"] = j;
                        row["minY"] = i;
                        row["maxX"] = j + sheet.Cells[i, j].ColumnSpan - 1;
                        row["maxY"] = i + sheet.Cells[i, j].RowSpan - 1;
                        dtSpan.Rows.Add(row);
                    }
                }
            }
            report.WriteRpt();
            dtSpan.Clear();
            btn_save.Enabled = false;
        }


        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                if (cell.Font == null)
                {
                    cell.Font = new Font("宋体", 9);
                }
                if (cell.Font.Bold)
                {
                    cell.Font = new Font(cell.Font.FontFamily, cell.Font.Size, cell.Font.Style ^ FontStyle.Bold);
                }
                else
                {
                    cell.Font = new Font(cell.Font.FontFamily, cell.Font.Size, cell.Font.Style | FontStyle.Bold);
                }

                btn_save.Enabled = true;
            }
        }
        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                if (cell.Font == null)
                {
                    cell.Font = new Font("宋体", 9);
                }
                if (cell.Font.Italic)
                {
                    cell.Font = new Font(cell.Font.FontFamily, cell.Font.Size, cell.Font.Style ^ FontStyle.Italic);
                }
                else
                {
                    cell.Font = new Font(cell.Font.FontFamily, cell.Font.Size, cell.Font.Style | FontStyle.Italic);
                }

                btn_save.Enabled = true;
            }
        }
        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                if (cell.Font == null)
                {
                    cell.Font = new Font("宋体", 9);
                }
                if (cell.Font.Underline)
                {
                    cell.Font = new Font(cell.Font.FontFamily, cell.Font.Size, cell.Font.Style ^ FontStyle.Underline);
                }
                else
                {
                    cell.Font = new Font(cell.Font.FontFamily, cell.Font.Size, cell.Font.Style | FontStyle.Underline);
                }
                btn_save.Enabled = true;
            }
        }



        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            using (FarPoint.Win.Spread.Design.BorderEditor borderedit = new FarPoint.Win.Spread.Design.BorderEditor(fpSpread1))
            {
                borderedit.Shown += new EventHandler(borderedit_Shown);
                DesignHelper.LocalizationCHS(borderedit);
                if (sheet.SelectionCount == 0)
                {
                    borderedit.StartColumn = sheet.ActiveColumnIndex;
                    borderedit.ColumnCount = 1;
                    borderedit.StartRow = sheet.ActiveRowIndex;
                    borderedit.RowCount = 1;
                }
                else
                {
                    CellRange range = sheet.GetSelection(0); //选中区域
                    if (range != null)
                    {
                        borderedit.StartColumn = range.Column;
                        borderedit.ColumnCount = range.ColumnCount;
                        borderedit.StartRow = range.Row;
                        borderedit.RowCount = range.RowCount;
                    }
                }
                borderedit.ShowDialog();
            }
            btn_save.Enabled = true;
        }
        void borderedit_Shown(object sender, EventArgs e)
        {
            FarPoint.Win.Spread.Design.BorderEditor borderedit = sender as FarPoint.Win.Spread.Design.BorderEditor;

            GroupBox groupBox = (borderedit.Controls["BorderLineGroupBox"] as GroupBox);
            ListView lineList = groupBox.Controls["ListView_Style"] as ListView;
            lineList.Items[6].Selected = true;
        }

        private void 左对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                cell.HorizontalAlignment = CellHorizontalAlignment.Left;
                btn_save.Enabled = true;
            }
        }

        private void 中对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                cell.HorizontalAlignment = CellHorizontalAlignment.Center;
                btn_save.Enabled = true;
            }
        }

        private void 右对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                cell.HorizontalAlignment = CellHorizontalAlignment.Right;
                btn_save.Enabled = true;
            }
        }

        private void 上对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                cell.VerticalAlignment = CellVerticalAlignment.Top;
                btn_save.Enabled = true;
            }
        }

        private void 中对齐ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                cell.VerticalAlignment = CellVerticalAlignment.Center;
                btn_save.Enabled = true;
            }
        }

        private void 下对齐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cell cell = DesignHelper.GetSelectedCell(sheet);
            if (cell != null)
            {
                cell.VerticalAlignment = CellVerticalAlignment.Bottom;
                btn_save.Enabled = true;
            }
        }

        private void fpSpread1_CellClick(object sender, CellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.ColumnHeader)
                {
                    fpSpread1.ContextMenuStrip = contextMenuStrip1;
                }
                else if (sheet.Rows[e.Row].Tag != null || e.RowHeader)
                {
                    fpSpread1.ContextMenuStrip = contextMenuStrip3;
                }
                else
                {
                    fpSpread1.ContextMenuStrip = contextMenuStrip2;
                }
            }
            else
            {
                fpSpread1.ContextMenuStrip = null;
            }
        }

        private void SetColName(object sender, EventArgs e)
        {
            int columnIndex = sheet.ActiveColumnIndex;
            using (ColumnNameInputDlg dlg = new ColumnNameInputDlg(sheet.ColumnHeader.Cells[0, columnIndex].Text, sheet.ColumnHeader.Cells[1, columnIndex].Text, report.OriginalData.Columns))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (DesignHelper.ContainColumnName(report, dlg.IColumnText, sheet.ColumnHeader.Cells[1, columnIndex].Text))
                    {
                        Function.Alert("列中已经存在了当前内部名称，修改失败", "提示");
                        return;
                    }
                    sheet.ColumnHeader.Cells[0, columnIndex].Text = dlg.EColumnText;
                    sheet.ColumnHeader.Cells[1, columnIndex].Text = dlg.IColumnText;
                    report.Columns.SetColumnName(report.Columns[columnIndex], dlg.IColumnText);
                    //report.Columns[columnIndex].ColumnName = 

                    report.Columns[columnIndex].Attibutes.NameZh_cn = dlg.EColumnText;
                    sheet.Columns[columnIndex].Tag = dlg.IColumnText;
                    btn_save.Enabled = true;
                }
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (btn_save.Enabled && Function.Confirm("是否保存当前修改信息", "提示"))
            {
                Save();
            }
            Function.DebugMode = !Function.DebugMode;
            InitReportStyle();
        }
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (btn_save.Enabled && Function.Confirm("是否保存当前修改信息", "提示"))
            {
                Save();
            }
            showNoDataColumn = !showNoDataColumn;
            InitReportStyle();
        }

        private void fpSpread1_CellDoubleClick(object sender, CellClickEventArgs e)
        {
            Com.Bing.Forms.InputDialog dlg = new Com.Bing.Forms.InputDialog("编辑文本", fpSpread1.ActiveSheet.ActiveCell.Text, true, true);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                sheet.ActiveCell.Text = dlg.TextResult;
                btn_save.Enabled = true;
            }
        }

        private void fpSpread1_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            foreach (ColumnWidthChangeExtents extents in e.ColumnList)
            {
                for (int i = extents.FirstColumn; i <= extents.LastColumn; i++)
                {
                    if (report.Columns.Count > i)
                    {
                        report.Columns[i].ColumnWidth = Convert.ToInt32(sheet.Columns[i].Width);
                    }
                }
            }
            btn_save.Enabled = true;

        }
        private void fpSpread1_RowHeightChanged(object sender, RowHeightChangedEventArgs e)
        {
            btn_save.Enabled = true;
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            Dictionary<object, string> dic = new Dictionary<object, string>();
            foreach (KeyValuePair<KeyValuePair<string, int>, string> pair in report.Data.MacorsVerbCollection)
            {
                if (!dic.ContainsKey(pair.Key.Key))
                {
                    dic.Add(pair.Key.Key, pair.Value);
                }
            }
            Com.Bing.Forms.FormSelectList dlg = new Com.Bing.Forms.FormSelectList("插入宏变量", dic, new bool[] { true, false });
            dlg.selectedClickEvent = insertMacro;
            dlg.Show();
        }
        public void insertMacro(object macroName)
        {
            if (macroName != null && sheet.ActiveCell != null)
            {
                sheet.ActiveCell.Text = sheet.ActiveCell.Text + "[" + macroName + "]";
                btn_save.Enabled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete)
            {
                CellRange range = sheet.GetSelection(0);
                if (range != null)
                {
                    for (int i = range.Row; i < range.Row + range.RowCount; i++)
                    {
                        for (int j = range.Column; j < range.Column + range.ColumnCount; j++)
                        {
                            sheet.Cells[i, j].Text = "";
                        }
                    }
                    btn_save.Enabled = true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        #region 列拖动处理
        //获取分隔列的集合
        private List<Row> GetListSpearateRow()
        {
            List<Row> listRow = new List<Row>();
            string[] strTags = new string[] { "标题", "表头", "表体", "页汇", "表脚" };
            foreach (string tag in strTags)
            {
                Row SeparateRow = sheet.GetRowFromTag(sheet.Rows[-1], tag);
                if (SeparateRow != null)
                {
                    listRow.Add(SeparateRow);
                }
            }
            return listRow;
        }
        //拖动时取消分隔行的合并，其他合并需要手动取消掉
        private void fpSpread1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragingCol && e.Button == MouseButtons.Left)
            {
                List<Row> listSpearateRow = GetListSpearateRow();
                foreach (Row separateRow in listSpearateRow)
                {
                    sheet.RemoveSpanCell(separateRow.Index, 0);
                }
            }
        }
        //拖动完成后合并分隔行
        private void fpSpread1_ColumnDragMoveCompleted(object sender, DragMoveCompletedEventArgs e)
        {
            List<Row> listSpearateRow = GetListSpearateRow();
            foreach (Row separateRow in listSpearateRow)
            {
                sheet.AddSpanCell(separateRow.Index, 0, 1, sheet.ColumnCount);
            }
            isDragingCol = false;
        }
        private void fpSpread1_ColumnDragMove(object sender, DragMoveEventArgs e)
        {
            isDragingCol = true;
        }
        #endregion

        private void showOriginalData_Click(object sender, EventArgs e)
        {
            FormShowOriginalData dlg = new FormShowOriginalData(report.OriginalData);
            dlg.Show();
        }

        #region 调整列顺序
        private void ChangeColOrder_Click(object sender, EventArgs e)
        {
            List<string> listColumns = new List<string>();
            Dictionary<string, OrderInfo> dicColumnInfo = new Dictionary<string, OrderInfo>();
            for (int i = 0; i < sheet.ColumnHeader.Columns.Count - 1; i++)
            {
                if (!sheet.Columns[i].Visible)
                    continue;
                string colNameCH = sheet.ColumnHeader.Cells[0, i].Text;
                string colName = sheet.ColumnHeader.Cells[1, i].Text;
                string keyName = string.Format("{0}|{1}", colNameCH, colName);
                listColumns.Add(keyName);

                OrderInfo info = new OrderInfo(sheet.Columns[i].Width);
                for (int j = 0; j < sheet.Rows.Count; j++)
                {
                    if (sheet.Rows[j].Tag != null)
                        continue;

                    string text = sheet.Cells[j, i].Text;
                    Font font = sheet.Cells[j, i].Font;
                    CellHorizontalAlignment hAlignment = sheet.Cells[j, i].HorizontalAlignment;
                    CellVerticalAlignment vAlignment = sheet.Cells[j, i].VerticalAlignment;
                    FarPoint.Win.IBorder border = sheet.Cells[j, i].Border; ;
                    CellInfo cellInfo = new CellInfo(j, text, font, hAlignment, vAlignment, border);
                    info.Add(cellInfo);
                }
                dicColumnInfo.Add(keyName, info);
            }
            FormAdjustColOrder dlg = new FormAdjustColOrder(listColumns);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int index = 0;
                for (int i = 0; i < sheet.ColumnHeader.Columns.Count - 1; i++)
                {
                    if (!sheet.Columns[i].Visible)
                        continue;
                    string colName = listColumns[index++];
                    string[] strs = colName.Split(new char[] { '|' }, 2);
                    sheet.ColumnHeader.Cells[0, i].Text = strs[0];
                    sheet.ColumnHeader.Cells[1, i].Text = strs[1];
                    sheet.Columns[i].Width = dicColumnInfo[colName].ColumnWidth;
                }
                foreach (string colName in listColumns)
                {
                    int colIndex = GetColumnIndexByColName(colName);
                    if (colIndex >= 0)
                    {
                        OrderInfo info = dicColumnInfo[colName];
                        foreach (CellInfo cellInfo in info)
                        {
                            sheet.Cells[cellInfo.RowIndex, colIndex].Text = cellInfo.Text;
                            sheet.Cells[cellInfo.RowIndex, colIndex].Font = cellInfo.Font;
                            sheet.Cells[cellInfo.RowIndex, colIndex].HorizontalAlignment = cellInfo.HAlignment;
                            sheet.Cells[cellInfo.RowIndex, colIndex].VerticalAlignment = cellInfo.VAlignment;
                            sheet.Cells[cellInfo.RowIndex, colIndex].Border = cellInfo.Border;
                        }
                    }
                }
                btn_save.Enabled = true;
            }
        }
        private int GetColumnIndexByColName(string colName)
        {
            string[] strs = colName.Split(new char[] { '|' }, 2);
            for (int i = 0; i < sheet.Columns.Count; i++)
            {
                if (sheet.Columns[i].Label == strs[1])
                    return i;
            }
            return -1;
        }
        #endregion
        private void fpSpread1_Paint(object sender, PaintEventArgs e)
        {
            //查看滚动条是否存在
            if (fpSpread1.VerticalScrollBar.Height == 0)
            {
                Offset.Y = 0;
            }

            if (fpSpread1.HorizontalScrollBar.Width == 0)
            {
                Offset.X = 0;
            }

            //TAG=1 是正斜线
            //    2 反斜线
            //    3 交叉线            
            foreach (SpecialLine line in specialLineInfo)
            {
                specialLineInfo.CalcLinePoint(line, Offset);
                switch (line.LineType)
                {
                    case 1:
                        e.Graphics.DrawLine(new Pen(Color.Black),
                            line.Location.X + line.Size.Width, line.Location.Y,
                            line.Location.X, line.Location.Y + line.Size.Height);
                        break;
                    case 2:
                        e.Graphics.DrawLine(new Pen(Color.Black),
                            line.Location.X, line.Location.Y,
                            line.Location.X + line.Size.Width, line.Location.Y + line.Size.Height);
                        break;
                    case 3:
                        e.Graphics.DrawLine(new Pen(Color.Black),
                            line.Location.X, line.Location.Y,
                            line.Location.X + line.Size.Width, line.Location.Y + line.Size.Height);

                        e.Graphics.DrawLine(new Pen(Color.Black),
                            line.Location.X + line.Size.Width, line.Location.Y,
                            line.Location.X, line.Location.Y + line.Size.Height);
                        break;
                }
            }
        }
        //列偏移量
        Point Offset = new Point();
        //滚动条拖动时触发重绘
        void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Offset.X = e.NewValue;

        }
        void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Offset.Y = e.NewValue;

        }
        void VerticalScrollBar_KeyUp(object sender, KeyEventArgs e)
        {
            fpSpread1.Refresh();
        }
        void HorizontalScrollBar_KeyUp(object sender, KeyEventArgs e)
        {
            fpSpread1.Refresh();
        }

        private void fpSpread1_EditChange(object sender, EditorNotifyEventArgs e)
        {
            btn_save.Enabled = true;
        }

        
    }

    public class OrderInfo : System.ComponentModel.BindingList<CellInfo>
    {
        private float columnWidth;
        public float ColumnWidth
        {
            get { return columnWidth; }
        }

        public OrderInfo(float columnWidth)
        {
            this.columnWidth = columnWidth;
        }
    }
    public class CellInfo
    {
        private int rowIndex;
        public int RowIndex
        {
            get { return rowIndex; }
        }

        private string text;
        public string Text
        { get { return text; } }

        private Font font;
        public Font Font
        { get { return font; } }

        private CellHorizontalAlignment hAlignment;
        public CellHorizontalAlignment HAlignment
        { get { return hAlignment; } }

        private CellVerticalAlignment vAlignment;
        public CellVerticalAlignment VAlignment
        { get { return vAlignment; } }

        private FarPoint.Win.IBorder border;
        public FarPoint.Win.IBorder Border
        { get { return border; } }

        public CellInfo(int rowIndex, string text, Font font, CellHorizontalAlignment hAlignment, CellVerticalAlignment vAlignment, FarPoint.Win.IBorder border)
        {
            this.rowIndex = rowIndex;
            this.text = text;
            this.font = font;
            this.hAlignment = hAlignment;
            this.vAlignment = vAlignment;
            this.border = border;
        }
    }

}