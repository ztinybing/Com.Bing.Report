using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraTreeList.Nodes;
using System.Diagnostics;
using System.Data.Common;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using Com.Bing.API;
using Com.Bing.Business;
using System.Text.RegularExpressions;
using Com.Bing.Forms;

namespace Com.Bing.Report
{
    public partial class PrintReportControl : UserControl
    {
        public Report curReport = null;
        private MutliProjectManager projectDataManager = null;
        public Object ProjectData
        {
            get { return projectDataManager.ProjectData; }
            set
            {
                projectDataManager.ProjectData = value;
            }
        }
        private RptFileCollection rptFiles = null;
        BatchReport batchReort = null;
        object reportArg = null;
        public PrintReportControl(Object reportArg)
            : this()
        {
            this.ProjectData = reportArg;
            this.reportArg = reportArg;
        }
        public PrintReportControl()
        {
            InitializeComponent();
            projectDataManager = new MutliProjectManager();
            projectDataManager.CheckProjectNode += this.CheckProjectNode;
            projectDataManager.InitMutliProject += this.InitMutliProjectStruct;
            projectDataManager.ReCheckProjectNode += this.ReCheckProjectNode;
            rptFiles = new RptFileCollection(EchoNameReplace);
            rptFiles.InitFromXml();
            InitReportList();
            batchReort = new BatchReport(rptFiles.RptFileTable, projectDataManager, this);
            Function.UpperFlag = "true".Equals(Function.ProfileString("环境设置", "上标设置", "false"), StringComparison.OrdinalIgnoreCase);            
        }

        //刷新当前报表节点
        public void ReferenceFocusedNode()
        {
            if (ReportList.FocusedNode == null)
            {
                return;
            }
            //InitMutliProjectStruct();
            string fileName = ReportList.FocusedNode.GetValue("src").ToString();
            if (!string.IsNullOrEmpty(fileName))
            {
                string descrName = ReportList.FocusedNode.GetValue("name").ToString();
                int descrLevel = 0;
                if (!int.TryParse(ReportList.FocusedNode.GetValue("level").ToString(), out descrLevel))
                {
                    Function.Alert("报表相关XML的LEVEL数据无效!", "提示");
                }
                curReport = new Report(descrName, descrLevel);

                curReport.ReadRPT(fileName);

                //用户报表
                string userFilePath = curReport.RptFilePath;
                if (userFilePath.StartsWith(Function.UserReportFolder))
                {
                    string sysFilePath = Function.ReportFolder + fileName;
                    if (File.GetLastWriteTime(sysFilePath) > File.GetLastWriteTime(userFilePath))
                    {
                        //系统报表新于用户报表，更新用户报表，保留原有列显示信息和参数设置
                        Report newReport = new Report(descrName, descrLevel);
                        newReport.RptFilePath = sysFilePath;
                        newReport.ReadRPT();
                        foreach (ReportColumn column in newReport.Columns)
                        {
                            ReportColumn col = curReport.Columns.Find(column.ColumnName);
                            if (col != null)
                            {
                                column.Attibutes.PrintStyle.NoPrint = col.Attibutes.PrintStyle.NoPrint;
                            }
                        }
                        curReport = newReport;
                        curReport.RptFilePath = userFilePath;
                        curReport.WriteRpt(false);
                    }
                }

                arguments.Enabled = (Function.ProfileInt(curReport.RptFilePath, "报表参数", "enabled", 0) == 1
                    && Function.ProfileInt(curReport.RptFilePath, "报表参数", "count", 0) != 0);
                //初始化预览方式数据
                //curReport.InitReportData(projectDataManager, 0);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["生成方式"] = "预览";
                curReport.InitReportData(projectDataManager.ProjectData as DataSet, dict);                
                Repaint();
            }
            else
            {
                curReport = null;
                ReportMode.Image = null;
            }
        }
        private void InitReportList()
        {
            ReportList.DataSource = rptFiles.RptFileTable.DefaultView;

            ReportList.Columns["src"].Visible = false;
            ReportList.Columns["level"].Visible = false;
            ReportList.Columns["name"].Caption = "报表列表";
            //将报表节点全部展开
            ReportList.ForceInitialize();
            expandTreeList(ReportList.Nodes);
            //ReportList.ExpandAll();
        }
        private void expandTreeList(TreeListNodes nodes)
        {
            foreach (TreeListNode node in nodes)
            {
                if (node.Level == 1)
                {
                    if (node.Nodes.Count > 0)
                        node.ParentNode.Expanded = true;
                }
                else
                {
                    expandTreeList(node.Nodes);
                }
            }
        }

