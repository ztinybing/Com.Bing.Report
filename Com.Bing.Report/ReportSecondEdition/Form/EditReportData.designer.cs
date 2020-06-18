namespace Com.Bing.Report
{
    partial class EditReportData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditReportData));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tool_Save = new System.Windows.Forms.ToolStripButton();
            this.tool_Merge = new System.Windows.Forms.ToolStripButton();
            this.tool_CancelMerge = new System.Windows.Forms.ToolStripButton();
            this.tool_InsertBorder = new System.Windows.Forms.ToolStripButton();
            this.tool_cancelBorder = new System.Windows.Forms.ToolStripButton();
            this.tool_borderSet = new System.Windows.Forms.ToolStripButton();
            this.tool_FontSet = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton21 = new System.Windows.Forms.ToolStripSplitButton();
            this.左对齐ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.中对齐ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.右对齐ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.上对齐ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.中对齐ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.下对齐ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.sheet = new FarPoint.Win.Spread.SheetView();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sheet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_Save,
            this.tool_Merge,
            this.tool_CancelMerge,
            this.tool_InsertBorder,
            this.tool_cancelBorder,
            this.tool_borderSet,
            this.tool_FontSet,
            this.toolStripButton21});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(644, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tool_Save
            // 
            this.tool_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tool_Save.Image = global::Com.Bing.Properties.Resource.save;
            this.tool_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_Save.Name = "tool_Save";
            this.tool_Save.Size = new System.Drawing.Size(23, 22);
            this.tool_Save.Text = "保存";
            this.tool_Save.Click += new System.EventHandler(this.tool_Save_Click);
            // 
            // tool_Merge
            // 
            this.tool_Merge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tool_Merge.Image = global::Com.Bing.Properties.Resource.mCell;
            this.tool_Merge.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_Merge.Name = "tool_Merge";
            this.tool_Merge.Size = new System.Drawing.Size(23, 22);
            this.tool_Merge.Text = "合并单元格";
            this.tool_Merge.Click += new System.EventHandler(this.tool_Merge_Click);
            // 
            // tool_CancelMerge
            // 
            this.tool_CancelMerge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tool_CancelMerge.Image = global::Com.Bing.Properties.Resource.CmCell;
            this.tool_CancelMerge.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_CancelMerge.Name = "tool_CancelMerge";
            this.tool_CancelMerge.Size = new System.Drawing.Size(23, 22);
            this.tool_CancelMerge.Text = "取消合并";
            this.tool_CancelMerge.Click += new System.EventHandler(this.tool_CancelMerge_Click);
            // 
            // tool_InsertBorder
            // 
            this.tool_InsertBorder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tool_InsertBorder.Image = global::Com.Bing.Properties.Resource.b_all;
            this.tool_InsertBorder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_InsertBorder.Name = "tool_InsertBorder";
            this.tool_InsertBorder.Size = new System.Drawing.Size(23, 22);
            this.tool_InsertBorder.Text = "添加边框";
            this.tool_InsertBorder.Click += new System.EventHandler(this.tool_InsertBorder_Click);
            // 
            // tool_cancelBorder
            // 
            this.tool_cancelBorder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tool_cancelBorder.Image = global::Com.Bing.Properties.Resource.b_none;
            this.tool_cancelBorder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_cancelBorder.Name = "tool_cancelBorder";
            this.tool_cancelBorder.Size = new System.Drawing.Size(23, 22);
            this.tool_cancelBorder.Text = "取消边框";
            this.tool_cancelBorder.Click += new System.EventHandler(this.tool_cancelBorder_Click);
            // 
            // tool_borderSet
            // 
            this.tool_borderSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tool_borderSet.Image = ((System.Drawing.Image)(resources.GetObject("tool_borderSet.Image")));
            this.tool_borderSet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_borderSet.Name = "tool_borderSet";
            this.tool_borderSet.Size = new System.Drawing.Size(23, 22);
            this.tool_borderSet.Text = "toolStripButton1";
            this.tool_borderSet.Click += new System.EventHandler(this.tool_borderSet_Click);
            // 
            // tool_FontSet
            // 
            this.tool_FontSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tool_FontSet.Image = global::Com.Bing.Properties.Resource.fontStyle;
            this.tool_FontSet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_FontSet.Name = "tool_FontSet";
            this.tool_FontSet.Size = new System.Drawing.Size(23, 22);
            this.tool_FontSet.Text = "字体设置";
            this.tool_FontSet.Click += new System.EventHandler(this.tool_FontSet_Click);
            // 
            // toolStripButton21
            // 
            this.toolStripButton21.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton21.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.左对齐ToolStripMenuItem,
            this.中对齐ToolStripMenuItem,
            this.右对齐ToolStripMenuItem,
            this.上对齐ToolStripMenuItem,
            this.中对齐ToolStripMenuItem1,
            this.下对齐ToolStripMenuItem});
            this.toolStripButton21.Image = global::Com.Bing.Properties.Resource.cc;
            this.toolStripButton21.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton21.Name = "toolStripButton21";
            this.toolStripButton21.Size = new System.Drawing.Size(32, 22);
            this.toolStripButton21.Text = "对齐方式";
            // 
            // 左对齐ToolStripMenuItem
            // 
            this.左对齐ToolStripMenuItem.Image = global::Com.Bing.Properties.Resource.lc;
            this.左对齐ToolStripMenuItem.Name = "左对齐ToolStripMenuItem";
            this.左对齐ToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.左对齐ToolStripMenuItem.Text = "左对齐";
            this.左对齐ToolStripMenuItem.Click += new System.EventHandler(this.左对齐ToolStripMenuItem_Click);
            // 
            // 中对齐ToolStripMenuItem
            // 
            this.中对齐ToolStripMenuItem.Image = global::Com.Bing.Properties.Resource.cc;
            this.中对齐ToolStripMenuItem.Name = "中对齐ToolStripMenuItem";
            this.中对齐ToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.中对齐ToolStripMenuItem.Text = "中对齐";
            this.中对齐ToolStripMenuItem.Click += new System.EventHandler(this.中对齐ToolStripMenuItem_Click);
            // 
            // 右对齐ToolStripMenuItem
            // 
            this.右对齐ToolStripMenuItem.Image = global::Com.Bing.Properties.Resource.rc;
            this.右对齐ToolStripMenuItem.Name = "右对齐ToolStripMenuItem";
            this.右对齐ToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.右对齐ToolStripMenuItem.Text = "右对齐";
            this.右对齐ToolStripMenuItem.Click += new System.EventHandler(this.右对齐ToolStripMenuItem_Click);
            // 
            // 上对齐ToolStripMenuItem
            // 
            this.上对齐ToolStripMenuItem.Image = global::Com.Bing.Properties.Resource.top;
            this.上对齐ToolStripMenuItem.Name = "上对齐ToolStripMenuItem";
            this.上对齐ToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.上对齐ToolStripMenuItem.Text = "上对齐";
            this.上对齐ToolStripMenuItem.Click += new System.EventHandler(this.上对齐ToolStripMenuItem_Click);
            // 
            // 中对齐ToolStripMenuItem1
            // 
            this.中对齐ToolStripMenuItem1.Image = global::Com.Bing.Properties.Resource.center;
            this.中对齐ToolStripMenuItem1.Name = "中对齐ToolStripMenuItem1";
            this.中对齐ToolStripMenuItem1.Size = new System.Drawing.Size(106, 22);
            this.中对齐ToolStripMenuItem1.Text = "中对齐";
            this.中对齐ToolStripMenuItem1.Click += new System.EventHandler(this.中对齐ToolStripMenuItem1_Click);
            // 
            // 下对齐ToolStripMenuItem
            // 
            this.下对齐ToolStripMenuItem.Image = global::Com.Bing.Properties.Resource.bottom;
            this.下对齐ToolStripMenuItem.Name = "下对齐ToolStripMenuItem";
            this.下对齐ToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.下对齐ToolStripMenuItem.Text = "下对齐";
            this.下对齐ToolStripMenuItem.Click += new System.EventHandler(this.下对齐ToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fpSpread1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(644, 350);
            this.panel1.TabIndex = 3;
            // 
            // sheet
            // 
            this.sheet.Reset();
            this.sheet.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.sheet.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.sheet.RowHeader.ColumnCount = 0;
            this.sheet.RowHeader.Columns.Default.Resizable = false;
            this.sheet.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // fpSpread1
            // 
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1, Row 0, Column 0, ";
            this.fpSpread1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
            this.fpSpread1.ColumnSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(0, 0);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.sheet});
            this.fpSpread1.Size = new System.Drawing.Size(644, 350);
            this.fpSpread1.TabIndex = 1;
            this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            // 
            // EditReportData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 375);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "EditReportData";
            this.Text = "报表数据编辑";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditReportData_FormClosed);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sheet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tool_Merge;
        private System.Windows.Forms.ToolStripButton tool_CancelMerge;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton tool_InsertBorder;
        private System.Windows.Forms.ToolStripButton tool_cancelBorder;
        private System.Windows.Forms.ToolStripButton tool_borderSet;
        private System.Windows.Forms.ToolStripButton tool_FontSet;
        private System.Windows.Forms.ToolStripSplitButton toolStripButton21;
        private System.Windows.Forms.ToolStripMenuItem 左对齐ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 中对齐ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 右对齐ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 上对齐ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 中对齐ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 下对齐ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tool_Save;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView sheet;
    }
}