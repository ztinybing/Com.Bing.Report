using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Com.Bing.API;
using Com.Bing.Business;

namespace Com.Bing.Forms
{
    public partial class ExceptionDialog : BaseDialog
    {
        public ExceptionDialog(Exception e)
        {
            InitializeComponent();
            if (e is InnerException)
            {
                e = (e as InnerException).InnerException;
            }
            txtError.Text = Application.ExecutablePath + "\r\n" + e.Message + "\r\n" + e.StackTrace;
            Function.RecordError(txtError.Text);
            Function.SendMail(txtError.Text);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtError.Text);
            Function.Alert("错误信息已经复制到剪贴板", "提示");
        }
    }
}