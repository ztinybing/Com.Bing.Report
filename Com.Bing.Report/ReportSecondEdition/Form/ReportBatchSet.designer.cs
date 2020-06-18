namespace Com.Bing.Report
{
    partial class ReportBatchSet
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
            this.ReportList = new Com.Bing.UI.BaseTreeList();
            this.batchRightMove = new DevExpress.XtraEditors.SimpleButton();
            this.batchLeftMove = new DevExpress.XtraEditors.SimpleButton();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.printList = new DevExpress.XtraGrid.GridControl();
            this.printListView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.projectPDF = new DevExpress.XtraEditors.CheckEdit();
            this.callTemplate = new DevExpress.XtraEditors.SimpleButton();
            this.saveTemplate = new DevExpress.XtraEditors.SimpleButton();
            this.projectCatalog = new DevExpress.XtraEditors.CheckEdit();
            this.calcTotalPage = new DevExpress.XtraEditors.CheckEdit();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.ckCatalogue = new DevExpress.XtraEditors.CheckEdit();
            this.startPageNum = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.ReportList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.printList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.printListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectPDF.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectCatalog.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.calcTotalPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckCatalogue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startPageNum.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ReportList
            // 
            this.ReportList.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.ReportList.AllowDrop = true;
            this.ReportList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.ReportList.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ReportList.Dock = System.Windows.Forms.DockStyle.Left;
            this.ReportList.Location = new System.Drawing.Point(0, 0);
            this.ReportList.Name = "ReportList";
            this.ReportList.OptionsBehavior.Editable = false;
            this.ReportList.OptionsView.ShowCheckBoxes = true;
            this.ReportList.OptionsView.ShowIndicator = false;
            this.ReportList.Size = new System.Drawing.Size(361, 518);
            this.ReportList.TabIndex = 10;
            // 
            // batchRightMove
            // 
            this.batchRightMove.Location = new System.Drawing.Point(363, 274);
            this.batchRightMove.Name = "batchRightMove";
            this.batchRightMove.Size = new System.Drawing.Size(42, 23);
            this.batchRightMove.TabIndex = 9;
            this.batchRightMove.Text = "<<";
            // 
            // batchLeftMove
            // 
            this.batchLeftMove.Location = new System.Drawing.Point(363, 182);
            this.batchLeftMove.Name = "batchLeftMove";
            this.batchLeftMove.Size = new System.Drawing.Size(42, 23);
            this.batchLeftMove.TabIndex = 8;
            this.batchLeftMove.Text = ">>";
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(408, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.printList);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.projectPDF);
            this.splitContainerControl1.Panel2.Controls.Add(this.callTemplate);
            this.splitContainerControl1.Panel2.Controls.Add(this.saveTemplate);
            this.splitContainerControl1.Panel2.Controls.Add(this.projectCatalog);
            this.splitContainerControl1.Panel2.Controls.Add(this.calcTotalPage);
            this.splitContainerControl1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainerControl1.Panel2.Controls.Add(this.btnOk);
            this.splitContainerControl1.Panel2.Controls.Add(this.ckCatalogue);
            this.splitContainerControl1.Panel2.Controls.Add(this.startPageNum);
            this.splitContainerControl1.Panel2.Controls.Add(this.labelControl1);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(424, 518);
            this.splitContainerControl1.SplitterPosition = 431;
            this.splitContainerControl1.TabIndex = 11;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // printList
            // 
            this.printList.AllowDrop = true;
            this.printList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printList.EmbeddedNavigator.Name = "";
            this.printList.Location = new System.Drawing.Point(0, 0);
            this.printList.MainView = this.printListView;
            this.printList.Name = "printList";
            this.printList.Size = new System.Drawing.Size(420, 427);
            this.printList.TabIndex = 8;
            this.printList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.printListView});
            // 
            // printListView
            // 
            this.printListView.GridControl = this.printList;
            this.printListView.Name = "printListView";
            this.printListView.OptionsMenu.EnableColumnMenu = false;
            this.printListView.OptionsSelection.MultiSelect = true;
            // 
            // projectPDF
            // 
            this.projectPDF.Location = new System.Drawing.Point(137, 8);
            this.projectPDF.Name = "projectPDF";
            this.projectPDF.Properties.Caption = "PDF输出";
            this.projectPDF.Size = new System.Drawing.Size(93, 19);
            this.projectPDF.TabIndex = 9;
            this.projectPDF.CheckedChanged += new System.EventHandler(this.projectPDF_CheckedChanged);
            // 
            // callTemplate
            // 
            this.callTemplate.Location = new System.Drawing.Point(120, 52);
            this.callTemplate.Name = "callTemplate";
            this.callTemplate.Size = new System.Drawing.Size(110, 23);
            this.callTemplate.TabIndex = 8;
            this.callTemplate.Text = "导入打印设置模板";
            this.callTemplate.Click += new System.EventHandler(this.callTemplate_Click);
            // 
            // saveTemplate
            // 
            this.saveTemplate.Location = new System.Drawing.Point(5, 52);
            this.saveTemplate.Name = "saveTemplate";
            this.saveTemplate.Size = new System.Drawing.Size(109, 23);
            this.saveTemplate.TabIndex = 7;
            this.saveTemplate.Text = "保存打印设置模板";
            this.saveTemplate.Click += new System.EventHandler(this.saveTemplate_Click);
            // 
            // projectCatalog
            // 
            this.projectCatalog.EditValue = true;
            this.projectCatalog.Location = new System.Drawing.Point(137, 31);
            this.projectCatalog.Name = "projectCatalog";
            this.projectCatalog.Properties.Caption = "按工程分类打印";
            this.projectCatalog.Size = new System.Drawing.Size(120, 19);
            this.projectCatalog.TabIndex = 6;
            this.projectCatalog.CheckedChanged += new System.EventHandler(this.projectCatalog_CheckedChanged);
            // 
            // calcTotalPage
            // 
            this.calcTotalPage.Location = new System.Drawing.Point(3, 31);
            this.calcTotalPage.Name = "calcTotalPage";
            this.calcTotalPage.Properties.Caption = "计算页数(耗时较长)";
            this.calcTotalPage.Size = new System.Drawing.Size(128, 19);
            this.calcTotalPage.TabIndex = 5;
            this.calcTotalPage.CheckedChanged += new System.EventHandler(this.calcTotalPage_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(335, 52);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消(&C)";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(243, 52);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "确定(&O)";
            // 
            // ckCatalogue
            // 
            this.ckCatalogue.Location = new System.Drawing.Point(3, 8);
            this.ckCatalogue.Name = "ckCatalogue";
            this.ckCatalogue.Properties.Caption = "打印目录";
            this.ckCatalogue.Size = new System.Drawing.Size(128, 19);
            this.ckCatalogue.TabIndex = 2;
            this.ckCatalogue.CheckedChanged += new System.EventHandler(this.ckCatalogue_CheckedChanged);
            // 
            // startPageNum
            // 
            this.startPageNum.Location = new System.Drawing.Point(308, 7);
            this.startPageNum.Name = "startPageNum";
            this.startPageNum.Size = new System.Drawing.Size(100, 21);
            this.startPageNum.TabIndex = 1;
            this.startPageNum.EditValueChanged += new System.EventHandler(this.startPageNum_EditValueChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(254, 10);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "起始页码";
            // 
            // ReportBatchSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 518);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.ReportList);
            this.Controls.Add(this.batchRightMove);
            this.Controls.Add(this.batchLeftMove);
            this.Name = "ReportBatchSet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.ReportList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.printList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.printListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectPDF.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectCatalog.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.calcTotalPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckCatalogue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startPageNum.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private Com.Bing.UI.BaseTreeList ReportList;
		private DevExpress.XtraEditors.SimpleButton batchRightMove;
		private DevExpress.XtraEditors.SimpleButton batchLeftMove;
		private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
		private DevExpress.XtraGrid.GridControl printList;
		private DevExpress.XtraGrid.Views.Grid.GridView printListView;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOk;
		private DevExpress.XtraEditors.CheckEdit ckCatalogue;
		private DevExpress.XtraEditors.TextEdit startPageNum;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.CheckEdit calcTotalPage;
        private DevExpress.XtraEditors.CheckEdit projectCatalog;
        private DevExpress.XtraEditors.SimpleButton callTemplate;
        private DevExpress.XtraEditors.SimpleButton saveTemplate;
        private DevExpress.XtraEditors.CheckEdit projectPDF;


	}
}