using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTreeList.Nodes;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using System.IO;
using Com.Bing.API;
namespace Com.Bing.Report
{
    public partial class ReportBatchSet : DevExpress.XtraEditors.XtraForm
    {
        //Field
        private PrintList printListData = new PrintList();
        private bool selectAllStatus = false;
        private Dictionary<string, TreeListNode> namdAndNodeDict = new Dictionary<string, TreeListNode>();
        //Method
        public ReportBatchSet()
        {
            InitializeComponent();
            #region 控件的一些初始化设置
            this.Text = "报表批量打印设置";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.startPageNum.Text = "1";
            this.ckCatalogue.Checked = Com.Bing.API.Function.ProfileString(Com.Bing.API.Function.IniFile, "Report", "PrintCatalogue", "0") == "0" ? false : true;
            this.calcTotalPage.Checked = Com.Bing.API.Function.ProfileString(Com.Bing.API.Function.IniFile, "Report", "CalcTotalPageNum", "0") == "0" ? false : true;
            this.projectCatalog.Checked = Com.Bing.API.Function.ProfileString(Com.Bing.API.Function.IniFile, "Report", "projectCatalog", "1") == "0" ? false : true;
            this.projectPDF.Checked = Com.Bing.API.Function.ProfileString(Com.Bing.API.Function.IniFile, "Report", "projectPDF", "0") == "0" ? false : true;
            printListData.PrintCatalogue = ckCatalogue.Checked;
            printListData.CalcTotalPageNum = calcTotalPage.Checked;
            printListData.ProjectCatalog = projectCatalog.Checked;
            printListData.ProjectPDF = projectPDF.Checked;
            #endregion
        }
        public ReportBatchSet(DataTable table)
            : this()
        {
            initReportList(table);
            initPrintList();
            initEvent();
        }

