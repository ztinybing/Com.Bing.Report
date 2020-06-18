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
            ���� = 0,
            ���� = 1
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
            adjustOrder(UpOrDown.����);
        }
        
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            adjustOrder(UpOrDown.����);
        }
        private void adjustOrder(UpOrDown upOrDown)
        {
            string focusedColName = listBoxControl1.Text;
            int index = listColName.IndexOf(focusedColName);
            if (upOrDown == UpOrDown.����)
            {
                if (index == 0)
                {
                    Function.Alert("�Ѿ��ǵ�һ���ˣ�����������", "��ʾ");
                    return;
                }
                index--;
            }
            if (upOrDown == UpOrDown.����)
            {
                if (index == listColName.Count - 1)
                {
                    Function.Alert("�Ѿ������һ���ˣ�����������", "��ʾ");
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