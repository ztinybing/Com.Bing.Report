namespace Com.Bing.Report
{
    partial class UIPreviewReport
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.reportstyle = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ReportMode = new System.Windows.Forms.PictureBox();
            this.reportstyle.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReportMode)).BeginInit();
            this.SuspendLayout();
            // 
            // reportstyle
            // 
            this.reportstyle.Controls.Add(this.tabPage1);
            this.reportstyle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportstyle.Location = new System.Drawing.Point(0, 0);
            this.reportstyle.Name = "reportstyle";
            this.reportstyle.SelectedIndex = 0;
            this.reportstyle.Size = new System.Drawing.Size(698, 400);
            this.reportstyle.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.reportstyle.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.DarkGray;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPage1.Controls.Add(this.ReportMode);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(690, 375);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "报表样式";
            // 
            // ReportMode
            // 
            this.ReportMode.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ReportMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportMode.Location = new System.Drawing.Point(3, 3);
            this.ReportMode.Name = "ReportMode";
            this.ReportMode.Size = new System.Drawing.Size(680, 365);
            this.ReportMode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ReportMode.TabIndex = 6;
            this.ReportMode.TabStop = false;
            this.ReportMode.Tag = "";
            this.ReportMode.WaitOnLoad = true;
            this.ReportMode.DoubleClick += new System.EventHandler(this.ReportMode_DoubleClick);
            // 
            // ReviewReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.reportstyle);
            this.Name = "ReviewReport";
            this.Size = new System.Drawing.Size(698, 400);
            this.reportstyle.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ReportMode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl reportstyle;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PictureBox ReportMode;
    }
}
