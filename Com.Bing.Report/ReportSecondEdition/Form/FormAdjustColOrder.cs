using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Com.Bing.API;

namespace Com.Bing.Report
{
    public partial class FormAdjustColOrder : Com.Bing.Forms.BaseDialog
    {
        public enum UpOrDown
        {
            上移 = 0,
            下移 = 1
        }

        private List<string> listColName = new List<string>();
        public FormAdjustColOrder(List<string> listColName)
        {
            InitializeComponent();
            this.listColName = listColName;
            listBoxControl1.DataSource = listColName;
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            adjustOrder(UpOrDown.上移);
        }
        
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            adjustOrder(UpOrDown.下移);
        }
        private void adjustOrder(UpOrDown upOrDown)
        {
            string focusedColName = listBoxControl1.Text;
            int index = listColName.IndexOf(focusedColName);
            if (upOrDown == UpOrDown.上移)
            {
                if (index == 0)
                {
                    Function.Alert("已经是第一列了，不能上移了", "提示");
                    return;
                }
                index--;
            }
            if (upOrDown == UpOrDown.下移)
            {
                if (index == listColName.Count - 1)
                {
                    Function.Alert("已经是最后一列了，不能下移了", "提示");
                    return;
                }
                index++;
            }
            listColName.Remove(focusedColName);
            listColName.Insert(index, focusedColName);
            listBoxControl1.Refresh();
            listBoxControl1.SelectedIndex = index;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}