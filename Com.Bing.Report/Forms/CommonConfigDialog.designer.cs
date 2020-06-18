namespace Com.Bing.Forms
{
    partial class CommonConfigDialog
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.nextStep = new DevExpress.XtraEditors.SimpleButton();
            this.cancel = new DevExpress.XtraEditors.SimpleButton();
            this.preStep = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // nextStep
            // 
            this.nextStep.Location = new System.Drawing.Point(256, 292);
            this.nextStep.Name = "nextStep";
            this.nextStep.Size = new System.Drawing.Size(75, 23);
            this.nextStep.TabIndex = 1;
            this.nextStep.Text = "下一步(&N)";
            this.nextStep.Click += new System.EventHandler(this.nextStep_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(346, 292);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "取消(&C)";
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // preStep
            // 
            this.preStep.Location = new System.Drawing.Point(17, 292);
            this.preStep.Name = "preStep";
            this.preStep.Size = new System.Drawing.Size(75, 23);
            this.preStep.TabIndex = 3;
            this.preStep.Text = "上一步(&P)";
            this.preStep.Click += new System.EventHandler(this.preStep_Click);
            // 
            // CommonConfigDialog
            // 
            this.Appearance.Font = new System.Drawing.Font("宋体", 9F);
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 327);
            this.Controls.Add(this.preStep);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.nextStep);
            this.Name = "CommonConfigDialog";
            this.Text = "参数设置";
            this.Controls.SetChildIndex(this.nextStep, 0);
            this.Controls.SetChildIndex(this.cancel, 0);
            this.Controls.SetChildIndex(this.preStep, 0);
            this.ResumeLayout(false);

        }

        #endregion
       
        private DevExpress.XtraEditors.SimpleButton nextStep;
        private DevExpress.XtraEditors.SimpleButton cancel;
        private DevExpress.XtraEditors.SimpleButton preStep;
    }
}