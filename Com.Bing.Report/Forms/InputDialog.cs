using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Com.Bing.Forms
{
    public partial class InputDialog : BaseDialog
    {
        private bool allowEmpty = false;
        private bool allowMulti = false;
        private void Init(string title, string def, bool allowMulti, bool allowEmpty, bool allowPassword)
        {
            InitializeComponent();
            this.allowEmpty = allowEmpty;
            this.teInput.PasswordChar = allowPassword ? '*' : (char)0;
            this.Text = title;
            this.TopMost = true;
            TextResult = def;
            this.allowMulti = allowMulti;
            if (allowMulti)
            {
                this.Height *= 2;
            }
            this.Activate();
        }

        public InputDialog()
        {
            Init("«Î ‰»ÎŒƒ±æ:", "", false, false, false);
        }

        public InputDialog(string title, string def)
        {
            Init(title, def, false, false, false);
        }

        public InputDialog(string title, string def, bool allowMulti)
        {
            Init(title, def, allowMulti, false, false);
        }

        public InputDialog(string title, string def, bool allowMulti, bool allowEmpty)
        {
            Init(title, def, allowMulti, allowEmpty, false);
        }

        public InputDialog(string title, string def, bool allowMulti, bool allowEmpty, bool allowPassword)
        {
            Init(title, def, allowMulti, allowEmpty, allowPassword);
        }

        public String TextResult
        {
            get
            {
                return teInput.Text;
            }
            set
            {
                teInput.Text = value;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (TextResult != "" || allowEmpty)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InputFrm_Load(object sender, EventArgs e)
        {
            teInput.Focus();
        }

        private void teInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !allowMulti)
            {
                btnOk_Click(sender, e);
            }
        }
    }
}