        private void initEvent()
        {
            this.batchRightMove.Click += new EventHandler(batchRightMove_Click);
            this.batchLeftMove.Click += new EventHandler(batchLeftMove_Click);
            this.btnOk.Click += new EventHandler(btnOk_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.Load += new EventHandler(ReportBatchSet_Load);
        }

        void ReportBatchSet_Load(object sender, EventArgs e)
        {
            ReportList.ExpandAll();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void initPrintList()
        {

            GridColumn gridColumn = new GridColumn();
            gridColumn = new GridColumn();
            gridColumn.Caption = "目录";
            gridColumn.FieldName = "ReportName";
            gridColumn.Name = "reportName";
            gridColumn.Visible = true;
            gridColumn.VisibleIndex = 0;
            gridColumn.Width = 360;
            gridColumn.OptionsColumn.AllowEdit = false;
            gridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            printListView.Columns.Add(gridColumn);

            gridColumn = new GridColumn();
            gridColumn.Caption = "是否编页";
            gridColumn.FieldName = "Pagination";
            gridColumn.Name = "paging";
            gridColumn.Visible = true;
            gridColumn.VisibleIndex = 1;
            gridColumn.Width = 100;

            gridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;

            printListView.Columns.Add(gridColumn);

            printListView.IndicatorWidth = 40;
            printListView.OptionsView.ShowGroupPanel = false;

            printList.AllowDrop = true;
            printList.DataSource = printListData;
            printListView.OptionsSelection.MultiSelect = true;
            printListView.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(printListView_CustomDrawRowIndicator);


            printList.MouseDown += new MouseEventHandler(printListView_MouseDown);
            printList.MouseMove += new MouseEventHandler(printListView_MouseMove);
            printList.DragDrop += new DragEventHandler(printList_DragDrop);
            printList.DragOver += new DragEventHandler(printList_DragOver);
            printList.DoubleClick += new EventHandler(printList_DoubleClick);
        }

        void printList_DoubleClick(object sender, EventArgs e)
        {

            DXMouseEventArgs args = e as DXMouseEventArgs;
            GridHitInfo info = printListView.CalcHitInfo(args.X, args.Y);
            if (info == null)
                return;
            if (info.RowHandle == printListView.FocusedRowHandle)
            {
                int targetIndex = printListView.GetDataSourceRowIndex(printListView.FocusedRowHandle);
                if (targetIndex >= 0)
                    printListData.RemoveAt(targetIndex);
            }
            if (info.InColumnPanel && info.Column.Name == "reportName")
            {
                printListView.SelectAll();
            }
            if (info.InColumnPanel && info.Column.Name == "paging")
            {

                foreach (PrintElement item in printListData)
                {
                    if (selectAllStatus)
                    {
                        item.Pagination = false;

                    }
                    else
                    {
                        item.Pagination = true;
                    }
                }
                printListView.RefreshData();
                selectAllStatus = !selectAllStatus;
            }
        }

        void printList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void printList_DragDrop(object sender, DragEventArgs e)
        {
            GridHitInfo targetInfo = printListView.CalcHitInfo(printList.PointToClient(new Point(e.X, e.Y)));
            GridHitInfo sourceInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
            if (targetInfo.RowHandle < 0 || targetInfo.RowHandle == sourceInfo.RowHandle)
                return;
            int targetIndex = printListView.GetDataSourceRowIndex(targetInfo.RowHandle);
            int sourceIndex = printListView.GetDataSourceRowIndex(sourceInfo.RowHandle);
            PrintElement copyobj = printListData[sourceIndex].Clone() as PrintElement;
            printListData.RemoveAt(sourceIndex);
            if (targetIndex > sourceIndex)
            {
                printListData.Insert(targetIndex - 1, copyobj);
            }
            else
            {
                printListData.Insert(targetIndex, copyobj);
            }
        }

        void printListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (info == null || e.Button != MouseButtons.Left)
                return;
            printList.DoDragDrop(info, DragDropEffects.Move);
        }
        GridHitInfo info = null;
        void printListView_MouseDown(object sender, MouseEventArgs e)
        {
            info = printListView.CalcHitInfo(e.X, e.Y);
        }

        void printListView_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
                e.Info.DisplayText = (1 + e.RowHandle).ToString();
        }
        private void initReportList(DataTable table)
        {
            ReportList.DataSource = table;
            ReportList.Columns["src"].Visible = false;
            ReportList.Columns["level"].Visible = false;
            ReportList.Columns["name"].Caption = "报表列表";
            ReportList.BestFitColumns();
            ReportList.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(ReportList_AfterCheckNode);
            ReportList.DoubleClick += new EventHandler(ReportList_DoubleClick);
            InitNameDict();
        }

        private void InitNameDict()
        {
            //生成Name与的值对，便于查找
            namdAndNodeDict.Clear();
            Stack<TreeListNode> nodeStact = new Stack<TreeListNode>();
            foreach (TreeListNode node in ReportList.Nodes)
            {
                nodeStact.Push(node);
            }
            while (nodeStact.Count != 0)
            {
                TreeListNode tempNode = nodeStact.Pop();
                foreach (TreeListNode node in tempNode.Nodes)
                {
                    nodeStact.Push(node);
                }
                namdAndNodeDict[tempNode["name"].ToString()] = tempNode;
            }
        }

        private void ReportList_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            TreeListUtilHelper.CheckMount(e.Node);
        }

        private void batchLeftMove_Click(object sender, EventArgs e)
        {
            TreeListNodes checkNodes = TreeListUtilHelper.GetCheckedNode(ReportList);
            PrintElement item = null;
            foreach (TreeListNode node in checkNodes)
            {
                item = getItemFromNode(node);
                printListData.Add(item);
            }
        }
        private PrintElement getItemFromNode(TreeListNode node)
        {
            PrintElement item = null;
            string reportSRC = node.GetValue("src") as string;
            if (File.Exists(Function.UserReportFolder + reportSRC))
            {
                reportSRC = Function.UserReportFolder + reportSRC;
            }
            else
            {
                reportSRC = Function.ReportFolder + reportSRC;
            }
            item = new PrintElement(node.GetValue("name") as string, reportSRC, false);
            item.Level = int.Parse(node.GetValue("level").ToString());
            return item;
        }

