using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FarPoint.Win.Spread;
using System.Windows.Forms;
using FarPoint.Win.Spread.Model;
using System.Data;

namespace Com.Bing.Report
{
    public static class DesignHelper
    {
        #region ���뷽ʽת�� FarPoint�еĶ�����StringAlignmentת��
        public static StringAlignment ConvertHAlignmentToStringAlignment(CellHorizontalAlignment horizontalAligment)
        {
            int value = 0;
            switch (horizontalAligment)
            {
                case CellHorizontalAlignment.Left:
                    value = 0;
                    break;
                case CellHorizontalAlignment.Right:
                    value = 2;
                    break;
                default:
                    value = 1;
                    break;
            }
            return (StringAlignment)value;
        }
        public static StringAlignment ConvertVAlignmentToStringAlignment(CellVerticalAlignment verticalAlignment)
        {
            int value = 0;
            switch (verticalAlignment)
            {
                case CellVerticalAlignment.Top:
                    value = 0;
                    break;
                case CellVerticalAlignment.Bottom:
                    value = 2;
                    break;
                default:
                    value = 1;
                    break;
            }
            return (StringAlignment)value;
        }
        public static CellHorizontalAlignment ConvertStringAlignmentToHAlignment(StringAlignment stringAlignment)
        {
            int value = 0;
            switch (stringAlignment)
            {
                case StringAlignment.Near:
                    value = 1;
                    break;
                case StringAlignment.Center:
                    value = 2;
                    break;
                case StringAlignment.Far:
                    value = 3;
                    break;
            }
            return (CellHorizontalAlignment)value;
        }
        public static CellVerticalAlignment ConvertStringAlignmentToVAlignment(StringAlignment stringAlignment)
        {
            int value = 0;
            switch (stringAlignment)
            {
                case StringAlignment.Near:
                    value = 1;
                    break;
                case StringAlignment.Center:
                    value = 2;
                    break;
                case StringAlignment.Far:
                    value = 3;
                    break;
            }
            return (CellVerticalAlignment)value;
        }
        #endregion

        #region FarPoint�Դ��߿����ú���
        public static void LocalizationCHS(FarPoint.Win.Spread.Design.BorderEditor borderedit)
        {
            borderedit.Text = "�߿�����";
            borderedit.Width = 280;
            foreach (Control control in borderedit.Controls)
            {
                if (control is Button)
                {
                    Button btn = control as Button;
                    switch (btn.Text)
                    {
                        case "Help":
                            borderedit.Controls.Remove(control);
                            break;
                        case "Ok":
                            btn.Size = new Size(80, 25);
                            btn.Location = new Point(btn.Location.X - 120, btn.Location.Y);
                            btn.Text = "ȷ��";
                            break;
                        case "Cancel":
                            btn.Size = new Size(80, 25);
                            btn.Location = new Point(btn.Location.X - 120, btn.Location.Y);
                            btn.Text = "ȡ��";
                            break;
                    }
                }
                if (control is Label)
                {
                    Label label = control as Label;
                    switch (label.Text)
                    {
                        case "Inside":
                            label.Text = "�ڱ߿�";
                            break;
                        case "Outline":
                            label.Text = "��߿�";
                            break;
                        case "None":
                            label.Text = "ȡ���߿�";
                            break;
                        case "Border":
                            label.Text = "�߿����";
                            break;
                        case "Presets":
                            label.Text = "Ԥ�ñ߿�";
                            break;
                    }
                }
            }
        }

        #endregion

        public static Cell GetSelectedCell(SheetView sheet)
        {
            CellRange ran = sheet.GetSelection(0);

            if (ran != null)
            {
                Cell cell = null;
                //ѡ����ҳ
                if (ran.Column == -1 && ran.Row == -1 && ran.ColumnCount == -1 && ran.RowCount == -1)
                {
                    cell = sheet.Cells[0, 0, sheet.RowCount - 1, sheet.ColumnCount - 1];
                }
                //ѡ������
                else if (ran.Column == -1 && ran.ColumnCount == -1)
                {
                    cell = sheet.Cells[ran.Row, 0, ran.RowCount + ran.Row - 1, sheet.Columns.Count - 1];
                }
                //ѡ������
                else if (ran.Row == -1 && ran.RowCount == -1)
                {
                    cell = sheet.Cells[0, ran.Column, sheet.Rows.Count - 1, ran.Column + ran.ColumnCount - 1];
                }
                else
                {
                    cell = sheet.Cells[ran.Row, ran.Column, ran.RowCount + ran.Row - 1, ran.Column + ran.ColumnCount - 1];
                }

                return cell;
            }
            return null;
        }

        public static bool ContainColumnName(Report report, string columnName, string defaultColumnName)
        {
            foreach (ReportColumn column in report.Columns)
            {
                if (column.ColumnName.Trim() == columnName.Trim() && column.ColumnName.Trim() != defaultColumnName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetTextPropertys(Cell cell, Text text)
        {
            if (cell.Font != null)
            {
                text.Attribute.Font = cell.Font;
            }

            #region ���䷽ʽ
            text.Attribute.Valign.LineAlignment = ConvertHAlignmentToStringAlignment(cell.HorizontalAlignment);
            text.Attribute.Valign.Alignment = ConvertVAlignmentToStringAlignment(cell.VerticalAlignment);
            #endregion

            #region �߿�
            text.Attribute.BoundaryLine.SetNoBoundary();
            if (cell.Border != null)
            {
                text.Attribute.BoundaryLine.UpperBoundaryLine = cell.Border.Inset.Top == 1;
                text.Attribute.BoundaryLine.LowerBoundaryLine = cell.Border.Inset.Bottom == 1;
                text.Attribute.BoundaryLine.LeftBoundaryLine = cell.Border.Inset.Left == 1;
                text.Attribute.BoundaryLine.RightBooundaryLine = cell.Border.Inset.Right == 1;
            }
            #endregion
        }

        public static List<CellStruct> GetListCellStruct(Cell cell, DataTable dt)
        {
            List<CellStruct> list = new List<CellStruct>();
            if (cell != null)
            {
                for (int i = cell.Row.Index; i <= cell.Row.Index2; i++)
                {
                    for (int j = cell.Column.Index; j <= cell.Column.Index2; j++)
                    {
                        CellStruct cellStruct = dt.Rows[i][j] as CellStruct;
                        if (!CellStruct.IsNullOrEmpty(cellStruct))
                        {
                            list.Add(cellStruct);
                        }
                    }
                }
            }
            return list;
        }

        //�ж�һ����Ԫ���Ƿ��ںϲ���Ԫ��֮�ڣ�ͬʱ�����ǵ�һ����Ԫ��
        public static bool InMergeCell(List<int[]> listMergeInfo, int x, int y)
        {
            foreach (int[] i in listMergeInfo)
            {
				//���ϣ����µ�
                if (i.Length == 4)
                {
                    int x1 = i[0];
                    int x2 = i[1];
                    int y1 = i[2];
                    int y2 = i[3];
                    if (x == x1 && y == y1)//�ϲ��ĵ�һ����Ԫ�񲻼���
                    {
                        return false;
                    }
                    else
                    {
                        if (x >= x1 && x <= x2 && y >= y1 && y < y2)//�����Ƿ��ںϲ���Ԫ��֮��
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
