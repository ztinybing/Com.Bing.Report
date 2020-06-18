using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using Com.Bing.API;
using Com.Bing.Business;
using Com.Bing.Forms;

namespace Com.Bing.Report
{
    public partial class PreviewReport : BaseDialog
    {
        public sealed class CursorsEx
        {
            System.ComponentModel.ComponentResourceManager resources = null;
            public static Cursor Hand
            {
                get
                {
                    return new CursorsEx("hand").cursor;
                }
            }
            public static Cursor HandGrab
            {
                get { return new CursorsEx("handgrab").cursor; }
            }
            Dictionary<string, Cursor> Cursors = new Dictionary<string, Cursor>();
            public Cursor cursor = null;
            private CursorsEx(string name)
            {
                resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewReport));
                if (!Cursors.ContainsKey(name))
                {
                    byte[] fileByte = (byte[])resources.GetObject(name);
                    if (fileByte == null)
                    {
                        throw new CursorFileNotExistsException(name);
                    }
                    MemoryStream memoryStream = new MemoryStream(fileByte.Length);
                    memoryStream.Write(fileByte, 0, fileByte.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    Cursors.Add(name, new Cursor(memoryStream));
                    memoryStream.Close();
                }
                cursor = Cursors[name];
            }
        }
        ImageZoom imageZoom = null;

