using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Printing;
using System.Collections.Specialized;
using Com.Bing.API;
using System.Data;
using System.IO;
using System.Drawing;


namespace Com.Bing.Report
{

    public delegate void ReportProeprtyChangedHandler(object sender, EventArgs e);
    public delegate void ReportDataChangedHandler(object sender, EventArgs e);
    public class PrintUtil
    {
        static PrintUtil()
        {
            printDocument.PrinterSettings = PrintDlg.PrinterSettings;
        }
        public static PrintDialog PrintDlg = new PrintDialog();
        public static PrintDocument printDocument = new PrintDocument();
        public static DialogResult ShowPrintDlg()
        {
            return ShowPrintDlg(false);
        }
        public static DialogResult ShowPrintDlg(bool projectPDF)
        {
            if (projectPDF)
            {
                bool pdfExists = false;
                foreach (string print in Function.GetPrintersCollection())
                {
                    if (print.ToUpper().Contains("PDF"))
                    {
                        PrintDlg.PrinterSettings.PrinterName = print;
                        pdfExists = true;
                        break;
                    }
                }
                if (!pdfExists)
                {
                    Function.Alert("PDF打印输出功能，需安装PDF打印机支持", "提示");
                    return DialogResult.Cancel;
                }
            }
            else
            {
                PrintDlg.PrinterSettings.PrinterName = null;
            }
            PrintDlg.AllowSomePages = true;
            return PrintDlg.ShowDialog();
        }
        public static PrintDocument PrintDocument
        {
            get
            {
                //printDocument.DocumentName = report.reportName;
                if (printDocument.PrintController == null)
                {
                    printDocument.PrintController = new System.Drawing.Printing.StandardPrintController();
                    printDocument.OriginAtMargins = true;
                }
                return printDocument;
            }
        }
        public static PaperSize PaperSize = null;
        /// <summary>
        /// 用于获取打印机纸张宽度，在系统重置打印机时应重掉该函数
        /// </summary>
        public static void AsynGetPaperSize()
        {
            System.ComponentModel.BackgroundWorker groundWorker = new System.ComponentModel.BackgroundWorker();
            groundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(groundWorker_DoWork);
            groundWorker.RunWorkerAsync();
        }
        static void groundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            PaperSize = PrintDlg.PrinterSettings.DefaultPageSettings.PaperSize;
        }
        public static void PrintWatermark(Graphics g, Report report)
        {
            Brush brsuh = new SolidBrush(Color.Gray);
            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
            g.DrawString("学  习  版",
                new Font("宋体", 60),
                brsuh,
                new Rectangle(0, 0, report.TotalWidth, report.TotalHeight),
                stringFormat);
        }
    }
    public class Report:IDisposable
    {
        //这个为程序黙认的打印程序设置
        private const int REPORTTOTALWIDTH = 827;
        private const int REPORTTOTALHEIGHT = 1127;
        private const int ExcelPagingWidth = 748;
        private const int ExcelPagingHeight = 1078;


        public event ReportProeprtyChangedHandler PropertyChanged = null;
        public event ReportDataChangedHandler DataChanged = null;

        private string dllPath = string.Empty;
        public string DllPath
        {
            get { return dllPath; }
        }
        private string docPath = string.Empty;
        public string DocPath
        {
            get { return docPath; }
        }
        public bool IsDoc
        {
            get
            {
                return !string.IsNullOrEmpty(docPath);
            }
        }
        private bool propertyValid = false;
        public bool PropertyValid
        {
            get { return propertyValid; }
            set { propertyValid = true; }
        }
        private bool dataValid = false;
        public bool DataValid
        {
            get { return dataValid; }
            set { dataValid = value; }
        }
        //DataAnalyse dataAnalyse = null;
        ReportDrawManager drawManager = null;
        public Graphics GraphicsBuffer
        {
            get { return drawManager.GraphicsBuffer; }
            set { drawManager.GraphicsBuffer = value; }
        }
        DataAnalyse DataAnalyse = null;
        public ReportDrawStruct WholeDrawStruct
        {
            get { return DataAnalyse.drawStruct; }
        }
        public int TatalPage
        {
            get { return WholeDrawStruct == null ? 0 : WholeDrawStruct.PageCount; }
        }
        private ReportClass rptClass = null;
        public ReportClass RptClass
        {
            get { return rptClass; }
            set { rptClass = value; }
        }
        private string reportName = string.Empty;
        public string ReportName
        {
            get
            {
                return attributes.ReportName;
            }
            set
            {
                attributes.ReportName = value;
            }
        }
        private string desrcName = string.Empty;
        public string DesrcName
        {
            get
            {
                if (string.IsNullOrEmpty(desrcName))
                {
                    desrcName = reportName;
                }
                return desrcName;
            }
            set { desrcName = value; }
        }
        private int desrcLevel = 0;
        public int DesrcLevel
        {
            get { return desrcLevel; }
        }
        private ReportAttribute attributes = null;
        public ReportAttribute Attributes
        {
            get { return attributes; }
            set
            {
                attributes = value;
            }
        }
        private ReportColumns columns = null;
        public ReportColumns Columns
        {
            get { return columns; }
            set { columns = value; }
        }
        private Bands bands = null;
        public Bands Bands
        {
            get { return bands; }
            set { bands = value; }
        }
        private Texts texts = null;
        public Texts Texts
        {
            get { return texts; }
            set { texts = value; }
        }
        public int RowHeight
        {
            get { return (int)(attributes.Font.Height * attributes.RowHeiht); }
        }
        internal int totalWidth = 0;
        public int TotalWidth
        {
            get
            {
                return (attributes.GrainDirection == 1) ?
                totalWidth : totalHeight;
            }
        }

        internal int totalHeight = 0;
        public int TotalHeight
        {
            get
            {
                return (attributes.GrainDirection == 1) ?
                totalHeight : totalWidth;
            }
        }

        public int AreaWidth
        {
            get
            {
                int areaWidth = getAreaWidth();
                return areaWidth;
            }
        }
        public int AreaHeight
        {
            get
            {
                int areaHeight = getAreaHeight();
                return areaHeight;
            }
        }
        //根椐报表属性来获取实际宽度与长宽
        private int getAreaWidth()
        {
            return TotalWidth - this.Attributes.Margin.Left - this.Attributes.Margin.Right;
        }
        private int getAreaHeight()
        {
            return TotalHeight - this.Attributes.Margin.Top - this.Attributes.Margin.Bottom;
        }
        internal SizeF GetExcelSizeF()
        {
            //EXCEL中打印，获取报表的上下左右边距，计算时需要扣除
            int tbMargin = (this.Attributes.Margin.Top + this.Attributes.Margin.Bottom) / 2;
            int lrMargin = (this.attributes.Margin.Left + this.attributes.Margin.Right) / 2;

            float h = attributes.GrainDirection == 1 ?
                AreaWidth / ((float)ExcelPagingWidth - lrMargin) : AreaWidth / ((float)ExcelPagingHeight - lrMargin);
            float v = attributes.GrainDirection == 1 ?
                AreaHeight / ((float)ExcelPagingHeight - tbMargin) : AreaHeight / ((float)ExcelPagingWidth - tbMargin);
            return new SizeF(h, v);
        }
        private MutliProjectManager projectDataManager = null;
        public MutliProjectManager ProjectDataManager
        {
            get { return projectDataManager; }

        }
        public int MutliProjectCount
        {
            get { return projectDataManager[desrcLevel].Count; }
        }
        private ReportData data = null;
        public ReportData Data
        {
            get { return data; }
            set { data = value; }
        }
        private DataTable originalData = new DataTable();
        public DataTable OriginalData
        {
            get { return originalData; }
            set { originalData = value; }
        }
        public Report()
        {
            ReInit();
        }
        public Report(string rptFile, DataTable table)
        {
            ReInit();
            this.RptFilePath = rptFile;

            this.ReadRPT();
            ReportData reportData = new ReportData(this);
            reportData.InitReportData(table);
            this.InitReportData(reportData);
        }
        public Report(string desrcName, int level)
            : this()
        {
            this.desrcLevel = level;
            this.desrcName = desrcName;
        }
        public void SetPrintProperty()
        {
            if (PrintUtil.PrintDlg.PrinterSettings.IsValid)
            {
                if (PrintUtil.PaperSize == null)
                {
                    PrintUtil.PaperSize = PrintUtil.PrintDlg.PrinterSettings.DefaultPageSettings.PaperSize;
                }
                totalWidth = PrintUtil.PaperSize.Width;
                totalHeight = PrintUtil.PaperSize.Height;
            }
            else
            {
                totalWidth = REPORTTOTALWIDTH;
                totalHeight = REPORTTOTALHEIGHT;
            }
            //以10陪数误差检测是否出现读取打印机信息异常的情况
            if (totalHeight > REPORTTOTALWIDTH * 10
                || totalHeight > REPORTTOTALHEIGHT * 10)
            {
                totalWidth = REPORTTOTALWIDTH;
                totalHeight = REPORTTOTALHEIGHT;
                //RecordError:检测不到打印机纸张设置
            }
        }
        internal void RaisePropertyChanged()
        {
            if (PropertyChanged != null && propertyValid)
            {
                InitReportDataByPrjIndex(0);
                PropertyChanged(this, new EventArgs());
            }
        }
        internal void RaiseDataChanged()
        {
            if (DataChanged != null && DataValid)
            {
                DataChanged(this, new EventArgs());
            }
        }
        #region 封装打印与读报表类
        private ReadOldRPT reportReader = null;
        private WriteRpt reportWriter = null;

        private string rptFilePath = string.Empty;
        public string RptFilePath
        {
            get { return rptFilePath; }
            set
            {
                rptFilePath = value;
                if (rptFilePath.ToLower().EndsWith(".doc"))
                {
                    docPath = RptFilePath;
                    rptFilePath = Path.Combine(Function.ReportFolder, "report_WORD报表模板.rpt");
                    dllPath = Path.Combine(Function.ReportFolder, "report_项目工程宏变量列表.dll");
                }
                else
                {
                    //报表路径定向
                    dllPath = Function.ReportFolder + Function.ProfileString(RptFilePath, "SOURCE",
                        "dll", string.Format("{0}.{1}", Path.GetFileNameWithoutExtension(RptFilePath), "dll"));
                    docPath = Function.ReportFolder + Function.ProfileString(RptFilePath, "SOURCE",
                        "doc", string.Format("{0}.{1}", Path.GetFileNameWithoutExtension(RptFilePath), "doc"));
                    if (!File.Exists(docPath))
                    {
                        docPath = "";
                    }
                }
            }
        }
        public void WriteRpt()
        {
            WriteRpt(true);
        }

        public void WriteRpt(bool needReCalc)
        {
            reportWriter.Write();
            if (needReCalc)
            {
                RaisePropertyChanged();
            }
        }
        public void Restore()
        {
            ReInit();
            ReadRPT();
            InitReportDataByPrjIndex(0);
        }
        public void ReadRPT()
        {
            //可以进行相应异常处理及定位
            try
            {
                reportReader.Read();
                SetPrintProperty();
                propertyValid = true;
            }
            catch (ReportIOException e)
            {
                if (Function.DebugMode)
                {
                    Function.RecordError(string.Format("报表文件名:{0},格式错误信息:｛1｝",
                     this.RptFilePath,
                     e.Message));
                }
                else
                {
                    Function.Alert(e.Message, "提示");
                }
                propertyValid = false;
            }
            //catch (Exception e)
            //{
            //    propertyValid = false;
            //throw e;
            //}
        }

        public void ReadRPT(string fileName)
        {
            RptFilePath = rptPathRedirect(fileName);
            ReadRPT();
        }
        private string rptPathRedirect(string fileName)
        {
            //读系统还是读系统的报表
            string filePath = "";
            if (File.Exists(Function.UserReportFolder + fileName))
            {
                filePath = Function.UserReportFolder + fileName;
            }
            else
            {
                filePath = Function.ReportFolder + fileName;
                if (!File.Exists(filePath))
                {
                    filePath = string.Empty;
                }
            }
            return filePath;
        }
        public void ReInit()
        {
            texts = new Texts(this);
            columns = new ReportColumns(this);
            reportReader = new ReadOldRPT(this);
            reportWriter = new WriteRpt(this);
            attributes = new ReportAttribute(this);
            drawManager = new ReportDrawManager(this);
            DataAnalyse = new DataAnalyse(this);
            bands = new Bands();
            propertyValid = false;
            data = null;

            //projectDataManager = null;
        }
        #endregion
        #region 报表的数据读取
        public void InitReportData(ReportData reportData)
        {
            this.data = reportData;
            if (data != null && reportData.BodyData != null)
            {
                dataValid = true;
                RaiseDataChanged();
            }
        }
        public void InitReportData(MutliProjectManager projectDataManager, int prjIndex)
        {
            this.projectDataManager = projectDataManager;
            InitReportDataByPrjIndex(prjIndex);
        }
        public void InitReportData(DataSet projectData, Dictionary<string, object> exParams)
        {
            if (this.propertyValid)
            {
                try
                {
                    SqliteData sqliteData = new SqliteData(this);
                    data = sqliteData.GetData(projectData, exParams);
                    if (data != null)
                    {
                        verifyDataTable();
                        dataValid = true;
                    }
                    else
                    {
                        dataValid = false;
                    }
                }
                catch
                {
                    dataValid = false;
                    throw;
                }
            }
            if (dataValid)
            {
                RaiseDataChanged();
            }
        }
        public void InitReportDataByPrjIndex(int prjIndex)
        {
            if (projectDataManager != null && propertyValid)
            {
                SqliteData sqliteData = new SqliteData(this);
                try
                {
                    if (prjIndex == -1)
                    {
                        data = sqliteData.GetData(projectDataManager.ProjectData);
                    }
                    else
                    {
                        if (projectDataManager.StartCheckProject(desrcLevel, prjIndex))
                        {
                            data = sqliteData.GetData(projectDataManager.ProjectData);
                            projectDataManager.EndCheckProject();
                        }
                    }

                    if (data != null)
                    {
                        verifyDataTable();
                        dataValid = true;
                    }
                    else
                    {
                        dataValid = false;
                    }
                }
                catch (ReportReadDataException readDataException)
                {
                    Function.Alert(readDataException.Message, "提示");
                    dataValid = false;
                }
                catch (Exception e)
                {
                    dataValid = false;
                    throw e;
                }
                if (dataValid)
                {
                    RaiseDataChanged();
                }
            }
        }
        /// <summary>
        /// 验证报表数据的有效性
        /// </summary>        
        public bool verifyDataTable()
        {
            //验证数据表            
            bool verify = true;
            //验证dataTable与报表属性是否一致                     
            foreach (ReportColumn col in columns)
            {
                if (col.ColumnVisible && !col.Attibutes.DynamicColumn && !data.BodyData.Columns.Contains(col.ColumnName))
                {
                    Function.Alert(string.Format("报表设计的列{0}在报表数据Table未找到!", col.ColumnName), "提示");
                    verify = false;
                    break;
                }
            }
            return verify;
        }

        #endregion
        public override string ToString()
        {
            StringBuilder reportStr = new StringBuilder();
            reportStr.AppendLine(ReadOldRPT.DESIGNNODE);
            reportStr.AppendLine(rptClass.ToString());
            reportStr.AppendLine(attributes.ToString());
            reportStr.AppendLine(columns.ToString());
            reportStr.AppendLine(bands.ToString());
            reportStr.AppendLine(string.Format("d005={0}", texts.Count));
            //TEXT
            for (int textIndex = 0; textIndex < texts.Count; textIndex++)
            {
                reportStr.AppendLine(string.Format("d005{0:D4}={1}", textIndex + 1, texts[textIndex].ToString()));
            }
            //一些附加信息，如参数设置等 
            reportStr.AppendLine(string.Join("\r\n", reportReader.RptExInfo().ToArray()));
            return reportStr.ToString();
        }
        #region

        public void ChangeProperty()
        {
            ReportProperty rProperty = new ReportProperty(this);
            rProperty.ShowDialog();
        }

        internal Dictionary<string, string> GetParam()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(RptFilePath))
            {
                string reportSysIni = Function.TempFolder + "ReportArgs.ini";

                Function.WriteSectionToFile(RptFilePath, "PARAMETER", reportSysIni, "[报表参数]", "[参数设置结果]");
                string[] sections = Function.ProfileSectionNames(reportSysIni);
                dic = Function.ProfileDictionary(reportSysIni, RptFilePath, sections);
                foreach (KeyValuePair<string, string> pair in Function.ProfileSection(Function.IniFile, "环境设置"))
                {
                    dic[pair.Key] = pair.Value;
                }
                foreach (KeyValuePair<string, string> pair in Function.ProfileSection(RptFilePath, "参数设置结果"))
                {
                    dic[pair.Key] = pair.Value;
                }
            }
            return dic;
        }
        internal void ChangeArgs()
        {
            string reportSysIni = Function.TempFolder + "ReportArgs.ini";
            string[] sections = Function.ProfileSectionNames(reportSysIni);
            Com.Bing.Forms.CommonConfigDialog dlg = new Com.Bing.Forms.CommonConfigDialog(
                reportSysIni, this.RptFilePath, sections, "报表参数设置");
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.ReInit();
                this.ReadRPT();
                this.WriteRpt();
                //cjl 20120412 重复工作
                //this.InitReportData(projectDataManager, 0);
                this.PropertyChanged(this, new EventArgs());
            }
        }

        public Image SpeedPreview(int pageIndex)
        {
            if (!propertyValid || !dataValid)
            {
                return null;
            }
            //drawManager.Draw(Point.Empty, pageIndex);
            //实际需要的报表显示值
            Bitmap image = new Bitmap(TotalWidth, TotalHeight,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                    );
            Graphics imageGraphics = Graphics.FromImage(image);
            imageGraphics.Clear(Color.White);
            //将报表的图像画到Image上
            drawManager.Draw(new Point(1, 1), pageIndex);
            Bitmap bitMapBuffer = drawManager.BitMapBuffer;
            imageGraphics.DrawImage(bitMapBuffer,
                new RectangleF(Attributes.Margin.Left,
                                Attributes.Margin.Top,
                                TotalWidth,
                                TotalHeight),
                new RectangleF(0, 0, bitMapBuffer.Width, bitMapBuffer.Height),
                GraphicsUnit.Pixel
                );
            //画出显示边框
            imageGraphics.DrawRectangle(new Pen(Color.Blue), new Rectangle(1, 1, bitMapBuffer.Width - 2, bitMapBuffer.Height - 2));
            imageGraphics.Dispose();
            return image;
        }
        internal void MutliProjectSendToExcel(string xlsFile)
        {
            for (int i = 0; i < MutliProjectCount; i++)
            {
                DataAnalyse.Save();
                StringBuilder vbaBuilder = new StringBuilder();
                InitReportDataByPrjIndex(i);
                drawManager.Send(vbaBuilder, true);
                XLSFile.Generate(xlsFile, vbaBuilder, DesrcName);
                DataAnalyse.Cover();
            }
        }
        public void SendToExcel(string xlsFile)
        {
            if (string.IsNullOrEmpty(docPath))
            {
                StringBuilder vbaBuilder = new StringBuilder();
                drawManager.Send(vbaBuilder);
                XLSFile.Generate(xlsFile, vbaBuilder, DesrcName);
            }
        }
        internal void SendToExcelAsPart()
        {
            StringBuilder vbaBuilder = new StringBuilder();
            drawManager.Send(vbaBuilder);
            XLSFile.Generate(vbaBuilder, DesrcName);
        }
        public void EditData(int curPage)
        {
            DataTable dataTable = this.WholeDrawStruct.GetPageData(curPage);
            PageInfo pageInfo = this.WholeDrawStruct.GetPageInfo(curPage);
            EditReportData dlg = new EditReportData(dataTable, pageInfo);
            dlg.ShowDialog();
        }
        public void Design()
        {
            try
            {
                ReportStyle reportStyle = new ReportStyle(this);
                reportStyle.ShowDialog();
            }
            catch (DataAnaysisException e)
            {
                Function.Alert(e.Message, "提示");
            }
        }
        #endregion
        #region 报表文件的相关操作

        internal void CoverUsrRpt()
        {
            if (RptFilePath.Contains(Function.UserReportFolder))
            {
                File.Delete(RptFilePath);
                //获取该报表的原始路径
                string fileNameWithOutSuffix = Path.GetFileNameWithoutExtension(RptFilePath);
                string suffix = "rpt";
                string fileNameWithSuffix = string.Format("{0}.{1}", fileNameWithOutSuffix, suffix);
                this.ReInit();
                ReadRPT(fileNameWithSuffix);
                InitReportDataByPrjIndex(1);

            }
        }
        internal void CopyRpt()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = Path.GetFileName(this.RptFilePath);
            dlg.InitialDirectory = Function.MakeFolder(Function.UserReportFolder + "用户自定义\\");
            dlg.DefaultExt = ".rpt";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string newRptFile = dlg.FileName;
                File.Delete(newRptFile);
                reportWriter.Write(newRptFile);
            }
        }
        internal void DeleteRpt()
        {
            if (Function.Confirm("删除的报表将不能恢复,确定删除?", "提示"))
            {
                if (string.IsNullOrEmpty(this.RptFilePath))
                {
                    Function.Alert("不存在此报表", "提示");
                }
                else if (this.RptFilePath.Contains(Function.UserDataFolder) || this.RptFilePath.Contains("用户自定义"))
                {
                    //未判段是否为自定义报表
                    File.Delete(this.RptFilePath);
                    Function.Alert("删除成功", "提示");

                }
                else
                {
                    Function.Alert("非自定义报表不能删除!", "提示");
                }
            }
        }

        #endregion
        internal void MutliProjectPrint()
        {
            for (int i = 0; i < MutliProjectCount; i++)
            {
                DataAnalyse.Save();
                PrintUtil.PrintDocument.PrintPage += PrintDocument_PrintPage;
                InitReportDataByPrjIndex(i);
                printPageIndex = 0;
                PrintUtil.PrintDocument.DefaultPageSettings.Landscape = Attributes.GrainDirection == 2;
                PrintUtil.PrintDocument.Print();
                PrintUtil.PrintDocument.PrintPage -= PrintDocument_PrintPage;
                DataAnalyse.Cover();
            }
        }
        public void Print()
        {


            if (IsDoc)
                ReportHelper.PrintWord(docPath, Data.MacorsVerbCollection);
            else
            {
                if (this.WholeDrawStruct == null)
                {
                    Function.Alert("报表数据异常！", "提示");
                    return;
                }
                PrintUtil.PrintDocument.PrintPage += PrintDocument_PrintPage;
                printPageIndex = 0;
                PrintUtil.PrintDocument.DefaultPageSettings.Landscape = Attributes.GrainDirection == 2;
                PrintUtil.PrintDocument.Print();
                PrintUtil.PrintDocument.PrintPage -= PrintDocument_PrintPage;
            }
        }
        int printPageIndex = 0;
        void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            PrinterSettings settings = (sender as PrintDocument).PrinterSettings;
            //传入打印机的右上可打印区
            Point point = new Point(Attributes.Margin.Left - Convert.ToInt32(PrintUtil.PrintDocument.DefaultPageSettings.PrintableArea.Left),
                Attributes.Margin.Top - Convert.ToInt32(PrintUtil.PrintDocument.DefaultPageSettings.PrintableArea.Top));
            printPageIndex++;
            if (settings.PrintRange == PrintRange.SomePages && (printPageIndex < settings.FromPage || printPageIndex > settings.ToPage))
            {
                e.Cancel = true;
                return;
            }
            //批量打印如设置编页，则报表的位置参数不为0
            e.Graphics.Clear(Color.White);
            //学习版水印处理
            PrintUtil.PrintWatermark(e.Graphics, this);
            drawManager.Draw(new IGraphics(e.Graphics), point, printPageIndex);

            if (printPageIndex >= TatalPage || (settings.PrintRange == PrintRange.SomePages && printPageIndex == settings.ToPage + 1))
            {
                e.HasMorePages = false;
            }
            else
            {
                e.HasMorePages = true;
            }
        }

        public void Dispose()
        {
            drawManager.Dispose();
        }
    }

}