        public void SetReportGroup(string[] rptGroup)
        {
            rptFiles.SetGroup(rptGroup);
            InitReportList();
        }
        public object ReportArg
        {
            get { return reportArg; }
        }

        /// <summary>
        /// 用于生成多工程的浏览、打印、发送的结构
        /// </summary>
        /// <example>
        /// XM 
        /// 
        ///   Dx1 
        ///      Dw1  checked
        ///      Dw2  
        ///   Dx2 
        ///      Dw3 checked
        ///      DW4 checked
        ///     生成的结构为    
        ///    1 (L(Dw1) ,dw1name) (L(Dw3),dw3name3) (L(Dw4),dw4name4)
        ///    2 (L(Dw1),dx1name ) (L(DW3,Dw4) , dx2name)
        ///    3 (L(Dw1,Dw3,Dw4),xm1name)
        /// </example>
        protected virtual Dictionary<int, List<KeyValuePair<List<DataRow>, string>>> InitMutliProjectStruct()
        {
            Dictionary<int, List<KeyValuePair<List<DataRow>, string>>> mutliProjectStruct = new Dictionary<int, List<KeyValuePair<List<DataRow>, string>>>();

            return mutliProjectStruct;
        }
        protected virtual void CheckProjectNode(List<KeyValuePair<List<DataRow>, string>> mutliProjectList, int projectIndex)
        {

        }
        protected virtual void ReCheckProjectNode(List<KeyValuePair<List<DataRow>, string>> mutliProjectList)
        {


        }
        public virtual string EchoNameReplace(string xmlName)
        {
            return xmlName;
        }
        #region 初始化控件的内容