        private int curPage = 1;
        public int CurPage
        {
            get { return curPage; }
            set
            {
                curPage = value;
                onPageChande();
            }
        }
        private int prjIndex = 0;
        Report report = null;
        public PreviewReport()
        {
            InitializeComponent();

        }
        public PreviewReport(Report report)
        {
            this.report = report;
            InitializeComponent();
            sendToXls.Visible = true;
            imageZoom = new ImageZoom(new Size(report.TotalWidth, report.TotalHeight), panel1);
            initImage();
            //图像双缓冲
            //base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            //base.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            //多工程管理器为空，则表明为预览查看单工程报表，则不显示 上下工程
            if (report.ProjectDataManager == null)
            {
                this.preProject.Visible = false;
                this.nextProject.Visible = false;
            }
        }
        public void initImage()
        {
            pictureBox.Image = report.SpeedPreview(CurPage);
            pictureBox.Dock = DockStyle.None;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Size = imageZoom.GetZoomSize();
            SetMemoInfo(imageZoom.ZoomRate);
            SetPictureBoxLocation(pictureBox.Size, panel1);
            this.pictureBox.Cursor = CursorsEx.Hand;
        }
        void SetMemoInfo(object echoRate)
        {
            toolStripStatusLabel2.Text = string.Format("{0}/{1}||  {2}%", curPage, report.TatalPage, echoRate);
        }
        private void SetPictureBoxLocation(Size size, Panel panel)
        {
            int pictureX = 0;
            int pictrueY = 0;
            pictureX = (panel.Width - size.Width) / 2;
            pictrueY = (panel.Height - size.Height) / 2;
            if (pictureX < 0)
            {
                pictureX = 20;
            }
            if (pictrueY < 0)
            {
                pictrueY = 20;
            }
            //这个地方也没对，一直加            
            pictureBox.Location = new Point(pictureX - panel.HorizontalScroll.Value, pictrueY - panel.VerticalScroll.Value);
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                imageZoom.ChangeZoomModule();
                pictureBox.Size = imageZoom.GetZoomSize();
                SetMemoInfo(imageZoom.ZoomRate);
                SetPictureBoxLocation(pictureBox.Size, panel1);
            }

        }

        #region 按钮事件处理函数

        private void toolClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void firstPage_Click(object sender, EventArgs e)
        {
            CurPage = 1;
        }
        private void prePage_Click(object sender, EventArgs e)
        {
            if (CurPage > 1)
            {
                CurPage -= 1;
            }
        }
        private void nextPage_Click(object sender, EventArgs e)
        {
            if (CurPage < report.TatalPage)
            {
                CurPage += 1;
            }
        }
        private void lastPage_Click(object sender, EventArgs e)
        {
            CurPage = report.TatalPage;
        }
        private void onPageChande()
        {
            initImage();
            OnResize(new EventArgs());
            toolStripStatusLabel1.Text = "第 " + curPage + " 页";
        }
        private void printPage_Click(object sender, EventArgs e)
        {
            if (PrintUtil.ShowPrintDlg() == DialogResult.OK)
            {
                if (report.ProjectDataManager != null)
                    report.MutliProjectPrint();
                else
                    report.Print();
            }
        }
        private void sendToXls_Click(object sender, EventArgs e)
        {
            string xlsFilePath = "";
            if (XLSFile.ShowSaveFile(ref xlsFilePath, report.DesrcName))
            {
                if(report.ProjectDataManager != null)
                    report.MutliProjectSendToExcel(xlsFilePath);
                else
                    report.SendToExcel(xlsFilePath);

                Function.Alert("发送完成!", "提示");
            }

        }
        //上一工程
        private void preProject_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (prjIndex > 0)
                {
                    report.InitReportDataByPrjIndex(--prjIndex);
                    CurPage = 1;
                }
            }
        }
        //下一工程
        private void nextProject_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (prjIndex < report.MutliProjectCount - 1)
                {
                    report.InitReportDataByPrjIndex(++prjIndex);
                    CurPage = 1;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            report.EditData(curPage);
            initImage();
        }

        private void PreviewReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.reportCenter.ClearDataContentBuffer();
        }

        #endregion

        #region 鼠标事件处理函数 a鼠标左键拖动报表的位置 b.鼠标中缩放报表

        private Point mouseLeftDownLocation = Point.Empty;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseLeftDownLocation = Control.MousePosition;
                pictureBox.Cursor = CursorsEx.HandGrab;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseLeftDownLocation = Point.Empty;
                pictureBox.Cursor = CursorsEx.Hand;
            }
        }

        List<Point> moveExensionBuffer = new List<Point>();
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point moveExtension = Point.Empty;
            lock (moveExensionBuffer)
            {
                if (checkDrag())
                {
                    //移动偏移量
                    moveExtension = new Point(-mouseLeftDownLocation.X, -mouseLeftDownLocation.Y);
                    moveExtension.Offset(Control.MousePosition);
                    mouseLeftDownLocation = Control.MousePosition;
                    moveExensionBuffer.Add(moveExtension);
                    SetScrollBarMove(1);
                    this.Invalidate();
                }
            }
        }
        private void SetScrollBarMove(int bufferCount)
        {
            lock (moveExensionBuffer)
            {
                if (moveExensionBuffer.Count > bufferCount)
                {
                    Point moveExension = Point.Empty;
                    foreach (Point point in moveExensionBuffer)
                    {
                        moveExension.Offset(point);
                    }

                    if (Math.Abs(moveExension.X) > 5 || Math.Abs(moveExension.Y) > 5)
                    {
                        moveExensionBuffer.Clear();
                        SetScrollBarMove(panel1.HorizontalScroll, moveExension.X);
                        SetScrollBarMove(panel1.VerticalScroll, moveExension.Y);
                    }
                }
            }
        }
        private void SetScrollBarMove(ScrollProperties scroll, int moveDistanse)
        {
            int moveResult = scroll.Value - moveDistanse;
            if (moveResult < 0)
            {
                moveResult = 0;
            }
            else if (moveResult > scroll.Maximum)
            {
                moveResult = scroll.Maximum;
            }
            scroll.Value = moveResult;
        }
        private bool checkDrag()
        {
            //左键是否被按下
            bool leftButtonDown = mouseLeftDownLocation != Point.Empty;
            //panel是否存在滚动条
            bool existsScorllBar = panel1.HorizontalScroll.Visible || panel1.VerticalScroll.Visible;
            //若右键被按下并且存在滚动条 , 则可拖动
            return leftButtonDown && existsScorllBar;
        }

        void pictureBox1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int wheelIndex = e.Delta;
            imageZoom.Zoom(wheelIndex);
            pictureBox.Size = imageZoom.GetZoomSize();
            SetMemoInfo(imageZoom.ZoomRate);
            SetPictureBoxLocation(pictureBox.Size, panel1);
        }

        #endregion
        private void PreviewReport_Shown(object sender, EventArgs e)
        {
            SetPictureBoxLocation(pictureBox.Size, panel1);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            //设置焦点，使pictureBox获取到中滚动的事件
            pictureBox.Focus();
        }
        private void propertyChang_Click(object sender, EventArgs e)
        {
            report.ChangeProperty();
            initImage();
        }
        private void toolStrip1_SizeChanged(object sender, EventArgs e)
        {
            SetPictureBoxLocation(pictureBox.Size, panel1);
        }
        void designReport_Click(object sender, System.EventArgs e)
        {
            report.Design();

            initImage();
        }

    }

    public class ImageZoom
    {
        enum ZoomModule
        {
            NormalPage, //正常比例100%
            ZoomPage,   //按比例缩放
            SinglePage  //单页
        }
        struct ModuleAndRate
        {
            public ZoomModule zoomModule;
            public double zoomRate;
            public ModuleAndRate(ZoomModule _zoomModule, double _zoomRate)
            {
                zoomModule = _zoomModule;
                zoomRate = _zoomRate;
            }
        }

        ZoomModule curZoomModule = ZoomModule.NormalPage;
        double _zoomRate = 0.0;
        //当前的缩放比例
        public double zoomRate
        {
            get
            {
                switch (curZoomModule)
                {
                    case ZoomModule.NormalPage:
                        _zoomRate = 1;
                        break;
                    case ZoomModule.SinglePage:
                        _zoomRate = SignlePageRatio;
                        break;
                }
                return _zoomRate;
            }
            set
            {
                _zoomRate = value;
            }
        }
        public double ZoomRate
        {
            get { return zoomRate * 100; }
        }
        private Size imageSize = Size.Empty;
        private Control container = null;
        Size containerSize
        {
            get { return container.Size; }
        }
        public const double NormalRatio = 1;
        public double SignlePageRatio = 0.0;
        public ImageZoom(Size imageSize, Control container)
        {
            this.imageSize = imageSize;
            this.container = container;
            container.SizeChanged += new EventHandler(container_SizeChanged);
        }

        void container_SizeChanged(object sender, EventArgs e)
        {
            InitSignleZoomModule();
        }
        /// <summary>
        /// 初始化单页缩放比率
        /// </summary>
        private void InitSignleZoomModule()
        {
            double widthZoomRate = containerSize.Width / (double)imageSize.Width;
            double heightZoomRate = (containerSize.Height - 40) / (double)imageSize.Height;
            SignlePageRatio = widthZoomRate > heightZoomRate ? heightZoomRate : widthZoomRate;
            //保留两位小数并取小
            SignlePageRatio = Math.Round(SignlePageRatio, 2, MidpointRounding.ToEven);
        }
        List<ModuleAndRate> recordZoom = new List<ModuleAndRate>();
        public Size GetZoomSize()
        {
            Size zoomSize = Size.Empty;
            zoomSize = CalaZoomSize(zoomRate);
            //记录该次显示的模式与显示比率
            recordZoom.Add(new ModuleAndRate(curZoomModule, zoomRate));
            return zoomSize;
        }

        private Size CalaZoomSize(double ratio)
        {
            double width = imageSize.Width * ratio;
            double height = imageSize.Height * ratio;
            return new Size(Convert.ToInt32(Math.Floor(width)), Convert.ToInt32(Math.Floor(height)));
        }

        internal void Zoom(int wheelIndex)
        {
            //每次滚动的该度为120 在这里规定 每次增加/减少0.01
            double zoomResult = zoomRate;
            if (wheelIndex > 0)
            {
                zoomResult += 0.02;
            }
            else
            {
                zoomResult -= 0.02;
            }
            if (zoomResult == 1)
            {
                curZoomModule = ZoomModule.NormalPage;
            }
            else if (zoomResult == SignlePageRatio)
            {
                curZoomModule = ZoomModule.SinglePage;
            }
            else if (zoomResult >= 0.48 && zoomResult < 2.02)
            {
                curZoomModule = ZoomModule.ZoomPage;
                zoomRate = zoomResult;
            }
        }
        /// <summary>
        /// 改变当前显示模式为前一个显示模式        
        /// </summary>
        internal void ChangeZoomModule()
        {
            if (curZoomModule != ZoomModule.NormalPage)
            {
                curZoomModule = ZoomModule.NormalPage;
            }
            else
            {
                if (recordZoom.Count == 1)
                {
                    curZoomModule = ZoomModule.SinglePage;
                }
                else
                {
                    ModuleAndRate preModuleAndRate = recordZoom[recordZoom.Count - 2];
                    if (preModuleAndRate.zoomModule == ZoomModule.ZoomPage && preModuleAndRate.zoomRate != 1)
                    {
                        curZoomModule = preModuleAndRate.zoomModule;
                        _zoomRate = preModuleAndRate.zoomRate;

                    }
                    else
                    {
                        curZoomModule = ZoomModule.SinglePage;
                    }
                }
            }
        }
    }
}
public class CursorFileNotExistsException : Exception
{
    public override string Message
    {
        get
        {
            return string.Format("光标文件不存在.相关信息:\r\n{0}", fileName);
        }
    }
    string fileName = string.Empty;
    public CursorFileNotExistsException(string fileName)
    {
        this.fileName = fileName;
    }
}