        private void ReportList_DoubleClick(object sender, EventArgs e)
        {
            if (ReportList.FocusedNode != null && ReportList.FocusedNode.Nodes.Count == 0)
                printListData.Add(getItemFromNode(ReportList.FocusedNode));
        }

        private void batchRightMove_Click(object sender, EventArgs e)
        {
            int[] selectRowHandle = printListView.GetSelectedRows();
            int[] selectDataSourceIndex = new int[selectRowHandle.Length];

            for (int i = 0; i < selectRowHandle.Length; i++)
            {
                selectDataSourceIndex[i] = printListView.GetDataSourceRowIndex(selectRowHandle[i]);

            }
            printListView.SelectRow(-1);
            for (int i = selectDataSourceIndex.Length - 1; i >= 0; i--)
            {
                printListData.RemoveAt(selectDataSourceIndex[i]);
            }
        }
        //Perperty
        public PrintList PrintListData
        {
            get { return printListData; }
        }

        private void ckCatalogue_CheckedChanged(object sender, EventArgs e)
        {
            Com.Bing.API.Function.SetProfileString(Com.Bing.API.Function.IniFile, "Report", "PrintCatalogue", ckCatalogue.Checked ? "1" : "0");
            printListData.PrintCatalogue = ckCatalogue.Checked;
        }

        private void calcTotalPage_CheckedChanged(object sender, EventArgs e)
        {
            Com.Bing.API.Function.SetProfileString(Com.Bing.API.Function.IniFile, "Report", "CalcTotalPageNum", calcTotalPage.Checked ? "1" : "0");
            printListData.CalcTotalPageNum = calcTotalPage.Checked;
        }

        private void projectCatalog_CheckedChanged(object sender, EventArgs e)
        {
            Com.Bing.API.Function.SetProfileString(Com.Bing.API.Function.IniFile, "Report", "projectCatalog", projectCatalog.Checked ? "1" : "0");
            printListData.ProjectCatalog = projectCatalog.Checked;
        }

