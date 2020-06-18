namespace Com.Bing.Report
{
    partial class PreviewReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewReport));
            System.ComponentModel.ComponentResourceManager exResources = new System.ComponentModel.ComponentResourceManager(typeof(PrintReportControl));
            

            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.preProject = new System.Windows.Forms.ToolStripButton();
            this.nextProject = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.firstPage = new System.Windows.Forms.ToolStripButton();
            this.prePage = new System.Windows.Forms.ToolStripButton();
            this.nextPage = new System.Windows.Forms.ToolStripButton();
            this.lastPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.previewPageData = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.printPage = new System.Windows.Forms.ToolStripButton();
            this.sendToXls = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.propertyChang = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolClose = new System.Windows.Forms.ToolStripButton();
            this.designReport = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 367);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(726, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusLabel1.Text = "第 1 页";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(11, 17);
            this.toolStripStatusLabel3.Text = "|";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(37, 17);
            this.toolStripStatusLabel2.Text = "２/２";
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(726, 314);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 28);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(726, 339);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Controls.Add(this.pictureBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(726, 314);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(726, 314);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            this.pictureBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preProject,
            this.nextProject,
            this.toolStripSeparator2,
            this.firstPage,
            this.prePage,
            this.nextPage,
            this.lastPage,
            this.toolStripSeparator1,
            this.previewPageData,
            this.toolStripSeparator3,
            this.printPage,
            this.sendToXls,
            this.toolStripSeparator4,
            this.propertyChang,
            this.designReport,
            this.toolStripSeparator5,
            this.toolClose});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(726, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.SizeChanged += new System.EventHandler(this.toolStrip1_SizeChanged);
            // 
            // preProject
            // 
            this.preProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.preProject.Image = ((System.Drawing.Image)(resources.GetObject("preProject.Image")));
            this.preProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.preProject.Name = "preProject";
            this.preProject.Size = new System.Drawing.Size(23, 22);
            this.preProject.Text = "上一工程";
            this.preProject.Click += new System.EventHandler(this.preProject_Click);
            // 
            // nextProject
            // 
            this.nextProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextProject.Image = ((System.Drawing.Image)(resources.GetObject("nextProject.Image")));
            this.nextProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextProject.Name = "nextProject";
            this.nextProject.Size = new System.Drawing.Size(23, 22);
            this.nextProject.Text = "下一工程";
            this.nextProject.Click += new System.EventHandler(this.nextProject_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // firstPage
            // 
            this.firstPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.firstPage.Image = ((System.Drawing.Image)(resources.GetObject("firstPage.Image")));
            this.firstPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.firstPage.Name = "firstPage";
            this.firstPage.Size = new System.Drawing.Size(23, 22);
            this.firstPage.Text = "firstPage";
            this.firstPage.ToolTipText = "首页";
            this.firstPage.Click += new System.EventHandler(this.firstPage_Click);
            // 
            // prePage
            // 
            this.prePage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prePage.Image = ((System.Drawing.Image)(resources.GetObject("prePage.Image")));
            this.prePage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prePage.Name = "prePage";
            this.prePage.Size = new System.Drawing.Size(23, 22);
            this.prePage.Text = "prePage";
            this.prePage.ToolTipText = "上一页";
            this.prePage.Click += new System.EventHandler(this.prePage_Click);
            // 
            // nextPage
            // 
            this.nextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextPage.Image = ((System.Drawing.Image)(resources.GetObject("nextPage.Image")));
            this.nextPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextPage.Name = "nextPage";
            this.nextPage.Size = new System.Drawing.Size(23, 22);
            this.nextPage.Text = "nextPage";
            this.nextPage.ToolTipText = "下一页";
            this.nextPage.Click += new System.EventHandler(this.nextPage_Click);
            // 
            // lastPage
            // 
            this.lastPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lastPage.Image = ((System.Drawing.Image)(resources.GetObject("lastPage.Image")));
            this.lastPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lastPage.Name = "lastPage";
            this.lastPage.Size = new System.Drawing.Size(23, 22);
            this.lastPage.Text = "lastPage";
            this.lastPage.ToolTipText = "尾页";
            this.lastPage.Click += new System.EventHandler(this.lastPage_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // previewPageData
            // 
            this.previewPageData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previewPageData.Image = ((System.Drawing.Image)(resources.GetObject("previewPageData.Image")));
            this.previewPageData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previewPageData.Name = "previewPageData";
            this.previewPageData.Size = new System.Drawing.Size(23, 22);
            this.previewPageData.Text = "报表数据编辑";
            this.previewPageData.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // printPage
            // 
            this.printPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printPage.Image = ((System.Drawing.Image)(resources.GetObject("printPage.Image")));
            this.printPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printPage.Name = "printPage";
            this.printPage.Size = new System.Drawing.Size(23, 22);
            this.printPage.Text = "printPage";
            this.printPage.ToolTipText = "打印";
            this.printPage.Click += new System.EventHandler(this.printPage_Click);
            // 
            // sendToXls
            // 
            this.sendToXls.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sendToXls.Image = ((System.Drawing.Image)(resources.GetObject("sendToXls.Image")));
            this.sendToXls.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sendToXls.Name = "sendToXls";
            this.sendToXls.Size = new System.Drawing.Size(23, 22);
            this.sendToXls.Text = "发送到Excel";
            this.sendToXls.Visible = false;
            this.sendToXls.Click += new System.EventHandler(this.sendToXls_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // propertyChang
            // 
            this.propertyChang.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.propertyChang.Image = ((System.Drawing.Image)(resources.GetObject("propertyChang.Image")));
            this.propertyChang.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.propertyChang.Name = "propertyChang";
            this.propertyChang.Size = new System.Drawing.Size(23, 22);
            this.propertyChang.Text = "调整";
            this.propertyChang.Click += new System.EventHandler(this.propertyChang_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolClose
            // 
            this.toolClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolClose.Name = "toolClose";
            this.toolClose.Size = new System.Drawing.Size(52, 22);
            this.toolClose.Text = "关闭(&C)";
            this.toolClose.ToolTipText = "关闭预览";
            this.toolClose.Click += new System.EventHandler(this.toolClose_Click);
            // 
            // designReport
            // 
            this.designReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.designReport.Image = ((System.Drawing.Image)(exResources.GetObject("btnSet.Image")));
            this.designReport.Click += new System.EventHandler(designReport_Click);
            this.designReport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.designReport.Name = "designReport";
            this.designReport.Size = new System.Drawing.Size(23, 22);
            this.designReport.Text = "设计报表";
            // 
            // PreviewReport
            // 
            this.Appearance.Font = new System.Drawing.Font("宋体", 9F);
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(726, 389);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.Name = "PreviewReport";
            this.Text = "报表预览";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreviewReport_FormClosed);
            this.Shown += new System.EventHandler(this.PreviewReport_Shown);
            this.Controls.SetChildIndex(this.statusStrip1, 0);
            this.Controls.SetChildIndex(this.toolStripContainer1, 0);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton firstPage;
        private System.Windows.Forms.ToolStripButton prePage;
        private System.Windows.Forms.ToolStripButton nextPage;
        private System.Windows.Forms.ToolStripButton lastPage;
        private System.Windows.Forms.ToolStripButton printPage;
        private System.Windows.Forms.ToolStripButton toolClose;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripButton sendToXls;
        private System.Windows.Forms.ToolStripButton preProject;
        private System.Windows.Forms.ToolStripButton nextProject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton previewPageData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton propertyChang;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton designReport;
		

    }
}