using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Com.Bing.API;
using System.IO;
namespace Com.Bing.Report
{
    public class WholeReportInfo
    {
        //操作号:1;
        private string reportName = "0";
        public string ReportName
        {
            set { reportName = value; }
        }
        private double top = 0;
        public double Top
        {
            set { top = value; }
        }
        private double bottom = 0;
        public double Buttom
        {
            set { bottom = value; }
        }
        private double left = 0;
        public double Left
        {
            set { left = value; }
        }
        private double right = 0;
        public double Right
        {
            set { right = value; }
        }
        private double rowHeight = 0;
        public double RowHeight
        {
            set { rowHeight = value; }
        }
        private double fontSize = 0;
        public double FontSize
        {
            set { fontSize = value; }
        }
        private int grainDirection = 0;
        public int GrainDirection
        {
            set { grainDirection = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("1");
            string sheetName = reportName.Replace("[", "【").Replace("]", "】");
            sheetName = System.Text.RegularExpressions.Regex.Replace(sheetName, @"[:\\/\?\*\']", "");
            //名称长度限定为21字符，因为实际sheet名称最大31字符，页数可能上千，再加空格 sheet9999，需要去掉10个字符
            if (sheetName.Length > 21)
            {
                sheetName = sheetName.Substring(0, 21);
            }
            sb.AppendLine(sheetName);
			//EXCEL自身带有边距，这里只需要加入报表的一半边距
            sb.AppendLine(top != 0 ? (top / 2 + 20).ToString() : "0");
            sb.AppendLine(bottom != 0 ? (bottom / 2).ToString() : "0");
            sb.AppendLine(left != 0 ? (left / 2).ToString() : "0");
            sb.AppendLine(right != 0 ? (right / 2).ToString() : "0");

            sb.AppendLine(rowHeight > 0 ? rowHeight.ToString() : "0");
            sb.AppendLine(fontSize > 0 ? ((int)fontSize).ToString() : "0");
            sb.AppendLine(grainDirection.ToString());
            return sb.ToString();
        }
    }
    public class SetPaginationFlag
    {
        //操作号:2
        private string range = "0";
        public string Range
        {
            set { range = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("2");
            sb.AppendLine(range);
            return sb.ToString();
        }
    }
    public class SetTilteRow
    {
        //操作号:3
        private string range = "0";
        public string Range
        { set { range = value; } }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("3");
            sb.AppendLine(range);
            return sb.ToString();
        }
    }
    public class SetColumnWidth
    {
        public static float EchoRatio = 1.0f;
        //操作号：4
        private int columnIndex = 0;
        public int ColumnIndex
        {
            set { columnIndex = value; }
        }
        private int columnWidth = 0;
        public int ColumnWidth
        {
            set
            {
                columnWidth = Convert.ToInt32(value / EchoRatio);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("4");
            sb.AppendLine(columnIndex.ToString());
            sb.AppendLine(columnWidth.ToString());
            return sb.ToString();
        }
    }
    public class SetRowHeight
    {
        public static float EchoRatio = 1.0f;