        #endregion
        #region 控件的事件
        public void preivew_Click(object sender, EventArgs e)
        {
            if (curReport != null)
            {
                if (curReport.IsDoc)
                {
                    //打印WORD格式报表
                    ReportHelper.OpenWord(curReport.DocPath);
                }
                else
                {
                    curReport.InitReportData(projectDataManager, 0);
                    PreviewReport viewDialog = new PreviewReport(curReport);                    
                    viewDialog.Show();
                    Repaint();
                }
            }
        }
        private void ReportList_DoubleClick(object sender, EventArgs e)
        {
            if (curReport == null)
            {
                return;
            }
            if (Control.ModifierKeys == Keys.Shift)
            {
                //shift+双击 打开资源管理器，选中报表文件                
                System.Diagnostics.Process.Start("Explorer.exe", @"/select," + curReport.RptFilePath);
            }
            else
            {
                preview.PerformClick();
            }
        }
        public void print_Click(object sender, EventArgs e)
        {
            if (curReport != null)
            {
                if (curReport.IsDoc)
                {
                    //打印WORD格式报表
                    ReportHelper.PrintWord(curReport.DocPath, curReport.Data.MacorsVerbCollection);
                }
                else
                {
                    PrintUtil.ShowPrintDlg();
                    curReport.InitReportData(this.projectDataManager, 0);
                    curReport.MutliProjectPrint();
                }
            }
        }
        /// <summary>
        /// 批量打印按钮事件 , 先弹出批量打印框架，再弹出打印框
        /// </summary>        
        public void printMount_Click(object sender, EventArgs e)
        {
            PanelLock(true);
            if (!batchReort.Print())
            {
                PanelLock(false);
            }
        }
        public void SetReportSector(object sender, EventArgs e)
        {
            string[] section = new string[] { "环境设置" };
            CommonConfigDialog dlg = new CommonConfigDialog(section);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Function.UpperFlag = "true".Equals(Function.ProfileString("环境设置", "上标设置", "false"), StringComparison.OrdinalIgnoreCase);
            }
        }
        #endregion
        private void PrintReportControl_SizeChanged(object sender, EventArgs e)
        {
            this.ReportList.Size = new Size(this.Size.Width, this.Size.Height);
        }
        private void ReportList_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            ReferenceFocusedNode();
        }

        private void Repaint()
        {
            ReportMode.Image = curReport.SpeedPreview(1);
            if (ReportMode.Image != null)
            {
                ReportMode.Cursor = Cursors.Arrow;
                ReportMode.Dock = DockStyle.None;
                ReportMode.Size = ReportMode.Image.Size;
                ReportMode.SizeMode = PictureBoxSizeMode.StretchImage;
                resizePicture();

            }
        }
        private void resizePicture()
        {
            //依据panel2的大小进行缩放图片，并使图片居中(长宽必须按照比例进行缩放)
            //调整大小
            double scale = 1;
            scale = (tabPage1.Width - 40) / (double)ReportMode.Width;
            if (scale > (tabPage1.Height - 40) / (double)ReportMode.Height)
                scale = (tabPage1.Height - 40) / (double)ReportMode.Height;
            ReportMode.Size = new Size((int)(ReportMode.Width * scale),
                (int)(ReportMode.Height * scale));
            ReportMode.Location = new Point(
                (tabPage1.Width - ReportMode.Width) / 2,
                (tabPage1.Height - ReportMode.Height) / 2);
        }
        private void tabPage1_Resize(object sender, EventArgs e)
        {
            reportstyle.Refresh();
            resizePicture();
        }
        private void PanelLock(bool isLock)
        {
            if (isLock)
            {
                this.preview.Enabled = false;
                this.print.Enabled = false;
                this.printAll.Enabled = false;
                this.sendtoexcel.Enabled = false;
                this.sendToExcelMount.Enabled = false;
                this.StopOpt.Enabled = true;
                panelControl1.Visible = true;
            }
            else
            {
                this.preview.Enabled = true;
                this.print.Enabled = true;
                this.printAll.Enabled = true;
                this.sendtoexcel.Enabled = true;
                this.sendToExcelMount.Enabled = true;
                this.StopOpt.Enabled = false;
                panelControl1.Visible = false;
            }
        }
        public void sendtoexcel_Click(object sender, EventArgs e)
        {
            string xlsFile = string.Empty;
            //WORD格式的报表不允许发送EXCEL
            if (curReport != null && !curReport.IsDoc && XLSFile.ShowSaveFile(ref xlsFile, curReport.DesrcName))
            {
                curReport.InitReportData(projectDataManager, 0);
                curReport.MutliProjectSendToExcel(xlsFile);
                Function.Alert("发送成功！", "提示");
            }
        }
        public void sendToExcelMount_Click(object sender, EventArgs e)
        {
            PanelLock(true);
            if (!batchReort.SendExcel())
            {
                PanelLock(false);
            }

        }
        private void ReportList_NodesReloaded(object sender, EventArgs e)
        {
            ReportList.ExpandAll();
        }
        public void rpChange_Click(object sender, EventArgs e)
        {
            if (curReport != null)
            {
                curReport.ChangeProperty();
                Repaint();
            }
        }
        private void rpChangeMount_Click(object sender, EventArgs e)
        {

            ModifyReportProperty dlg = new ModifyReportProperty(this.rptFiles);
            dlg.ShowDialog();
        }
        private void ReportMode_DoubleClick(object sender, EventArgs e)
        {
            preview.PerformClick();
        }

        private void arguments_Click(object sender, EventArgs e)
        {
            if (curReport != null)
            {
                curReport.InitReportData(projectDataManager, 0);
                curReport.ChangeArgs();
                Repaint();
            }
        }

        private void ReportList_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            if (e.Node.Focused)
            {
                e.Appearance.BackColor = Color.Blue;
                e.Appearance.ForeColor = Color.White;
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (curReport != null)
            {
                curReport.Design();
                Repaint();
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (curReport != null)
            {
                curReport.CoverUsrRpt();
                Repaint();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (curReport != null)
            {
                curReport.CopyRpt();
                reLoadTreeList();
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (curReport != null)
            {
                curReport.DeleteRpt();
                reLoadTreeList();
            }
        }
        //重新加载报表树结构
        private void reLoadTreeList()
        {
            this.rptFiles.InitFromXml();
            InitReportList();
        }
        private static string CopyRootSource = string.Empty;
        public void CopyRoot_Click(object sender, EventArgs e)
        {

        }
        public void PasteRoot_Click(object sender, EventArgs e)
        {

        }
        public void RootSyn(object sender, EventArgs e)
        {

        }
        public void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressText.Text = PrintProgress.WorkContent;
            progressBarControl.Position = PrintProgress.ProgressPercentage;
        }
        public void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                progressText.Text = "工作被取消";
                progressBarControl.Position = 100;
                return;
            }
            else
            {
                Function.Alert("批量操作结束", "提示");
            }
            PanelLock(false);
        }

        private void StopOpt_Click(object sender, EventArgs e)
        {
            batchReort.CancelAsync();
        }

        private bool bStop = false;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                SearchNode dlg = new SearchNode(rptFiles.RptFileTable);
                dlg.Show();
                dlg.SearchNodeByName += new EventHandler<NodeEventArgs>(SearchNodeByName);
            }
            if (keyData == (Keys.Control | Keys.B))
            {
                if (Function.Confirm("是否进行批量点击测试？", "提示"))
                {
                    ReportList.ExpandAll();
                    Timer t = new Timer();
                    t.Interval = 1000;
                    t.Tick += new EventHandler(t_Tick);
                    t.Start();
                }
            }
            if (keyData == Keys.Escape)
            {
                bStop = true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        void t_Tick(object sender, EventArgs e)
        {
            if (bStop)
            {
                (sender as Timer).Stop();
                bStop = false;
            }
            else
            {
                SendKeys.SendWait("{DOWN}");
                if (ReportList.FocusedNode == ReportList.Nodes.LastNode)
                {
                    (sender as Timer).Stop();
                }
            }

        }
        void SearchNodeByName(object sender, NodeEventArgs e)
        {
            TreeListNode node = ReportList.FindNodeByFieldValue("ID", e.ID);
            if (node != null)
            {
                if (node.ParentNode != null)
                {
                    node.ParentNode.ExpandAll();
                }
                ReportList.SetFocusedNode(node);
            }
        }


    }
    public delegate string EchoNameReplace(string name);
    public class RptFileCollection
    {
        private string[] rptGroup = new string[] { "" };
        DataTable rptFileTable = new DataTable();
        public DataTable RptFileTable
        {
            get { return rptFileTable; }
        }
        EchoNameReplace EchoNameReplace = null;
        public RptFileCollection(EchoNameReplace echoNameReplace)
        {
            this.EchoNameReplace = echoNameReplace;
            initTableStruct(rptFileTable);
        }
        //初始化报表列表数据
        private void initTableStruct(DataTable rptTable)
        {
            DataColumn dataColumn = null;
            dataColumn = new DataColumn("ID", Type.GetType("System.Int32"));
            rptTable.Columns.Add(dataColumn);
            dataColumn = new DataColumn("ParentID", Type.GetType("System.Int32"));
            rptTable.Columns.Add(dataColumn);
            dataColumn = new DataColumn("name");
            rptTable.Columns.Add(dataColumn);
            dataColumn = new DataColumn("src");
            rptTable.Columns.Add(dataColumn);
            //报表分级
            dataColumn = new DataColumn("level");
            dataColumn.DefaultValue = 1;
            rptTable.Columns.Add(dataColumn);
        }
        private void xmlNodeToTreeNode(System.Data.DataTable table, XmlElement rootElement, TreeNode rootNode)
        {
            foreach (XmlElement xle in rootElement.ChildNodes)
            {
                int level = 1;
                TreeNode node = new TreeNode();
                XmlAttribute srcAttribute = xle.Attributes["src"];
                if (srcAttribute == null) continue;
                node.Name = xle.Attributes["src"].Value;
                node.Text = EchoNameReplace(xle.Attributes["name"].Value);
                if (xle.Attributes["level"] != null)
                {
                    level = int.Parse(xle.Attributes["level"].Value);
                }
                node.Tag = node.GetHashCode();
                table.Rows.Add(new object[] { node.Tag, rootNode.Tag, node.Text, node.Name, level });
                if (xle.HasChildNodes)
                    xmlNodeToTreeNode(table, xle, node);
            }
        }


        public void InitFromXml()
        {
            rptFileTable.Clear();
            ReportHelper.GenerateXml(Function.MakeFolder(Function.UserReportFolder + "用户自定义"), Function.ReportFolder + "ReportSet用户自定义.xml");
            foreach (string rpt in rptGroup)
            {
                string rptXml = Function.ReportFolder + "ReportSet" + rpt + ".xml";
                if (!File.Exists(rptXml))
                {
                    continue;
                }
                //读取配制文件中的报表(xml)
                ParseXml(rptXml);
            }
        }
        private void ParseXml(string rptXml)
        {
            XmlDocumentEx xld = new XmlDocumentEx();
            xld.Load(rptXml);
            XmlElement rootElement = xld.DocumentElement;
            TreeNode rootNode = new TreeNode();
            rootNode.Name = rootElement.Attributes["src"].Value;
            rootNode.Text = EchoNameReplace(rootElement.Attributes["name"].Value);
            rootNode.Tag = rootNode.GetHashCode();
            rptFileTable.Rows.Add(new object[] { rootNode.Tag, -1, rootNode.Text, rootNode.Name });
            xmlNodeToTreeNode(rptFileTable, rootElement, rootNode);
        }

        public void SetGroup(string[] group)
        {
            this.rptGroup = new string[group.Length + 1];
            group.CopyTo(this.rptGroup, 0);
            this.rptGroup[group.Length] = "用户自定义";

            InitFromXml();
        }

    }
}
