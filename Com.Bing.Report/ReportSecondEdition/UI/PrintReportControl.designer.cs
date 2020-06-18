namespace Com.Bing.Report
{
    partial class PrintReportControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            //ReportCenter.CheckProjectNode = null;
            //ReportCenter.ReCheckProjectNode = null;
            if (disposing && (components != null))
            {
                components.Dispose();
				backgroundWorker.CancelAsync();
            }			
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintReportControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.rpChange = new System.Windows.Forms.ToolStripButton();
            this.rpChangeMount = new System.Windows.Forms.ToolStripButton();
            this.arguments = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.preview = new System.Windows.Forms.ToolStripButton();
            this.print = new System.Windows.Forms.ToolStripButton();
            this.printAll = new System.Windows.Forms.ToolStripButton();
            this.sendtoexcel = new System.Windows.Forms.ToolStripButton();
            this.sendToExcelMount = new System.Windows.Forms.ToolStripButton();
            this.StopOpt = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRestore = new System.Windows.Forms.ToolStripButton();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnSet = new System.Windows.Forms.ToolStripButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ReportList = new Com.Bing.UI.BaseTreeList();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.progressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
            this.progressText = new DevExpress.XtraEditors.LabelControl();
            this.reportstyle = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ReportMode = new System.Windows.Forms.PictureBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReportList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl.Properties)).BeginInit();
            this.reportstyle.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReportMode)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(908, 591);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.TabIndex = 6;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rpChange,
            this.rpChangeMount,
            this.arguments,
            this.toolStripSeparator1,
            this.preview,
            this.print,
            this.printAll,
            this.sendtoexcel,
            this.sendToExcelMount,
            this.StopOpt,
            this.toolStripSeparator2,
            this.btnRestore,
            this.btnCopy,
            this.btnDelete,
            this.btnSet});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(908, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // rpChange
            // 
            this.rpChange.Image = ((System.Drawing.Image)(resources.GetObject("rpChange.Image")));
            this.rpChange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rpChange.Name = "rpChange";
            this.rpChange.Size = new System.Drawing.Size(49, 22);
            this.rpChange.Text = "调整";
            this.rpChange.Click += new System.EventHandler(this.rpChange_Click);
            // 
            // rpChangeMount
            // 
            this.rpChangeMount.Image = ((System.Drawing.Image)(resources.GetObject("rpChangeMount.Image")));
            this.rpChangeMount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rpChangeMount.Name = "rpChangeMount";
            this.rpChangeMount.Size = new System.Drawing.Size(73, 22);
            this.rpChangeMount.Text = "批量调整";
            this.rpChangeMount.Click += new System.EventHandler(this.rpChangeMount_Click);
            // 
            // arguments
            // 
            this.arguments.Image = ((System.Drawing.Image)(resources.GetObject("arguments.Image")));
            this.arguments.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.arguments.Name = "arguments";
            this.arguments.Size = new System.Drawing.Size(49, 22);
            this.arguments.Text = "参数";
            this.arguments.Click += new System.EventHandler(this.arguments_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // preview
            // 
            this.preview.Image = ((System.Drawing.Image)(resources.GetObject("preview.Image")));
            this.preview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(49, 22);
            this.preview.Text = "预览";
            this.preview.Click += new System.EventHandler(this.preivew_Click);
            // 
            // print
            // 
            this.print.Image = ((System.Drawing.Image)(resources.GetObject("print.Image")));
            this.print.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.print.Name = "print";
            this.print.Size = new System.Drawing.Size(49, 22);
            this.print.Text = "打印";
            this.print.Click += new System.EventHandler(this.print_Click);
            // 
            // printAll
            // 
            this.printAll.Image = ((System.Drawing.Image)(resources.GetObject("printAll.Image")));
            this.printAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printAll.Name = "printAll";
            this.printAll.Size = new System.Drawing.Size(73, 22);
            this.printAll.Text = "批量打印";
            this.printAll.Click += new System.EventHandler(this.printMount_Click);
            // 
            // sendtoexcel
            // 
            this.sendtoexcel.Image = ((System.Drawing.Image)(resources.GetObject("sendtoexcel.Image")));
            this.sendtoexcel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sendtoexcel.Name = "sendtoexcel";
            this.sendtoexcel.Size = new System.Drawing.Size(49, 22);
            this.sendtoexcel.Text = "发送";
            this.sendtoexcel.Click += new System.EventHandler(this.sendtoexcel_Click);
            // 
            // sendToExcelMount
            // 
            this.sendToExcelMount.Image = ((System.Drawing.Image)(resources.GetObject("sendToExcelMount.Image")));
            this.sendToExcelMount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sendToExcelMount.Name = "sendToExcelMount";
            this.sendToExcelMount.Size = new System.Drawing.Size(73, 22);
            this.sendToExcelMount.Text = "批量发送";
            this.sendToExcelMount.Click += new System.EventHandler(this.sendToExcelMount_Click);
            // 
            // StopOpt
            // 
            this.StopOpt.Image = ((System.Drawing.Image)(resources.GetObject("StopOpt.Image")));
            this.StopOpt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StopOpt.Name = "StopOpt";
            this.StopOpt.Size = new System.Drawing.Size(49, 22);
            this.StopOpt.Text = "中止";
            this.StopOpt.Click += new System.EventHandler(this.StopOpt_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRestore
            // 
            this.btnRestore.Image = ((System.Drawing.Image)(resources.GetObject("btnRestore.Image")));
            this.btnRestore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(49, 22);
            this.btnRestore.Text = "还原";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(49, 22);
            this.btnCopy.Text = "复制";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(49, 22);
            this.btnDelete.Text = "删除";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSet
            // 
            this.btnSet.Image = ((System.Drawing.Image)(resources.GetObject("btnSet.Image")));
            this.btnSet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(49, 22);
            this.btnSet.Text = "设计";
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
            this.splitContainer2.Panel1.Controls.Add(this.ReportList);
            this.splitContainer2.Panel1.Controls.Add(this.panelControl1);
            this.splitContainer2.Panel1.Padding = new System.Windows.Forms.Padding(1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
            this.splitContainer2.Panel2.Controls.Add(this.reportstyle);
            this.splitContainer2.Panel2.Padding = new System.Windows.Forms.Padding(1);
            this.splitContainer2.Size = new System.Drawing.Size(908, 562);
            this.splitContainer2.SplitterDistance = 301;
            this.splitContainer2.TabIndex = 0;
            // 
            // ReportList
            // 
            this.ReportList.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.ReportList.AllowDrop = true;
            this.ReportList.Appearance.FocusedCell.BackColor = System.Drawing.Color.Blue;
            this.ReportList.Appearance.FocusedCell.Options.UseBackColor = true;
            this.ReportList.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ReportList.DefaultFocusColumn = 0;
            this.ReportList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportList.FocusedBackColor = System.Drawing.Color.White;
            this.ReportList.Location = new System.Drawing.Point(1, 1);
            this.ReportList.Name = "ReportList";
            this.ReportList.OptionsBehavior.Editable = false;
            this.ReportList.OptionsBehavior.ImmediateEditor = false;
            this.ReportList.OptionsView.ShowIndicator = false;
            this.ReportList.SelectBackColor = System.Drawing.Color.LightSteelBlue;
            this.ReportList.SelectMode = Com.Bing.UI.TreeListX.SelectModeEnum.Default;
            this.ReportList.Size = new System.Drawing.Size(299, 510);
            this.ReportList.TabIndex = 5;
            this.ReportList.ToolTip = "";
            this.ReportList.WarningCount = 800;
            this.ReportList.NodesReloaded += new System.EventHandler(this.ReportList_NodesReloaded);
            this.ReportList.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.ReportList_CustomDrawNodeCell);
            this.ReportList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.ReportList_FocusedNodeChanged);
            this.ReportList.DoubleClick += new System.EventHandler(this.ReportList_DoubleClick);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.progressBarControl);
            this.panelControl1.Controls.Add(this.progressText);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(1, 511);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(299, 50);
            this.panelControl1.TabIndex = 9;
            this.panelControl1.Visible = false;
            // 
            // progressBarControl
            // 
            this.progressBarControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBarControl.Location = new System.Drawing.Point(2, 30);
            this.progressBarControl.Name = "progressBarControl";
            this.progressBarControl.Size = new System.Drawing.Size(295, 18);
            this.progressBarControl.TabIndex = 9;
            // 
            // progressText
            // 
            this.progressText.Location = new System.Drawing.Point(6, 10);
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(96, 14);
            this.progressText.TabIndex = 8;
            this.progressText.Text = "批量操作进度提示";
            // 
            // reportstyle
            // 
            this.reportstyle.Controls.Add(this.tabPage1);
            this.reportstyle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportstyle.Location = new System.Drawing.Point(1, 1);
            this.reportstyle.Name = "reportstyle";
            this.reportstyle.SelectedIndex = 0;
            this.reportstyle.Size = new System.Drawing.Size(601, 560);
            this.reportstyle.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.reportstyle.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.DarkGray;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPage1.Controls.Add(this.ReportMode);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(593, 535);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "报表样式";
            this.tabPage1.Resize += new System.EventHandler(this.tabPage1_Resize);
            // 
            // ReportMode
            // 
            this.ReportMode.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ReportMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportMode.Location = new System.Drawing.Point(3, 3);
            this.ReportMode.Name = "ReportMode";
            this.ReportMode.Size = new System.Drawing.Size(583, 525);
            this.ReportMode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ReportMode.TabIndex = 6;
            this.ReportMode.TabStop = false;
            this.ReportMode.Tag = "";
            this.ReportMode.WaitOnLoad = true;
            this.ReportMode.DoubleClick += new System.EventHandler(this.ReportMode_DoubleClick);
            // 
            // PrintReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PrintReportControl";
            this.Size = new System.Drawing.Size(908, 591);
            this.SizeChanged += new System.EventHandler(this.PrintReportControl_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ReportList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl.Properties)).EndInit();
            this.reportstyle.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ReportMode)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
        private Com.Bing.UI.BaseTreeList ReportList;
		private System.Windows.Forms.TabControl reportstyle;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.PictureBox ReportMode;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		
		private DevExpress.XtraEditors.LabelControl progressText;
        private DevExpress.XtraEditors.ProgressBarControl progressBarControl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton rpChange;
        private System.Windows.Forms.ToolStripButton arguments;
        private System.Windows.Forms.ToolStripButton btnSet;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton preview;
        private System.Windows.Forms.ToolStripButton print;
        private System.Windows.Forms.ToolStripButton printAll;
        private System.Windows.Forms.ToolStripButton sendtoexcel;
        private System.Windows.Forms.ToolStripButton sendToExcelMount;
        private System.Windows.Forms.ToolStripButton StopOpt;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnRestore;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton rpChangeMount;



	}
}