        private void projectPDF_CheckedChanged(object sender, EventArgs e)
        {
            Com.Bing.API.Function.SetProfileString(Com.Bing.API.Function.IniFile, "Report", "projectPDF", projectPDF.Checked ? "1" : "0");
            printListData.ProjectPDF = projectPDF.Checked;
        }
        private void saveTemplate_Click(object sender, EventArgs e)
        {
            string saveFileName = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //用户选择保存文件名
            saveFileDialog.InitialDirectory = Function.MakeFolder(Path.Combine(Function.UserFolder, "打印模板"));
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveFileName = saveFileDialog.FileName;
                StreamWriter writer = new StreamWriter(File.Open(saveFileName, FileMode.Create));
                writer.AutoFlush = true;
                //将批量打印中的列表内容写入到文件中
                writer.WriteLine(printListData.ToString());
                foreach (PrintElement printElement in printListData)
                {
                    writer.WriteLine(printElement.ToString());
                }
                writer.Close();
            }
        }

        private void callTemplate_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Function.MakeFolder(Path.Combine(Function.UserFolder, "打印模板"));
            openFileDialog.DefaultExt = "txt";
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(new FileStream(openFileDialog.FileName, FileMode.Open));
                List<string> propertyList = new List<string>();
                while (!reader.EndOfStream)
                {
                    string reportName = reader.ReadLine();
                    if (reportName != "")
                    {
                        propertyList.Add(reportName);
                        //propertyList.Add(reportName);
                    }
                }
                reader.Close();

                try
                {
                    //设置批量打印的整体属性
                    printListData.SetProperty(propertyList[0]);
                    //生成报表的列表
                    if (namdAndNodeDict.Count == 0)
                    {
                        InitNameDict();
                    }

                    for (int i = 1; i < propertyList.Count; i++)
                    {
                        //ReportList
                        string[] properties = propertyList[i].Split(' ');
                        if (namdAndNodeDict.ContainsKey(properties[0]))
                        {
                            printListData.Add(getItemFromNode(namdAndNodeDict[properties[0]]));
                            printListData[printListData.Count - 1].Pagination = bool.Parse(properties[1]);
                        }
                    }
                }
                catch
                {
                    Function.Alert("选择文件的格式不对!", "提示");
                }


            }
        }

        private void startPageNum_EditValueChanged(object sender, EventArgs e)
        {
            int page = 1;
            if (int.TryParse(startPageNum.Text, out page))
            {
                printListData.StartPageNum = page;
            }
            else
            {
                Function.Alert("请输入数值!", "提示");
            }
        }

    }
    public class PrintList : BindingList<PrintElement>
    {
        //Field
        private int startPageNum = 1;
        private bool printCatalogue = false;
        private bool calcTotalPageNum = false;
        private bool projectCatalog = false;
        private bool projectPDF = false;

        //Method	
        public new void Add(PrintElement item)
        {
            if (!this.Contains(item))
                base.Add(item);
        }
        //Perperty
        public int StartPageNum
        {
            set { startPageNum = value; }
            get { return startPageNum; }
        }
        public bool PrintCatalogue
        {
            get { return printCatalogue; }
            set { printCatalogue = value; }
        }
        public bool CalcTotalPageNum
        {
            get { return calcTotalPageNum; }
            set { calcTotalPageNum = value; }
        }
        //按工程分类
        public bool ProjectCatalog
        {
            get { return projectCatalog; }
            set { projectCatalog = value; }
        }
        //PDF输出
        public bool ProjectPDF
        {
            get { return projectPDF; }
            set { projectPDF = value; }
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", startPageNum, printCatalogue, calcTotalPageNum, projectCatalog, projectPDF);
        }
        internal void SetProperty(string proeprtyList)
        {
            string[] properties = proeprtyList.Split(' ');
            if (properties.Length == 5)
            {
                startPageNum = int.Parse(properties[0]);
                printCatalogue = bool.Parse(properties[1]);
                calcTotalPageNum = bool.Parse(properties[2]);
                projectCatalog = bool.Parse(properties[3]);
                projectPDF = bool.Parse(properties[4]);
            }
        }
    }
    public class PrintElement : ICloneable
    {
        Report report = null;
        public Report Report
        {
            get { return report; }
        }
        //Field
        private string reportName = string.Empty;
        private string reportSrc = string.Empty;
        private bool pagination = false;
        private int level = 1;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        //Method
        public PrintElement()
        {

            reportName = string.Empty;
            reportSrc = string.Empty;
            pagination = false;
        }
        public PrintElement(string reportName, string reportSrc, bool pagination)
        {

            this.reportName = reportName;
            this.reportSrc = reportSrc;
            this.pagination = pagination;
        }
        //Perperty

        public string ReportName
        {
            set { reportName = value; }
            get { return reportName; }
        }
        public string ReportSrc
        {
            set { reportSrc = value; }
            get { return reportSrc; }
        }
        public bool Pagination
        {
            set { pagination = value; }
            get { return pagination; }
        }
        public override bool Equals(object obj)
        {
            PrintElement item = obj as PrintElement;
            if (item.ReportName == this.ReportName)
                return true;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return reportName + " " + pagination;
        }

        #region ICloneable 成员

        public object Clone()
        {
            return new PrintElement(this.reportName, this.reportSrc, this.pagination);
        }

        #endregion

        public void NewReport()
        {
            report = new Report(this.reportName, this.level);
            report.RptFilePath = this.reportSrc;
            string fileName = Path.GetFileName(ReportSrc);
            //用户自定义的报表不需要重定位RPT路径
            if (ReportSrc.Contains("用户自定义"))
                report.ReadRPT();
            else
                report.ReadRPT(fileName);
        }
        public void NewReport(MutliProjectManager projectManager)
        {
            report = new Report(this.reportName, this.level);
            //report.DocPath = reportSrc;
            report.PropertyValid = true;
            report.ReadRPT("report_WORD报表模板.rpt");
            //report.DllPath = Path.Combine(Function.ReportFolder, "report_项目工程宏变量列表.dll");
            report.PropertyValid = true;
            report.InitReportData(projectManager, -1);
        }
    }
}