        //操作号：5
        private int rowIndex = 0;
        public int RowIndex
        {
            set { rowIndex = value; }
        }
        private int rowHeight = 0;
        public int RowHeight
        {
            set
            {
                rowHeight = Convert.ToInt32(value / EchoRatio);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("5");
            sb.AppendLine(rowIndex.ToString());
            sb.AppendLine(rowHeight.ToString());
            return sb.ToString();
        }
    }
    public class SetCellContext
    {
        //操作号:10
        private string range = string.Empty;
        public string Range
        {
            set { range = value; }
        }
        private string cellContext = string.Empty;
        public string CellContext
        {
            set
            {
                cellContext = Com.Bing.API.Function.GenUpperMark(value);
            }
        }
        private int horizontalAlign = 0;
        public int HorizontalAlig
        {
            set { horizontalAlign = value; }
        }
        private int verticalAlign = 0;
        public int VerticalAlign
        {
            set { verticalAlign = value; }
        }
        private string fontName = "0";
        public string FontName
        {
            set { fontName = value; }
        }
        private float fontSize = 0.0f;
        public float FontSize
        {
            set { fontSize = value; }
        }
        //默认为自动换行
        private string autoWrap = "1";
        public string AutoWrap
        {
            set { autoWrap = value; }
        }
        private int bold = 0;
        public int Bold
        {
            set { bold = value; }
        }
        private int italic = 0;
        public int Italic
        {
            set { italic = value; }
        }
        private int underline = 0;
        public int Underline
        {
            set { underline = value; }
        }
        private int diagonal = 0;
        public int Diagonal
        {
            set { diagonal = value; }
        }
        private int topLine = 1;
        public int TopLine
        {
            set { topLine = value; }
        }
        private int buttomLine = 1;
        public int ButtomLine
        {
            set { buttomLine = value; }
        }
        private int leftLine = 1;
        public int LeftLine
        {
            set { leftLine = value; }
        }
        private int rightLine = 1;
        public int RightLine
        {
            set { rightLine = value; }
        }
        private int foreColor = 0;
        public int ForeColor
        {
            set { foreColor = value; }
        }
        private int backColor = 0;
        public int BackColor
        {
            set { backColor = value; }
        }

    }
    public class XLSFile
    {
        public static void Generate(string xlsFile, StringBuilder vbaContext, string rptName)
        {
            FileStream fs = null;
            fs = new FileStream(Function.VbaFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(vbaContext.ToString());//初始化整体xls样式
            sw.Close();
            try
            {
                long kk = Function.TransExcel(Function.VbaFilePath, rptName, xlsFile);
                if (Function.DebugMode)
                {
                    File.Copy(Function.VbaFilePath, string.Format("{0}{1:yyyyMMddHHmmssfff}.vba", Function.TempFolder, DateTime.Now));
                }
                File.Delete(Function.VbaFilePath);
            }
            catch (Exception e)
            {
                ReportOperateException sendException = null;
                if (!File.Exists(Function.XlsTemplate))
                {
                    sendException = new ExcelTemplateLostException();
                }
                else
                {
                    sendException = new SendToExcelException(e.Message);
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(e.Message, @".*无法加载 DLL“TransToExcel.dll”.*"))
                {
                    Function.Alert("程序找不到TransToExcel.dll组件!请确认TransToExcel.dll程序组件是否存在!", "提示");
                }
                else
                {
                    throw sendException;
                }
            }
        }
        public static void Generate(StringBuilder vbaContext, string rptName)
        {
            FileStream fs = null;
            fs = new FileStream(Function.VbaFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(vbaContext.ToString());//初始化整体xls样式
            sw.Close();
            try
            {
                long kk = Function.TransExcelPart(Function.VbaFilePath, rptName);
                if (Function.DebugMode)
                {
                    File.Copy(Function.VbaFilePath, string.Format("{0}{1:yyyyMMddHHmmssfff}.vba", Function.TempFolder, DateTime.Now));
                }
                File.Delete(Function.VbaFilePath);
            }
            catch (Exception e)
            {
                ReportOperateException sendException = null;
                if (!File.Exists(Function.XlsTemplate))
                {
                    sendException = new ExcelTemplateLostException();
                }
                else
                {
                    sendException = new SendToExcelException(e.Message);
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(e.Message, @".*无法加载 DLL“TransToExcel.dll”.*"))
                {
                    Function.Alert("程序找不到TransToExcel.dll组件!请确认TransToExcel.dll程序组件是否存在!", "提示");
                }
                else
                {
                    throw sendException;
                }
            }
        }

        public static bool ShowSaveFile(ref string xlsFilePath, string rptName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.Filter = "Microsoft Excel文件(*.xls)|*.xls";
            saveFileDialog.InitialDirectory = Function.UserFolder;
            saveFileDialog.FileName = Function.ReplaceSpecialCharWithBlank(rptName);
            bool selectSaveFile = false;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                xlsFilePath = saveFileDialog.FileName;
                try
                {
                    if (!File.Exists(xlsFilePath))
                    {
                        File.Copy(Function.XlsTemplate, xlsFilePath);
                    }
                }
                catch
                {
                    Function.Alert("目标文件被占用，或模板文件丢失。", "错误");
                }
                selectSaveFile = true;
            }
            return selectSaveFile;
        }
    }
}
