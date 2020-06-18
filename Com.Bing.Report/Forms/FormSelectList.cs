using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Com.Bing.Forms
{
    public partial class FormSelectList : Com.Bing.Forms.BaseDialog
    {
        public delegate void SelectedEventHandle(object selectedValue);
        public SelectedEventHandle selectedClickEvent;
        //·µ»ØÑ¡ÔñµÄ×Ö·û´®
        private object value;
        public object Value
        {
            get { return this.value; }
        }
        DataTable dt = new DataTable();
        public FormSelectList(string title, Dictionary<object, string> dict)
        {
            InitializeComponent();
            DataBound(title, dict);
        }

        public FormSelectList(string title, Dictionary<object, string> dict, bool[] columnVisible)
        {
            InitializeComponent();
            DataBound(title, dict);
            for (int i = 0; i < columnVisible.Length; i++)
            {
                gridView1.Columns[i].Visible = columnVisible[i];
            }

        }
        private void DataBound(string title, Dictionary<object, string> dict)
        {
            this.Text = title;
            dt.Columns.Add("obj", typeof(object));
            dt.Columns.Add("mc", typeof(string));
            foreach (KeyValuePair<object, string> pair in dict)
            {
                DataRow row = dt.NewRow();
                row["obj"] = pair.Key;
                row["mc"] = pair.Value;
                dt.Rows.Add(row);

            }
            gridControl1.DataSource = dt;
        }



        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                DialogResult = DialogResult.OK;
                value = dt.Rows[gridView1.FocusedRowHandle]["obj"];
            }

            if (selectedClickEvent != null)
            {
                selectedClickEvent(value);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void gridControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = gridView1.CalcHitInfo(new Point(e.X, e.Y));
            if (info.RowHandle >= 0)
            {
                gridView1.FocusedRowHandle = info.RowHandle;
                simpleButton1_Click(sender, e);
            }
        }
    }
}