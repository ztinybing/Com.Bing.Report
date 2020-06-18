using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Com.Bing.UI
{
    public class BaseTextBox : DevExpress.XtraEditors.MemoEdit
    {
        public char PasswordChar
        {
            get { return this.Properties.PasswordChar; }
            set { this.Properties.PasswordChar = value; }
        }
    }
}
