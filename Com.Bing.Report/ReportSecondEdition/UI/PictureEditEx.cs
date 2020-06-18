using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace Com.Bing.Report
{
    public class CheckedPictureEdit : PictureEdit
    {
        private static readonly object EVENT_CHECKEDCHANGED = new object();
        private Color checkedColor = Color.Moccasin;
        public event EventHandler CheckedChanged
        {
            add
            {
                Events.AddHandler(EVENT_CHECKEDCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_CHECKEDCHANGED, value);
            }
        }
        [Browsable(true)]
        public Color CheckedColor
        {
            get { return checkedColor; }
            set { checkedColor = value; }
        }
        private Color unCheckedColor = Color.White;
        [Browsable(true)]
        public Color UnCheckedColor
        {
            get { return unCheckedColor; }
            set { unCheckedColor = value; }
        }
        private bool _checked = false;

        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [Bindable(true)]
        [SettingsBindable(true)]
        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                OnCheckedChanged();
            }
        }
        void CheckedPictureEdit_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
            this.Parent.Focus();
        }
        private void OnCheckedChanged()
        {
            this.BackColor = _checked ? checkedColor : UnCheckedColor;
            EventHandler handler = (EventHandler)Events[EVENT_CHECKEDCHANGED];
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
        public CheckedPictureEdit()
        {
            this.Click += new EventHandler(CheckedPictureEdit_Click);
        }

    }

    public class SelectColorPictureEdit : PictureEdit
    {
        private static readonly object EVENT_COLORCHANGED = new object();
        public event EventHandler ColorChanged
        {
            add { Events.AddHandler(EVENT_COLORCHANGED, value); }
            remove { Events.RemoveHandler(EVENT_COLORCHANGED, value); }
        }
        private ColorDialog colDialog = new ColorDialog();
        private Color selectedColor;
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [SettingsBindable(true)]
        public Color SelectedColor
        {
            get { return selectedColor; }
            set
            {
                if (selectedColor != value)
                {
                    selectedColor = value;
                    OnSelectedColorChanged();
                }
            }
        }
        private void OnSelectedColorChanged()
        {
            this.BackColor = this.selectedColor;
            EventHandler handler = (EventHandler)Events[EVENT_COLORCHANGED];
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
            this.Parent.Focus();
        }
        public SelectColorPictureEdit()
        {
            this.Click += new EventHandler(SelectColorPictureEdit_Click);
        }
        void SelectColorPictureEdit_Click(object sender, EventArgs e)
        {
            if (colDialog.ShowDialog() == DialogResult.OK)
            {
                this.SelectedColor = colDialog.Color;
            }
        }
    }
}
