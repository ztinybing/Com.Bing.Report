using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;

namespace Com.Bing.Forms
{
    public partial class BaseDialog : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public WeakReference preFocusControl = null;

        Form activeForm = null;
		public BaseDialog()
        {
            InitializeComponent();
            Button btnCancel = new Button();
            btnCancel.Click +=new EventHandler(btnCancel_Click);
            this.CancelButton = btnCancel;
            activeForm = GetActiveForm();
            this.FormClosing += new FormClosingEventHandler(BaseDialog_FormClosing);
            this.FormClosed += new FormClosedEventHandler(BaseDialog_FormClosed);
            this.VisibleChanged += new EventHandler(BaseDialog_VisibleChanged);
        }

        public static Form GetActiveForm()
        {
            Form frm = Form.ActiveForm;
            if (frm is IOwnerDisable)
            {
                frm = frm.ParentForm;
            }
            return frm;
        }

        /// <summary>
        /// 设置上一个获取焦点的控件
        /// </summary>
        private void SetLastFocusControl()
        {
            Control control = Com.Bing.API.Function.GetFoucsControl();
            if (control != null)
            {
                preFocusControl = new WeakReference(control);
            }
            else
            {
                preFocusControl = null;
            }
        }

        /// <summary>
        /// 使上一个获取焦点的控件获取焦点
        /// </summary>
        private void FocusLastFocusControl()
        {
            if (preFocusControl != null && preFocusControl.IsAlive)
            {
                Control control = preFocusControl.Target as Control;
                if (control != null)
                    control.Focus();
            }
        }

        void BaseDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            FocusLastFocusControl();
        }

        //构造函数中指定的owner，在可见性改变后会失效
		void BaseDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (!DesignMode && this.Visible && activeForm != null && !activeForm.IsDisposed)
            {
	            this.Owner = activeForm;
            }
        }


        void BaseDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Owner = null;            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();            
        }

        public  new void Show()
        {
            SetLastFocusControl();
            base.Show();
        }
        public new void Show(IWin32Window owner)
        {
            SetLastFocusControl();
            base.Show(owner);
        }
        public new DialogResult ShowDialog()
        {
            SetLastFocusControl();
            return base.ShowDialog();
        }       
        public new DialogResult ShowDialog(IWin32Window owner)
        {
            SetLastFocusControl();
            return base.ShowDialog(owner);
        }
        public new void Hide()
        {
            FocusLastFocusControl();
            base.Hide();
        }
    }

    public interface IOwnerDisable
    {
    }
}